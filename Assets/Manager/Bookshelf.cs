using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;
using System.Linq;

public class Bookshelf : MonoBehaviour
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
