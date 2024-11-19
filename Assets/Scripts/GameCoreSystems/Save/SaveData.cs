using UnityEngine;

public class SaveData : MonoBehaviour
{
    public static SaveData instance;

    [Header("Save Data")]
    public bool verticalMode;

    // GridData�� �߰�
    public GridData gridData;

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
            verticalMode = this.verticalMode,
            // GridData�� ����
            gridDataJson = JsonUtility.ToJson(gridData)
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

            // GridData ����
            gridData = JsonUtility.FromJson<GridData>(saveObject.gridDataJson);

            Debug.Log("Game Loaded!");
        }
    }

    [System.Serializable]
    private class SaveObject
    {
        public bool verticalMode;
        // GridData�� �����ϱ� ���� JSON ���ڿ� �ʵ�
        public string gridDataJson;
    }
}
