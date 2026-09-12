using UnityEngine;

public class LevelSpawner : MonoBehaviour
{
    public LevelData level;
    public BookDatabase database;
    void Start()
    {
        foreach (var data in level.BookLayout)
        {
            var prefab = database.GetPrefab(data.Type);
            if(prefab == null)
            {
                Debug.LogError($"Không tìm thấy prefab cho {data.Type}");
                continue;
            }
            var obj = Instantiate(prefab, data.Position, Quaternion.identity);
            var sr = obj.GetComponent<SpriteRenderer>();
            if(sr != null) sr.sortingOrder = data.SortingOrder;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
