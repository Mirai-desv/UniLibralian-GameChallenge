using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

// Hàng chờ (holding tray): nơi giữ tạm những quyển sách bị chọn nhưng chưa có bookshelf match với nó
public class HoldingTray : MonoBehaviour
{
    [Header("Layout Setup")]
    [SerializeField] private BookSpace bookSpacePrefab;
    [SerializeField] private GameObject leftCapPrefab;
    [SerializeField] private GameObject rightCapPrefab;
    [SerializeField] private int capacity = 7;
    [SerializeField] private float spaceWidth = 0.3f;
    [SerializeField] private float capWidth = 0.15f;

    private readonly List<BookSpace> spaces = new List<BookSpace>();

    // Bắn ra khi hàng chờ bị lấp đầy hoàn toàn -> điều kiện thua level
    // sẽ được GameManager nghe và thực hiện, nma hiện chưa làm phần đấy
    public event Action OnTrayFull;

    public bool IsFull => spaces.Count > 0 && spaces.All(s => s.IsOccupied);
    public bool HasBooks => spaces.Any(s => s.IsOccupied);

    private void Awake()
    {
        BuildLayout();
    }

    // Dựng hàng ô trống, spacing tương tự Bookshelf.Initialize() để đồng bộ hình ảnh
    private void BuildLayout()
    {
        bool hasCaps = leftCapPrefab != null && rightCapPrefab != null;
        float capsTotalWidth = hasCaps ? capWidth * 2f : 0f;
        float totalWidth = spaceWidth * capacity + capsTotalWidth;
        float cursor = -totalWidth / 2f;

        if (hasCaps)
        {
            SpawnAt(leftCapPrefab, cursor + capWidth / 2f);
            cursor += capWidth;
        }

        for (int i = 0; i < capacity; i++)
        {
            BookSpace space = Instantiate(bookSpacePrefab, transform);
            space.transform.localPosition = new Vector3(cursor + spaceWidth / 2f, 0f, 0f);
            spaces.Add(space);
            cursor += spaceWidth;
        }

        if (hasCaps)
        {
            SpawnAt(rightCapPrefab, cursor + capWidth / 2f);
        }
    }

    private void SpawnAt(GameObject prefab, float localX)
    {
        GameObject obj = Instantiate(prefab, transform);
        obj.transform.localPosition = new Vector3(localX, 0f, 0f);
    }

    public BookSpace GetFirstEmptySpace()
    {
        return spaces.FirstOrDefault(s => !s.IsOccupied);
    }

    // Tìm ô đang giữ 1 quyển sách đúng Type (dùng khi vừa có Bookshelf mới khớp Type xuất hiện)
    public BookSpace FindWaitingBook(BookType type)
    {
        return spaces.FirstOrDefault(s => s.IsOccupied && s.CurrentBook != null && s.CurrentBook.Type == type);
    }

    public void NotifySpaceAssigned()
    {
        if (IsFull)
        {
            OnTrayFull?.Invoke();
        }
    }
    public int GetIndexOfSpace(BookSpace space)
    {
        if (space == null)
            return -1;

        BookSpace[] spaces = GetComponentsInChildren<BookSpace>();

        return System.Array.IndexOf(spaces, space);
    }

}