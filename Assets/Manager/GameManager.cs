using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    //public GameObject BookPrefab;
    //public GameObject BookshelfPrefab;
    
    public Transform targetPosition;

    private List<Bookshelf> Bookshelfs = new List<Bookshelf>();
    private List<Book> allBooks = new List<Book>();
    [SerializeField] private BoxManager boxManager;
    [SerializeField] private GameStateController gameStateController;

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
        // Xem lai ve vai tro cua dong duoi
        List<List<Color>> BookData = new List<List<Color>>();

        int EmptyBookshelfs = 2;

        //Them bookshelf rong
        for(int i = 0; i < EmptyBookshelfs; i++)
        {
            BookData.Add(new List<Color>());
        }
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
        if (gameStateController.CurrentState != GameStateController.GameState.Playing)
            return;

        if (!boxManager.TryGetTargetSpace(book, out BookSpace targetSpace))
        {
            Debug.Log("Sai màu hoặc không có bookshelf phù hợp");

            gameStateController.CheckGameState(allBooks, boxManager);
            return;
        }

        book.MoveToPosition(targetSpace.SpaceTransform.position, () =>
        {
            boxManager.PlaceBook(book, targetSpace);
            OnBookArrived(book);
        });
    }

    private void OnBookArrived(Book book)
    {
        // Đến nơi rồi nên coi như xóa book khỏi map
        allBooks.Remove(book);

        // Cập nhật lại trạng thái bị che
        RefreshAllBlockedStates();

        // Đoạn của dev 2 nhá
        // Đây đoạn của dev 2 đây
        gameStateController.CheckGameState(allBooks, boxManager);

    }
    
}
