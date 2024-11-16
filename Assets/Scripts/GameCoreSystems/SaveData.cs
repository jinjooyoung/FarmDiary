using UnityEngine;

public class SaveData : MonoBehaviour
{
    public static SaveData instance;

    [Header("Save Data")]
    public bool verticalMode;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveGame()
    {
        SaveObject saveObject = new SaveObject
        {
            verticalMode = this.verticalMode
        };

        string json = JsonUtility.ToJson(saveObject);
        SaveSystem.Save(json, "GameSave");
        Debug.Log("Game Saved!");
    }

    public void LoadGame()
    {
        string saveString = SaveSystem.Load("GameSave");
        if (saveString != null)
        {
            SaveObject saveObject = JsonUtility.FromJson<SaveObject>(saveString);
            this.verticalMode = saveObject.verticalMode;

            Debug.Log("Game Loaded!");
        }
    }

    [System.Serializable]
    private class SaveObject
    {
        public bool verticalMode;
    }
}