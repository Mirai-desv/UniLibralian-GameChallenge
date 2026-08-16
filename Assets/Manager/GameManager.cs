using UnityEngine;
using System.Collections.Generic;

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

    private List<Bookshelf> Bookshelfs = new List<Bookshelf>();
    private List<Book> allBooks = new List<Book>();

    // Dùng cho điều keienj thắng: thắng khi số sách đã xếp xong == tổng số sách của level
    private int totalBooksToPlace = 0;
    private int booksPlaced = 0;
    private bool isLevelOver = false;

    // Nếu chưa có GameManager thì sử dụng GameManager này để ko hủy logic game
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
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
            // Kệ vừa bị lấp đầy bởi 1 lượt khác ngay trước đó (hiếm, phòng ngừa thôi)
            Debug.LogWarning($"Kệ {shelf.Type} báo còn chỗ nhưng GetFirstEmptySpace() lại null.");
            return;
        }

        // Giữ chỗ ngay lập tức, tránh 2 lượt sách nhắm cùng 1 ô
        targetSpace.AssignBook(book);

        Vector3 targetPos = targetSpace.SpaceTransform.position;
        book.MoveToPosition(targetPos, () =>
        {
            // Gắn sách làm con của BookSpace (giữ đúng vị trí & sortingPoder theo kệ)
            book.transform.SetParent(targetSpace.SpaceTransform, worldPositionStays: true);
            allBooks.Remove(book);

            // Báo cho kệ biết vừa có thêm 1 ô được lắp, kệ sẽ tự kiểm tra xem đã đầy chưa
            shelf.NotifySpaceAssigned();

            // Sách đã xếp xong -> tính vào điều kiện thắng level
            booksPlaced++;
            CheckWinCondition();

            RefreshAllBlockedStates();
        });
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
 
        BookSpace waitingSpace = holdingTray.FindWaitingBook(shelf.Type);
        if (waitingSpace == null) return;
 
        Book waitingBook = waitingSpace.CurrentBook;
        waitingSpace.Clear();
 
        PlaceBookInShelf(waitingBook, shelf);
    }

    private void CheckWinCondition()
    {
        if(isLevelOver) return;

        // totalBooksToPlace > 0 để tránh thắng ảo khi level chưa kịp đăng ký sách nào
        if(totalBooksToPlace > 0 && booksPlaced >= totalBooksToPlace)
        {
            HandleLevelWin();
        }
    }

    private void HandleLevelWin()
    {
        if(isLevelOver) return;
        isLevelOver = true;
        
        // Chưa có UI, tạm thời log thôi
        Debug.Log("LEVEL COMPLETE! Đã xếp xong toàn bộ sách.");
    }

    private void HandleLevelLose()
    {
        if(isLevelOver) return;
        isLevelOver = true;

        // Chưa có UI, tạm thời log thôi
        Debug.Log("GAME OVER! Hàng chờ đã đầy, không còn chỗ chứa sách chưa có kệ khớp.");
    }

    // Tạm thời để đây, khả năng là sẽ ko cần đến
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
}
