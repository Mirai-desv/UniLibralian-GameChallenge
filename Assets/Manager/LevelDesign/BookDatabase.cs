using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "BookDatabase", menuName = "LevelDesign/BookDatabase")]
public class BookDatabase : ScriptableObject
{
    [System.Serializable]
    public class Entry
    {
        public BookType Type;
        public GameObject Prefab;
    }
    public List<Entry> Entries;
    public GameObject GetPrefab(BookType type) =>
        Entries.Find(e => e.Type == type)?.Prefab;
}
