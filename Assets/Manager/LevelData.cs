using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class BookSpawnData
{
    public BookType Type;
    public Vector2 Position;
    public int SortingOrder; //càng cao thì càng nằm ở trên
}
public class LevelData : ScriptableObject
{
    public List<BookshelfData> ShelfQueue;

    // Toàn bộ sách xuất hiện sẵn trên bàn chơi khi vào level
    public List<BookSpawnData> BookLayout = new List<BookSpawnData>();
}