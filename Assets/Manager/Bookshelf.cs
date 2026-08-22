using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using DG.Tweening;

[System.Serializable]
public class BookshelfVisualEntry
{
    public BookType Type;
    public Sprite Sprite;

    [Header("Custom Adjustments")]
    public Vector2 Offset = Vector2.zero;
    public Vector3 Scale = Vector3.one;
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
    // Prefab này PHẢI có SpriteRenderer, giống hệt cách bookPrefab / placedBookVisualPrefab
    // đang dùng trong GameManager.cs (SpawnPlacedBookVisual) -> không dùng UI Image.
    [SerializeField] private GameObject topVisualPrefab;
    [SerializeField] private BookshelfVisualEntry[] topVisualsByType;
    [SerializeField] private float topVisualOffsetY = 0.2f; // khoảng cách visual cách phía trên Bookshelf

    // Giữ tham chiếu instance hiện tại để tránh spawn chồng nhiều visual mỗi lần Initialize() được gọi lại
    private GameObject currentTopVisual;

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

    // TOP VISUAL — cùng pattern với SpawnPlacedBookVisual trong GameManager.cs:
    // 1 prefab (SpriteRenderer) + mảng sprite theo Type.
    private void SpawnTopVisual()
    {
        // Xóa visual cũ (nếu Initialize() được gọi lại)
        if (currentTopVisual != null)
        {
            Destroy(currentTopVisual);
            currentTopVisual = null;
        }

        if (topVisualPrefab == null)
        {
            Debug.LogWarning(
                $"Bookshelf [{Type}] chưa được gán Top Visual Prefab!"
            );
            return;
        }

        // Tìm entry (sprite + offset + scale riêng) tương ứng với BookType
        BookshelfVisualEntry entry = topVisualsByType?
            .FirstOrDefault(e => e.Type == Type);

        if (entry == null || entry.Sprite == null)
        {
            Debug.LogWarning(
                $"Bookshelf [{Type}] không có Top Visual Sprite tương ứng!"
            );
            return;
        }

        // Vị trí đỉnh Bookshelf trong world space, tính từ RectTransform nếu có
        // (Bookshelf dùng RectTransform), fallback SpriteRenderer nếu không có.
        Vector3 topAnchor = GetTopAnchorWorldPosition();

        Vector3 spawnPosition = topAnchor + new Vector3(
            entry.Offset.x,
            entry.Offset.y + topVisualOffsetY,
            0f
        );

        // Tạo visual như một world object (SpriteRenderer), y hệt cách
        // SpawnPlacedBookVisual đang tạo book visual đã xếp lên kệ.
        GameObject visual = Instantiate(
            topVisualPrefab,
            spawnPosition,
            Quaternion.identity,
            transform
        );

        currentTopVisual = visual;

        SpriteRenderer sr = visual.GetComponent<SpriteRenderer>();

        if (sr == null)
        {
            Debug.LogWarning(
                $"Top Visual Prefab '{topVisualPrefab.name}' cần có SpriteRenderer " +
                "(giống Book / Placed Book Visual prefab), hiện đang thiếu!"
            );
            return;
        }

        sr.sprite = entry.Sprite;
        visual.transform.localScale = entry.Scale;

        // Đặt vẽ trên cùng layer/order với Bookshelf, +1 để nằm phía trước
        SpriteRenderer shelfRenderer = GetComponentInChildren<SpriteRenderer>();
        if (shelfRenderer != null && shelfRenderer != sr)
        {
            sr.sortingLayerID = shelfRenderer.sortingLayerID;
            sr.sortingOrder = shelfRenderer.sortingOrder + 1;
        }
    }
    [Header("Animation")]
    [SerializeField] private float showDuration = 0.35f;
    [SerializeField] private float hideDuration = 0.25f;

    public void PlayShowAnimation()
    {
        transform.DOKill();

        transform.localScale = Vector3.zero;

        transform.DOScale(Vector3.one, showDuration)
            .SetEase(Ease.OutBack);
    }

    public void PlayHideAnimation(Action onComplete)
    {
        transform.DOKill();

        transform.DOScale(Vector3.zero, hideDuration)
            .SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                onComplete?.Invoke();
            });
    }

    // Lấy tọa độ world của điểm giữa-đỉnh Bookshelf, dùng RectTransform nếu có
    // (không phụ thuộc việc Bookshelf có SpriteRenderer hay không).
    private Vector3 GetTopAnchorWorldPosition()
    {
        RectTransform rect = GetComponent<RectTransform>();
        if (rect == null) rect = GetComponentInChildren<RectTransform>();

        if (rect != null)
        {
            Vector3[] corners = new Vector3[4]; // 0:BL 1:TL 2:TR 3:BR
            rect.GetWorldCorners(corners);
            return (corners[1] + corners[2]) / 2f;
        }

        SpriteRenderer renderer = GetComponentInChildren<SpriteRenderer>();
        if (renderer != null)
        {
            Bounds bounds = renderer.bounds;
            return new Vector3(bounds.center.x, bounds.max.y, transform.position.z);
        }

        return transform.position;
    }

    public BookSpace GetFirstEmptySpace()
    {
        return spaces.FirstOrDefault(s => !s.IsOccupied);
    }

    public void NotifySpaceAssigned()
    {
        if (IsFull)
        {
            OnShelfFilled?.Invoke(this);
        }
    }

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