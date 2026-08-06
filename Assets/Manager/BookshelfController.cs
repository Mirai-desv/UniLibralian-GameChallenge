using UnityEngine;
using System;
using System.Collections.Generic;
public class BookshelfController : MonoBehaviour
{
    [Header ("Level Data")]
    [SerializeField] private LevelData currentLevel;
    [Header ("Shelf Setup")]
    [SerializeField] private Bookshelf bookshelfPrefab;
    //[SerializeField] private List<BookshelfData> shelfQueueData;
    [SerializeField] private Transform[] slotPositions = new Transform[2];
    
    private Queue<BookshelfData> shelfQueue;
    private readonly Bookshelf[] activeShelves = new Bookshelf[2];

    private void Awake()
    {
        shelfQueue = new Queue<BookshelfData>(currentLevel.ShelfQueue);
    }

    private void Start()
    {
        for(int i = 0; i < slotPositions.Length; i++)
        {
            SpawnNextShelf(i);
        }
    }

    private void SpawnNextShelf(int slotIndex)
    {
        if(shelfQueue.Count == 0)
        {
            Debug.Log("BookshelfController: hết bookshelf trong hàng đợi.");
            return;
        }
        BookshelfData data = shelfQueue.Dequeue();

        Bookshelf newShelf = Instantiate(bookshelfPrefab, slotPositions[slotIndex].position, Quaternion.identity);
        newShelf.Initialize(data.Type, data.SpaceCount);
        newShelf.OnShelfFilled += HandleShelfFilled;
        activeShelves[slotIndex] = newShelf;
    }

    private void HandleShelfFilled(Bookshelf shelf)
    {
        int slotIndex = Array.IndexOf(activeShelves, shelf);
        if(slotIndex < 0) return;
        shelf.OnShelfFilled -= HandleShelfFilled;
        Destroy(shelf.gameObject);
        activeShelves[slotIndex] = null;
        SpawnNextShelf(slotIndex);
    }

    public Bookshelf FindMatchingShelf(BookType type)
    {
        foreach(var shelf in activeShelves)
        {
            if(shelf != null && shelf.Type == type && !shelf.IsFull)
            {
                return shelf;
            }
        }
        return null;
    }
}
