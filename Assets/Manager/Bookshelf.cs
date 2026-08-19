using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;
using System.Linq;

public class Bookshelf : MonoBehaviour
{
    [Header("Data")]
    public BookType Type { get; private set; }

    [Header("Layout Setup")]
    [SerializeField] private BookSpace bookSpacePrefab; 
    [SerializeField] private float spaceWidth = 0.3f;    // Độ dày / khoảng cách giữa các cuốn sách
    [SerializeField] private Vector3 startOffset = Vector3.zero; // Tùy chỉnh điểm đặt sách đầu tiên so với tâm Kệ

    private readonly List<BookSpace> spaces = new List<BookSpace>();

    // Bắn ra sự kiện khi BookSpace cuối cùng vừa được lấp đầy (thông báo win level)
    public event Action<Bookshelf> OnShelfFilled;

    // Kiểm tra xem tất cả các vị trí BookSpace đã có sách xếp vào chưa
    public bool IsFull => spaces.Count > 0 && spaces.All(s => s.IsOccupied);

    public void Initialize(BookType type, int count)
    {
        Type = type;

        ClearSpaces();

        // Tính toán khoảng cách để căn giữa chuỗi sách trên Asset Kệ
        float totalWidth = spaceWidth * count;
        float startX = -totalWidth / 2f + spaceWidth / 2f;

        for (int i = 0; i < count; i++)
        {
            BookSpace space = Instantiate(bookSpacePrefab, transform);
            
            // Định vị các BookSpace ẩn đóng vai trò làm điểm đặt sách
            Vector3 localPos = startOffset + new Vector3(startX + (i * spaceWidth), 0f, 0f);
            space.transform.localPosition = localPos;
            
            spaces.Add(space);
        }
    }

    // Tìm vị trí trống đầu tiên để thả sách vào
    public BookSpace GetFirstEmptySpace()
    {
        return spaces.FirstOrDefault(s => !s.IsOccupied);
    }

    // Gọi hàm này mỗi khi có một cuốn sách mới được xếp vào kệ
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
            if (space != null) Destroy(space.gameObject);
        }
        spaces.Clear();
    }
}
/*
{
    [Header ("Data")]
    public BookType Type {get; private set; }
    [Header ("Layout Setup")]
    [SerializeField] private BookSpace bookSpacePrefab;
    [SerializeField] private GameObject leftCapPrefab;
    [SerializeField] private GameObject rightCapPrefab;
    [SerializeField] private float spaceWidth = 0.3f; // Độ dày của sách
    [SerializeField] private float capWidth = 0.15f; // Độ dày của chốt chặn
    private readonly List<BookSpace> spaces = new List<BookSpace>();

    // Bắn ra khi BookSpace cuối cùng vừa đc lấp đầy (thông báo win level)
    public event Action<Bookshelf> OnShelfFilled;
    // Hàm kiểm tra xem có đủ 2 Object chặn trái & chặn phải không
    public bool IsFull => spaces.Count > 0 && spaces.All(s => s.IsOccupied);
    public void Initialize(BookType type, int count)
    {
        Type = type;
        bool hasCaps = leftCapPrefab != null && rightCapPrefab != null;
        float capsTotalWidth = hasCaps ? capWidth * 2f : 0f;
        float totalWidth = spaceWidth * count + capsTotalWidth;
        float cursor = -totalWidth / 2f;

        if(hasCaps)
        {
            SpawnAt(leftCapPrefab, cursor + capWidth / 2f);
            cursor += capWidth;
        }
        
        for(int i = 0; i < count; i++)
        {
            BookSpace space = Instantiate(bookSpacePrefab, transform);
            space.transform.localPosition = new Vector3(cursor + spaceWidth / 2f, 0f, 0f);
            spaces.Add(space);
            cursor += spaceWidth;
        }

        if(hasCaps)
        {
            SpawnAt(rightCapPrefab, cursor + capWidth / 2f);
        }
    }
    public void SpawnAt(GameObject prefab, float localX)
    {
        GameObject obj = Instantiate(prefab, transform);
        obj.transform.localPosition = new Vector3(localX, 0f, 0f);
    }

    // Hàm này tìm vị trí phù hợp nhất và trả về nguyên cái BookSpace phù hợp với yêu cầu
    public BookSpace GetFirstEmptySpace()
    {
        return spaces.FirstOrDefault(s => !s.IsOccupied);
    }

    public void NotifySpaceAssigned()
    {
        if(IsFull)
        {
            OnShelfFilled?.Invoke(this);
        }
    }
}
*/
