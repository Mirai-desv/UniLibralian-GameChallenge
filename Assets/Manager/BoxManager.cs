using UnityEngine;

public class BoxManager : MonoBehaviour
{
    [SerializeField] private BookshelfController bookshelfController;

    // Trả về BookSpace đích nếu book được phép vào box
    public bool TryGetTargetSpace(Book book, out BookSpace targetSpace)
    {
        targetSpace = null;

        // Tìm bookshelf cùng loại đang mở
        Bookshelf shelf = bookshelfController.FindMatchingShelf(book.Type);

        // Không có bookshelf cùng màu
        if (shelf == null)
            return false;

        // Lấy slot trống đầu tiên
        targetSpace = shelf.GetFirstEmptySpace();

        if (targetSpace == null)
            return false;

        return true;
    }

    // Gọi sau khi animation hoàn tất
    public void PlaceBook(Book book, BookSpace targetSpace)
    {
        targetSpace.AssignBook(book);

        // Báo bookshelf kiểm tra đầy
        Bookshelf shelf = targetSpace.GetComponentInParent<Bookshelf>();
        shelf.NotifySpaceAssigned();
    }
}