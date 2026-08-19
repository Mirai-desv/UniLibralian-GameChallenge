using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Refs")]
    [SerializeField] private BookshelfController bookshelfController;
    [SerializeField] private HoldingTray holdingTray;

    [Header("Level Data")]
    [SerializeField] private LevelData currentLevel;
    [SerializeField] private Book bookPrefab;
    [SerializeField] private Transform boardContainer;

    private List<Bookshelf> Bookshelfs = new List<Bookshelf>();
    private List<Book> allBooks = new List<Book>();
    [SerializeField] private GameplayStateController GameplayStateController;

    private int totalBooksToPlace = 0;
    private int booksPlaced = 0;
    private bool isLevelOver = false;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.LogWarning("Phát hiện 2 GameManager cùng tồn tại trong 1 scene, huỷ bản thừa.");
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }

        // Unsubscribe để tránh lỗi/leak khi object bị huỷ giữa chừng
        if (bookshelfController != null)
        {
            bookshelfController.OnShelfSpawned -= HandleShelfSpawned;
        }

        if (holdingTray != null)
        {
            holdingTray.OnTrayFull -= HandleLevelLose;
        }
    }

    private void Start()
    {
        if (GameplayStateController != null)
        {
            GameplayStateController.ResetGameState();
        }

        if (bookshelfController != null)
        {
            bookshelfController.OnShelfSpawned += HandleShelfSpawned;
        }

        if (holdingTray != null)
        {
            holdingTray.OnTrayFull += HandleLevelLose;
        }

        LoadLevel();
    }

    void LoadLevel()
    {
        GenerateLevel();
        RefreshAllBlockedStates();
    }

    void GenerateLevel()
    {
        if (currentLevel == null)
        {
            Debug.LogWarning("GameManager chưa được gán LevelData, không có sách nào để sinh ra.");
            return;
        }

        if (bookPrefab == null)
        {
            Debug.LogWarning("GameManager chưa được gán Book Prefab, không thể sinh sách.");
            return;
        }

        foreach (BookSpawnData data in currentLevel.BookLayout)
        {
            Book newBook = Instantiate(bookPrefab, data.Position, Quaternion.identity, boardContainer);
            newBook.Initialize(data.Type, data.SortingOrder);
            RegisterBook(newBook);
        }
    }

    public void RegisterBook(Book book)
    {
        if (book == null || allBooks.Contains(book)) return;
        allBooks.Add(book);
        totalBooksToPlace++;
    }

    public void RefreshAllBlockedStates()
    {
        foreach (var book in allBooks)
        {
            if (book != null)
            {
                book.UpdateBlockedState();
            }
        }
    }

    public void HandleBookSelected(Book book)
    {
        if (isLevelOver) return;

        if (GameplayStateController != null &&
            GameplayStateController.CurrentState != GameplayStateController.GameState.Playing)
        {
            return;
        }

        Bookshelf targetShelf = bookshelfController != null ? bookshelfController.FindMatchingShelf(book.Type) : null;
        if (targetShelf != null)
        {
            PlaceBookInShelf(book, targetShelf);
            return;
        }

        PlaceBookInTray(book);
    }

    private void PlaceBookInShelf(Book book, Bookshelf shelf)
    {
        BookSpace targetSpace = shelf.GetFirstEmptySpace();
        if (targetSpace == null)
        {
            Debug.LogWarning($"Kệ {shelf.Type} báo còn chỗ nhưng GetFirstEmptySpace() lại null.");
            return;
        }

        targetSpace.AssignBook(book);

        Vector3 targetPos = targetSpace.SpaceTransform.position;
        book.MoveToPosition(targetPos, () =>
        {
            book.transform.SetParent(targetSpace.SpaceTransform, worldPositionStays: true);
            allBooks.Remove(book);

            shelf.NotifySpaceAssigned();

            booksPlaced++;
            CheckWinCondition();

            RefreshAllBlockedStates();
        });
    }

    private void PlaceBookInTray(Book book)
    {
        if (isLevelOver) return;
        if (holdingTray == null)
        {
            Debug.LogWarning("GameManager chưa được gán HoldingTray, sách tạm thời đứng yên.");
            return;
        }

        BookSpace traySpace = holdingTray.GetFirstEmptySpace();
        if (traySpace == null)
        {
            HandleLevelLose();
            return;
        }

        traySpace.AssignBook(book);

        Vector3 targetPos = traySpace.SpaceTransform.position;
        book.MoveToPosition(targetPos, () =>
        {
            book.transform.SetParent(traySpace.SpaceTransform, worldPositionStays: true);
            allBooks.Remove(book);
            holdingTray.NotifySpaceAssigned();
            RefreshAllBlockedStates();
        });
    }

    private void HandleShelfSpawned(Bookshelf shelf)
    {
        if (isLevelOver) return;
        if (holdingTray == null) return;

        BookSpace waitingSpace = holdingTray.FindWaitingBook(shelf.Type);
        if (waitingSpace == null) return;

        Book waitingBook = waitingSpace.CurrentBook;
        waitingSpace.Clear();

        PlaceBookInShelf(waitingBook, shelf);
    }

    private void CheckWinCondition()
    {
        if (isLevelOver) return;

        if (totalBooksToPlace > 0 && booksPlaced >= totalBooksToPlace)
        {
            HandleLevelWin();
        }
    }

    private void HandleLevelWin()
    {
        if (isLevelOver) return;
        isLevelOver = true;

        if (GameplayStateController != null)
        {
            GameplayStateController.SetState(GameplayStateController.GameState.Win);
        }

        int currentLevelIndex = LevelLoader.Instance != null ? LevelLoader.Instance.GetCurrentLevel() : 0;
        if (LevelProgressManager.Instance != null)
        {
            LevelProgressManager.Instance.CompleteLevel(currentLevelIndex);
        }

        Debug.Log("LEVEL COMPLETE! Đã xếp xong toàn bộ sách.");
    }

    private void HandleLevelLose()
    {
        if (isLevelOver) return;
        isLevelOver = true;

        if (GameplayStateController != null)
        {
            GameplayStateController.SetState(GameplayStateController.GameState.Lose);
        }

        Debug.Log("GAME OVER! Hàng chờ đã đầy, không còn chỗ chứa sách chưa có kệ khớp.");
    }
}