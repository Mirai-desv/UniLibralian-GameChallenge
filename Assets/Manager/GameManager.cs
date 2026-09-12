using UnityEngine;
using System.Collections.Generic;
using System.Linq;

// Cho phép gán asset (sprite) hiển thị khác nhau theo từng BookType khi sách đã xếp xong vào kệ.
// Nếu để trống mảng này, placedBookVisualPrefab sẽ dùng sprite mặc định có sẵn trên chính prefab.
[System.Serializable]
public class PlacedBookVisualEntry
{
    public BookType Type;
    public Sprite Sprite;
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Refs")]
    [SerializeField] private BookshelfController bookshelfController;
    [SerializeField] private HoldingTray holdingTray;

    [Header("Level Data")]
    [SerializeField] private LevelData currentLevel; // dùng chung asset với LevelData của BookShelfController
    [SerializeField] private Book bookPrefab;
    [SerializeField] private Transform boardContainer; // để trống cũng được, chỉ để gọi Hierarchy

    [Header("Placed Book Visual")]
    [SerializeField] private GameObject placedBookVisualPrefab;
    // Nếu muốn mỗi loại sách (BookType) hiện 1 sprite khác nhau khi đã xếp vào kệ
    [SerializeField] private PlacedBookVisualEntry[] placedBookVisualsByType;
    private List<GameObject> waitingBookVisuals = new List<GameObject>();
    private Dictionary<BookSpace, GameObject> trayVisuals = new Dictionary<BookSpace, GameObject>();
    private List<Bookshelf> Bookshelfs = new List<Bookshelf>();
    private List<Book> allBooks = new List<Book>();

    // Dùng cho điều keienj thắng: thắng khi số sách đã xếp xong == tổng số sách của level
    private int totalBooksToPlace = 0;
    private int booksPlaced = 0;
    private bool isLevelOver = false;
    private List<GameObject> holdingTrayVisuals = new List<GameObject>();
    public List<BookSpace> Spaces = new List<BookSpace>();

    // Nếu chưa có GameManager thì sử dụng GameManager này để ko hủy logic game
    private void Awake()
    {
        if(instance == null || instance == this)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Mỗi khi 1 kệ mới xuất hiện, thử xem có sách nào đang chờ trong HoldingTray khớp Type không
        if(bookshelfController != null)
        {
            bookshelfController.OnShelfSpawned += HandleShelfSpawned;
        }

        // Hàng chờ đầy hoàn toàn thì thua
        if(holdingTray != null)
        {
            holdingTray.OnTrayFull += HandleLevelLose;
        }
        LoadLevel();
    }

    void LoadLevel()
    {
        GenerateLevel();
        // Check trạng thái bị che ngay khi vừa xuất hiện level
        RefreshAllBlockedStates();
    }

    void GenerateLevel()
    {
        int existingBookCount =
            FindObjectsByType<Book>(FindObjectsInactive.Include, FindObjectsSortMode.None).Length;
        if (existingBookCount > 0)
        {
            return;
        }

        if(currentLevel == null)
        {
            Debug.LogWarning("GameManager chưa được gán LevelData, không có sách nào để sinh ra.");
            return;
        }

        if(bookPrefab == null)
        {
            Debug.LogWarning("GameManager chưa được gán Book Prefab, không thể sinh sách.");
            return;
        }

        foreach(BookSpawnData data in currentLevel.BookLayout)
        {
            Book newBook = Instantiate(bookPrefab, data.Position, Quaternion.identity, boardContainer);
            newBook.Initialize(data.Type, data.SortingOrder);
        }
    }

    // Book tự gọi hàm này khi Start() để được GameManager quản lý
    public void RegisterBook(Book book)
    {
        if(book == null || allBooks.Contains(book)) return;
        allBooks.Add(book);
        totalBooksToPlace++;
    }

    public void RefreshAllBlockedStates()
    {
        foreach(var book in allBooks)
        {
            if(book != null)
            {
                book.UpdateBlockedState();
            }
        }
    }

    public void HandleBookSelected(Book book)
    {
        if(isLevelOver) return;
        // Tìm kệ đang active có cùng Type và còn ô trống
        Bookshelf targetShelf = bookshelfController.FindMatchingShelf(book.Type);
        if(targetShelf != null)
        {
            PlaceBookInShelf(book, targetShelf);
            return;
        }
        PlaceBookInTray(book);
    }

    private void PlaceBookInShelf(Book book, Bookshelf shelf)
    {
        BookSpace targetSpace = shelf.GetFirstEmptySpace();
        if(targetSpace == null)
        {
            // Kệ vừa bị lấp đầy bởi 1 lượt khác ngay trước đó
            Debug.LogWarning($"Kệ {shelf.Type} báo còn chỗ nhưng GetFirstEmptySpace() lại null.");
            return;
        }

        // Giữ chỗ ngay lập tức, tránh 2 lượt sách nhắm cùng 1 ô
        targetSpace.AssignBook(book);

        Vector3 targetPos = targetSpace.SpaceTransform.position;
        book.MoveToPosition(targetPos, () =>
        {
            // Sách đã bay tới đúng ô -> thay bằng asset MỚI (không cần tương tác nữa),
            SpawnPlacedBookVisual(book.Type, targetSpace.SpaceTransform, book.SpriteRenderer.sortingOrder);

            allBooks.Remove(book);
            Destroy(book.gameObject);

            // Báo cho kệ biết vừa có thêm 1 ô được lắp, kệ sẽ tự kiểm tra xem đã đầy chưa
            shelf.NotifySpaceAssigned();

            // Sách đã xếp xong -> tính vào điều kiện thắng level
            booksPlaced++;
            CheckWinCondition();

            RefreshAllBlockedStates();
        });
    }

    /*
    private void SpawnTrayBookVisual(BookType type, Transform parent, int sortingOrder)
    {
        if (placedBookVisualPrefab == null) return;
        GameObject visual = Instantiate(placedBookVisualPrefab, parent.position, Quaternion.identity, parent);
        holdingTrayVisuals.Add(visual);

        SpriteRenderer sr = visual.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sortingOrder = sortingOrder;
            Sprite matchedSprite = placedBookVisualsByType?.FirstOrDefault(e => e.Type == type)?.Sprite;
            if (matchedSprite != null) sr.sprite = matchedSprite;
        }
    }
    */
    private void ClearTrayVisuals(BookSpace space)
    {
        int index = holdingTray.GetIndexOfSpace(space);
        if (index >= 0 && index < holdingTrayVisuals.Count)
        {
            Destroy(holdingTrayVisuals[index]);
            holdingTrayVisuals.RemoveAt(index);
        }
    }

    private void SpawnPlacedBookVisual(BookType type, Transform parent, int sortingOrder, BookSpace space = null)
    {
        if (placedBookVisualPrefab == null)
        {
            Debug.LogWarning("Chưa gán Placed Book Visual Prefab!");
            return;
        }

        GameObject visual = Instantiate(placedBookVisualPrefab, parent.position, Quaternion.identity, parent);

        SpriteRenderer sr = visual.GetComponent<SpriteRenderer>();

        if (sr != null)
        {
            sr.sortingOrder = sortingOrder;

            Sprite matchedSprite = placedBookVisualsByType?.FirstOrDefault(e => e.Type == type)?.Sprite;

            if (matchedSprite != null)
            {
                sr.sprite = matchedSprite;
            }
        }

        // Nếu visual thuộc HoldingTray thì lưu lại để sau này xóa được
        if (space != null)
        {
            trayVisuals[space] = visual;
        }
    }

    private void RemovePlacedBookVisual(BookSpace space)
    {
        if (space == null)
            return;

        if (trayVisuals.TryGetValue(space, out GameObject visual))
        {
            if (visual != null)
            {
                Destroy(visual);
            }

            trayVisuals.Remove(space);
        }
    }

    // Gửi sách vào ô trống đầu tiên của hàng chờ (holding tray) khi chưa có kệ nào khớp Type
    private void PlaceBookInTray(Book book)
    {
        if(isLevelOver) return;
        if(holdingTray == null)
        {
            Debug.LogWarning("GameManager chưa được gán HoldingTray, sách tạm thời đúng yên.");
            return;
        }

        BookSpace traySpace = holdingTray.GetFirstEmptySpace();
        if(traySpace == null)
        {
            // Hàng chờ đầy = thua
            HandleLevelLose();
            return;
        }

        traySpace.AssignBook(book);

        Vector3 targetPos = traySpace.SpaceTransform.position;
        book.MoveToPosition(targetPos, () =>
        {
            SpawnPlacedBookVisual(book.Type, traySpace.SpaceTransform, book.SpriteRenderer.sortingOrder, traySpace);
            book.transform.SetParent(traySpace.SpaceTransform, worldPositionStays: true);
            // Sách đã rời board chính (không còn tương tác/ che sách khác nữa) nên bỏ khỏi allBooks, nhưng vẫn tồn tại trong tray, chờ tới khi có kệ khớp Type xuất hiện
            allBooks.Remove(book);
            holdingTray.NotifySpaceAssigned();
            RefreshAllBlockedStates();
        });
    }

    // Gọi mỗi khi BookshelfController spawn 1 kệ mới -> thử rút sách đang chờ trong tray khớp Type
    private void HandleShelfSpawned(Bookshelf shelf)
    {
        if(isLevelOver) return;
        if (holdingTray == null) return;

        while(true)
        {
            BookSpace waitingSpace = holdingTray.FindWaitingBook(shelf.Type);
            if (waitingSpace == null)
                break;
            Book waitingBook = waitingSpace.CurrentBook;
            RemovePlacedBookVisual(waitingSpace);
            waitingSpace.Clear();
            if (waitingBook == null)
            {
                continue;
            }
            PlaceBookInShelf(waitingBook, shelf);
        }
    }

    private void CheckWinCondition()
    {
        if(isLevelOver) return;

        allBooks.RemoveAll(book => book == null);



        bool hasUnplacedBooks = allBooks.Any(book => book != null) ||
            (holdingTray != null && holdingTray.HasBooks);
        if (totalBooksToPlace > 0 && !hasUnplacedBooks)
        {
            HandleLevelWin();
        }
    }

    private void HandleLevelWin()
    {
        if(isLevelOver) return;
        isLevelOver = true;

        GameplayStateController stateController =
            FindFirstObjectByType<GameplayStateController>(FindObjectsInactive.Include);
        if (stateController != null)
        {
            stateController.SetState(GameplayStateController.GameState.Win);
        }
        else
        {
            Debug.LogError("Không tìm thấy GameplayStateController để bật WinPanel.");
        }

        if (LevelProgressManager.Instance != null)
        {
            int currentLevel = LevelLoader.Instance != null
                ? LevelLoader.Instance.GetCurrentLevel()
                : PlayerPrefs.GetInt("CurrentLevel", 0);
            LevelProgressManager.Instance.CompleteLevel(currentLevel);
        }

        Debug.Log("LEVEL COMPLETE! Đã xếp xong toàn bộ sách.");
    }

    private void HandleLevelLose()
    {
        if(isLevelOver) return;
        isLevelOver = true;

        GameplayStateController stateController = FindFirstObjectByType<GameplayStateController>();
        if (stateController != null)
        {
            stateController.SetState(GameplayStateController.GameState.Lose);
        }

        Debug.Log("GAME OVER! Hàng chờ đã đầy, không còn chỗ chứa sách chưa có kệ khớp.");
    }

    // Tạm thời để đây, khả năng là sẽ ko cần đến
    /*
    private void OnBookArrived(Book book, Bookshelf shelf, BookSpace space)
    {
        // Gắn sách làm con của BookSpace để giữ đúng vị trí & sortingOrder theo kệ
        book.transform.SetParent(space.SpaceTransform, worldPositionStays: true);

        // Đến nơi rồi nên coi như xóa book khỏi map
        allBooks.Remove(book);

        // Báo lại trạng thái cho kệ
        shelf.NotifySpaceAssigned();

        // Cập nhật lại trạng thái bị che
        RefreshAllBlockedStates();
    }
    */
}