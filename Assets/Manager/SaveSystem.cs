using UnityEngine;

public static class SaveSystem
{
    private const string KEY = "LevelProgress";

    public static void Save(LevelProgressData data)
    {
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(KEY, json);
        PlayerPrefs.Save();
    }

    public static LevelProgressData Load()
    {
        if (!PlayerPrefs.HasKey(KEY))
        {
            LevelProgressData data = new LevelProgressData();
            Save(data);
            return data;
        }

        string json = PlayerPrefs.GetString(KEY);
        return JsonUtility.FromJson<LevelProgressData>(json);
    }
}