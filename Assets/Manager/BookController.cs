using UnityEngine;
using System;
using System.Collections;

public class BookController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 100f;
    private Book book;
    void Awake()
    {
        book = GetComponent<Book>();
    }
    public bool IsBlocked {get; private set;}

    // Theo nguyên lý OOP để tránh lỗi truy cập
    public void UpdateBlockedState()
    {
        bool blocked = CheckIfBlocked();
        book.SetBlockedState(blocked);
        // Sẽ thêm animation sáng tối để ng chơi biết cái nào đang bị đè
    }

    // Check xem có quyển sách nào đang đè không
    private bool CheckIfBlocked()
    {
        Vector2 center = book.BoxCollider.bounds.center;
        Vector2 size = book.BoxCollider.bounds.size * 0.9f; // thu nhỏ nhẹ để tránh false positive do 2 book chạm cạnh

        Collider2D[] overlaps = Physics2D.OverlapBoxAll(center, size, 0f);

        foreach (var col in overlaps)
        {
            if (col.gameObject == gameObject) continue;

            Book otherBook = col.GetComponent<Book>();
            if (otherBook == null) continue;

            if (otherBook.SpriteRenderer.sortingOrder > book.SpriteRenderer.sortingOrder)
            {
                return true;
            }
        }
        return false;
    }

    public void OnBookClicked()
    {
        if (book.IsBlocked || book.IsMoving)
        {
            // Sẽ thêm animation kiểu rung lắc nhẹ hoặc gì đấy để thông báo là nó đang ko sài đc
            return;
        }
        GameManager.instance.HandleBookSelected(book);
    }

    public void RequestMove(Vector3 targetPosition, Action onComplete)
    {
        StartCoroutine(MoveToPosition(targetPosition, onComplete));
    }

    private IEnumerator MoveToPosition(Vector3 targetPosition, Action onComplete)
    {
        book.IsMoving = true;

        // Đưa sortingOrder lên cao nhất tạm thời, tránh bị các book khác che đi khi đang bay qua
        int originalSortingOrder = book.SpriteRenderer.sortingOrder;
        book.SpriteRenderer.sortingOrder = 1000;

        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3. MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPosition;
        book.SpriteRenderer.sortingOrder = originalSortingOrder;

        book.IsMoving = false;
        onComplete?.Invoke();
    }
}
