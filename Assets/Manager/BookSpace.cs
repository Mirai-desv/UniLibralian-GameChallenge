using UnityEngine;

public class BookSpace : MonoBehaviour
{
    public bool IsOccupied {get; private set;}
    private Book currentBook;
    public Transform SpaceTransform => transform;
    public void AssignBook(Book book)
    {
        currentBook = book;
        IsOccupied = true;
    } 

    //Gọi khi cần thu hồi lại chỗ trống, cũng chỉ dùng ở giá hàng đợi thôi
    public void Clear()
    {
        currentBook = null;
        IsOccupied = false;
    }
}
