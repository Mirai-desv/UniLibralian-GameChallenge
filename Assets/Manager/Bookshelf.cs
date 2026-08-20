using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

[System.Serializable]
public class BookshelfVisualEntry
{
    public BookType Type;
    public Sprite Sprite;
}

public class Bookshelf : MonoBehaviour
{
    [Header("Data")]
    public BookType Type { get; private set; }

    [Header("Layout Setup")]
    [SerializeField] private BookSpace bookSpacePrefab;
    [SerializeField] private float spaceWidth = 0.3f;
    [SerializeField] private Vector3 startOffset = Vector3.zero;

    [Header("Top Visual")]
    [SerializeField] private GameObject topVisualPrefab;
    [SerializeField] private BookshelfVisualEntry[] topVisualsByType;

    private readonly List<BookSpace> spaces = new List<BookSpace>();

    // Bắn ra sự kiện khi BookSpace cuối cùng vừa được lấp đầy
    public event Action<Bookshelf> OnShelfFilled;

    // Kiểm tra xem tất cả BookSpace đã được lấp đầy chưa
    public bool IsFull => spaces.Count > 0 && spaces.All(s => s.IsOccupied);

    public void Initialize(BookType type, int count)
    {
        Type = type;

        ClearSpaces();

        // Tạo visual phía trên Bookshelf
        SpawnTopVisual();

        // Tính toán khoảng cách để căn giữa chuỗi sách
        float totalWidth = spaceWidth * count;
        float startX = -totalWidth / 2f + spaceWidth / 2f;

        for (int i = 0; i < count; i++)
        {
            BookSpace space = Instantiate(bookSpacePrefab, transform);

            Vector3 localPos = startOffset +
                new Vector3(startX + (i * spaceWidth), 0f, 0f);

            space.transform.localPosition = localPos;

            spaces.Add(space);
        }
    }

    // =========================================================
    // TOP VISUAL
    // =========================================================

    private void SpawnTopVisual()
    {
        if (topVisualPrefab == null)
        {
            Debug.LogWarning(
                $"Bookshelf [{Type}] chưa được gán Top Visual Prefab!"
            );
            return;
        }
        // Tìm sprite tương ứng với BookType
        Sprite matchedSprite = topVisualsByType?
            .FirstOrDefault(e => e.Type == Type)?.Sprite;

        if (matchedSprite == null)
        {
            Debug.LogWarning(
                $"Bookshelf [{Type}] không có Top Visual Sprite tương ứng!"
            );
            return;
        }
        SpriteRenderer bookshelfRenderer = GetComponent<SpriteRenderer>();
        // Nếu SpriteRenderer nằm trong child của Bookshelf
        if (bookshelfRenderer == null)
        {
            bookshelfRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        if (bookshelfRenderer == null)
        {
            Debug.LogWarning(
                $"Bookshelf [{Type}] không tìm thấy SpriteRenderer!"
            );
            return;
        }

        // Lấy điểm cao nhất của sprite Bookshelf
        Bounds bounds = bookshelfRenderer.bounds;

        // Khoảng cách visual cách phía trên Bookshelf
        float offsetY = 0.2f;

        Vector3 spawnPosition = new Vector3(
            bounds.center.x,
            bounds.max.y + offsetY,
            transform.position.z
        );

        // Tạo visual
        GameObject visual = Instantiate(
            topVisualPrefab,
            spawnPosition,
            Quaternion.identity
        );

        // Gán sprite
        SpriteRenderer visualRenderer = visual.GetComponent<SpriteRenderer>();

        if (visualRenderer != null)
        {
            visualRenderer.sprite = matchedSprite;

            // Đảm bảo visual nằm phía trước Bookshelf
            visualRenderer.sortingLayerID =
                bookshelfRenderer.sortingLayerID;

            visualRenderer.sortingOrder =
                bookshelfRenderer.sortingOrder + 1;
        }
        else
        {
            Debug.LogWarning(
                $"Top Visual Prefab '{topVisualPrefab.name}' không có SpriteRenderer!"
            );
        }
        visual.transform.SetParent(transform, true);
    }

    // =========================================================
    // BOOK SPACE
    // =========================================================

    // Tìm vị trí trống đầu tiên để thả sách vào
    public BookSpace GetFirstEmptySpace()
    {
        return spaces.FirstOrDefault(s => !s.IsOccupied);
    }

    // Gọi mỗi khi một cuốn sách được xếp vào kệ
    public void NotifySpaceAssigned()
    {
        if (IsFull)
        {
            OnShelfFilled?.Invoke(this);
        }
    }

    // Xóa toàn bộ BookSpace cũ
    private void ClearSpaces()
    {
        foreach (var space in spaces)
        {
            if (space != null)
            {
                Destroy(space.gameObject);
            }
        }

        spaces.Clear();
    }

    // Giải phóng sách khỏi Bookshelf
    public void ReleaseBooks()
    {
        foreach (var space in spaces)
        {
            if (space != null && space.CurrentBook != null)
            {
                space.CurrentBook.transform.SetParent(
                    null,
                    worldPositionStays: true
                );
            }
        }
    }
}