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
        Vector3 targetPos = targetPosition.position;

        book.MoveToPosition(targetPos, () => {OnBookArrived(book);});
    }

    private void OnBookArrived(Book book)
    {
        // Đến nơi rồi nên coi như xóa book khỏi map
        allBooks.Remove(book);

        // Cập nhật lại trạng thái bị che
        RefreshAllBlockedStates();

        // Đoạn của dev 2 nhá
    }
    
}
