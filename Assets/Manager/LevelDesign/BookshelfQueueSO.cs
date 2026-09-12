using UnityEngine;
using System.Collections.Generic;

// ScriptableObject rieng cho conveyor
// Tach khoi LevelData de co the thiet ke/tai su dung doc lap cho nhieu level
[CreateAssetMenu(fileName = "BookshelfQueueso", menuName = "Scriptable Objects/BookshelfQueueso", order = 1)]
public class BookshelfQueueso : ScriptableObject
{
    public List<BookshelfData> ShelfQueue = new List<BookshelfData>();
}
