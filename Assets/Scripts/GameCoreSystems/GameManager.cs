using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] private List<SaveDatas.PrefabData> seedPrefabs; // ���� ������ ����Ʈ
    [SerializeField] private List<SaveDatas.PrefabData> fieldPrefabs; // �� ������ ����Ʈ

    private Dictionary<int, GameObject> seedDictionary; // ���� ID-������ ����
    private Dictionary<int, GameObject> fieldDictionary; // �� ID-������ ����

    public GridData gridData;

    public Camera mainCam;
    public Text testText;

    private const string FirstLaunchKey = "IsFirstLaunch";
    private const string CoinKey = "Coin";
    private const string GemKey = "Gem";
    private const int DefaultCoin = 100;
    private const int DefaultGem = 0;

    public static int currentCoin = 0;
    public static int currentGem = 0;

    [SerializeField] private Transform playerTransform;

    [SerializeField] private FieldManager fieldManager;

    [SerializeField] private PlacementSystem placementSystem;

    [SerializeField] private float autoSaveInterval = 300f; // �ڵ� ���� ���� (��)
    private float autoSaveTimer; // �ڵ� ���� Ÿ�̸�

    [SerializeField] private List<Crop> crop = new List<Crop>();
    [SerializeField] public List<FarmField> field = new List<FarmField>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        if (gridData == null)
        {
            gridData = new GridData(); // �ʿ��� ��� ScriptableObject�� �� �ν��Ͻ� ����
        }

        // ���� Dictionary �ʱ�ȭ
        seedDictionary = new Dictionary<int, GameObject>();
        foreach (var seedData in seedPrefabs)
        {
            if (!seedDictionary.ContainsKey(seedData.id))
            {
                seedDictionary.Add(seedData.id, seedData.prefab);
                Debug.Log($"Seed ID: {seedData.id}, Prefab Name: {seedData.prefab.name}");
            }
            else
            {
                Debug.LogWarning($"�ߺ��� Seed ID: {seedData.id}. Ȯ�� �ʿ�!");
            }
        }

        // �� Dictionary �ʱ�ȭ
        fieldDictionary = new Dictionary<int, GameObject>();
        foreach (var fieldData in fieldPrefabs)
        {
            if (!fieldDictionary.ContainsKey(fieldData.id))
            {
                fieldDictionary.Add(fieldData.id, fieldData.prefab);
                Debug.Log($"Field ID: {fieldData.id}, Prefab Name: {fieldData.prefab.name}");
            }
            else
            {
                Debug.LogWarning($"�ߺ��� Field ID: {fieldData.id}. Ȯ�� �ʿ�!");
            }
        }

        SaveSystem.Init();
        InitializeSaveData();
    }

    public GameObject GetSeedPrefabById(int id)
    {
        if (seedDictionary.TryGetValue(id, out var prefab))
        {
            return prefab;
        }
        else
        {
            Debug.LogWarning($"Seed ID {id}�� �ش��ϴ� �������� �����ϴ�.");
            return null;
        }
    }

    public GameObject GetFieldPrefabById(int id)
    {
        if (fieldDictionary.TryGetValue(id, out var prefab))
        {
            return prefab;
        }
        else
        {
            Debug.LogWarning($"Field ID {id}�� �ش��ϴ� �������� �����ϴ�.");
            return null;
        }
    }

    void Start()
    {
        //InitializePlayerPrefs();    // �׽�Ʈ�Ҷ��� �ּ� Ǯ��, ���߿� ����

        // �ڵ� ���� Ÿ�̸� �ʱ�ȭ
        autoSaveTimer = autoSaveInterval;

        // ���� ������ �ʱ�ȭ
        crop.Clear();
        field.Clear();

        gridData = placementSystem.placedOBJData;

        if (PlayerPrefs.GetInt("TutorialDone", 0) == 0)
        {
            currentCoin = 150160;
        }

        // ���� ��ġ�� Crop�� Field �߰�
        Crop[] crops = FindObjectsOfType<Crop>();
        FarmField[] fields = FindObjectsOfType<FarmField>();

        foreach (var item in crops)
        {
            if (!crop.Contains(item))
            {
                crop.Add(item);
            }
        }

        foreach (var item in fields)
        {
            if (!field.Contains(item))
            {
                field.Add(item);
            }
        }
    }

    public void RemoveMissingCrops()
    {
        crop.RemoveAll(c => c == null);
    }

    public void RemoveMissingFields()
    {
        field.RemoveAll(f => f == null);
    }

    void Update()
    {
        // Ÿ�̸� ������Ʈ
        autoSaveTimer -= Time.deltaTime;

        if (autoSaveTimer <= 0)
        {
            SaveGameData();
            Debug.Log("�ڵ� ���� �Ϸ�!");
            autoSaveTimer = autoSaveInterval; // Ÿ�̸� ����
        }

        RemoveMissingFields();
        RemoveMissingCrops();

        testText.text = currentCoin.ToString("N0");

        float currentTime = Time.time;

        foreach (var crop in crop)
        {
            crop.CheckGrowth(currentTime); // ���� �ð� ����
        }
    }

    private void InitializeSaveData()
    {
        string saveString = SaveSystem.Load("GameData.json");
        if (string.IsNullOrEmpty(saveString))
        {
            currentCoin = DefaultCoin;
            currentGem = DefaultGem;
            SaveGameData();
        }
        else
        {
            LoadGameData();
        }
    }

    public void InitializePlayerPrefs()
    {
        int totalCrops = 61; // �۹��� ������ ID

        for (int i = 9; i <= totalCrops; i++)
        {
            string cropKey = "CropUnlocked_" + i; // �� �۹��� �ر� ���� Ű
            PlayerPrefs.DeleteKey(cropKey); // �ش� Ű �����Ͽ� �ʱ�ȭ
        }
        PlayerPrefs.SetInt("TutorialKeyboard", 0);
        PlayerPrefs.SetInt("KeyboardAllClear", 0);
        PlayerPrefs.SetInt("TutorialDone", 0);

        fieldManager.InitializeUnlockedFields();

        // ��ü �ر� �ε����� �ʱ�ȭ (Optional)
        PlayerPrefs.SetInt("UnlockPlant", 2); // UnlockPlant�� �ʱ�ȭ
        PlayerPrefs.Save(); // ������� ����
    }

    public void QuitGame()
    {
        // �÷��̾� �����۷����� �����ϰ� ���� ����
        PlayerPrefs.Save();

#if UNITY_EDITOR
        // �����Ϳ����� �÷��� ��带 �����մϴ�.
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // ����� ���ø����̼ǿ����� ������ �����մϴ�.
        Application.Quit();
#endif
    }

    // ���� �߰� �޼���
    public static void AddCoins(int amount)
    {
        if (amount < 0)
            return;
        currentCoin += amount;
        AchievementsDatabase.CoinProgress(currentCoin);
        PlayerPrefs.SetInt(CoinKey, currentCoin);
        PlayerPrefs.Save();
        //Debug.Log("�� ���� : " + coin);
    }

    // ���� ���� �޼���
    public static void SubtractCoins(int amount)
    {
        if (amount < 0)
            return;
        if (currentCoin - amount < 0)
        {
            Debug.Log("������ ������� �ʽ��ϴ�.");
            return;
        }
        else
        {
            currentCoin -= amount;
            AchievementsDatabase.CoinProgress(currentCoin);
            PlayerPrefs.SetInt(CoinKey, currentCoin);
            PlayerPrefs.Save();
        }
        //Debug.Log("�� ���� : " + coin);
    }

    //==========================================================================

    public void SaveGameData()
    {
        // ������ ��ü ����
        RemoveMissingCrops();
        RemoveMissingFields();

        Vector3 playerPosition = playerTransform.position;

        // �� ������ ����
        List<GridFieldSave> fieldSaves = new();
        foreach (var field in field)
        {
            Vector3 worldPosition = field.transform.position;

            Renderer renderer = field.GetComponent<Renderer>(); // Renderer ��������
            float alpha = renderer != null ? renderer.material.color.a : 1.0f; // ���� �� ��������

            var fieldData = new GridFieldSave
            {
                position = worldPosition, // Vector3�� �״�� ����
                id = field.ID,
                placementData = new PlacementData(
                    field.occupiedPositions,
                    field.ID,
                    field.PlacedObjectIndex,
                    field.cropState,
                    field.seedPlantedState,
                    alpha // ���� �� ����
                )
            };
            fieldSaves.Add(fieldData);

            Debug.Log($"�� ����: ID={field.ID}, ���� ��ġ={worldPosition}, ���� ��={alpha}");
        }

        // �۹� ������ ����
        List<GridCropSave> cropSaves = new();
        foreach (var crop in crop)
        {
            Vector3 worldPosition = crop.transform.position;

            Renderer renderer = crop.GetComponent<Renderer>(); // Renderer ��������
            float alpha = renderer != null ? renderer.material.color.a : 1.0f; // ���� �� ��������

            var cropData = new GridCropSave
            {
                position = worldPosition, // Vector3�� �״�� ����
                id = crop.ID,
                placementData = new PlacementData(
                    crop.occupiedPositions,
                    crop.ID,
                    crop.PlacedObjectIndex,
                    crop.cropState,
                    crop.seedPlantedState,
                    alpha // ���� �� ����
                ),
                currentStage = crop.currentStage,           // ���� �ܰ� ����
                cropState = crop.cropState,                 // �۹� ���� ����
                growthStartTime = crop.growthStartTime,      // ���� ���� �ð� ����
                growthTimes = crop.growthTimes
            };
            cropSaves.Add(cropData);

            Debug.Log($"�۹� ����: ID={crop.ID}, ���� ��ġ={worldPosition}, ���� ��={alpha}");
        }

        // ���� ��ü ����
        GridDataSave gridSaveData = new GridDataSave
        {
            crops = cropSaves,
            fields = fieldSaves
        };

        // ����� ������ ����
        var storage = FindObjectOfType<Storage>();
        List<CropStorage> storedCrops = storage != null ? storage.storedCropsByID : new List<CropStorage>();

        // AIStateManager���� ���� �� ��������
        var aiStateManager = FindObjectOfType<AIStateManager>();
        int currentWaterAmount = aiStateManager != null ? aiStateManager.currentWaterAmount : 0;

        // AllSaveData ��ü ����
        AllSaveData saveData = new AllSaveData
        {
            coin = currentCoin,
            playerPosition = playerPosition,
            gridDataJson = JsonUtility.ToJson(gridSaveData),
            storedCropsByID = storedCrops, // ����� ������ �߰�
            currentWaterAmount = currentWaterAmount // ���� �� ����
        };

        // JSON ����ȭ �� ����
        string json = JsonUtility.ToJson(saveData);
        SaveSystem.Save(json, "GameData.json");
    }

    public void LoadGameData()
    {
        string saveString = SaveSystem.Load("GameData.json");

        if (!string.IsNullOrEmpty(saveString))
        {
            AllSaveData saveData = JsonUtility.FromJson<AllSaveData>(saveString);

            // �⺻ ������ ����
            currentCoin = saveData.coin;
            playerTransform.position = saveData.playerPosition;

            // GridData ������ȭ
            GridDataSave gridSaveData = JsonUtility.FromJson<GridDataSave>(saveData.gridDataJson);

            // �� ������ �ε�
            foreach (var fieldSave in gridSaveData.fields)
            {
                GameObject prefab = GetFieldPrefabById(fieldSave.id);
                if (prefab != null)
                {
                    GameObject newField = Instantiate(prefab, fieldSave.position, Quaternion.identity);
                    FarmField farmField = newField.GetComponent<FarmField>();
                    if (farmField != null)
                    {
                        farmField.LoadPlacementData(fieldSave.placementData);
                        farmField.ID = fieldSave.id;

                        // �θ��� �ڽ� SpriteRenderer�鿡 ���� �� ����
                        SpriteRenderer[] childRenderers = newField.GetComponentsInChildren<SpriteRenderer>();
                        foreach (var spriteRenderer in childRenderers)
                        {
                            Color color = spriteRenderer.color;
                            color.a = fieldSave.placementData.alpha; // ����� ���� �� ����
                            spriteRenderer.color = color;
                        }

                        field.Add(farmField);
                    }
                }
            }

            // �۹� ������ �ε�
            foreach (var cropSave in gridSaveData.crops)
            {
                GameObject prefab = GetSeedPrefabById(cropSave.id);
                if (prefab != null)
                {
                    GameObject newCrop = Instantiate(prefab, cropSave.position, Quaternion.identity);
                    Crop cropInstance = newCrop.GetComponent<Crop>();
                    if (cropInstance != null)
                    {
                        cropInstance.LoadPlacementData(cropSave.placementData);
                        cropInstance.ID = cropSave.id;
                        cropInstance.currentStage = cropSave.currentStage; // ���� �ܰ� ����
                        cropInstance.cropState = cropSave.cropState; // ���� ����
                        cropInstance.growthStartTime = cropSave.growthStartTime;
                        cropInstance.growthTimes = cropSave.growthTimes;

                        cropInstance.UpdateCropVisual(); // �ð��� ���� ������Ʈ
                        cropInstance.UpdateSortingLayer(); // ���� ���̾� ����

                        // �θ��� �ڽ� SpriteRenderer�鿡 ���� �� ����
                        SpriteRenderer[] childRenderers = newCrop.GetComponentsInChildren<SpriteRenderer>();
                        foreach (var spriteRenderer in childRenderers)
                        {
                            Color color = spriteRenderer.color;
                            color.a = cropSave.placementData.alpha; // ����� ���� �� ����
                            spriteRenderer.color = color;
                        }

                        crop.Add(cropInstance);
                    }
                }
            }

            // ����� ������ �ε�
            var storage = FindObjectOfType<Storage>();
            if (storage != null)
            {
                storage.storedCropsByID = saveData.storedCropsByID;
                Debug.Log("����� �����Ͱ� ���������� �ε�Ǿ����ϴ�.");
            }

            var aiStateManager = FindObjectOfType<AIStateManager>();
            if (aiStateManager != null)
            {
                aiStateManager.currentWaterAmount = saveData.currentWaterAmount;
            }
        }
    }

    public void AddSeed(Crop newCrop)
    {
        crop.Add(newCrop);
    }

    public void Addfield(FarmField newFarmField)
    {
        field.Add(newFarmField);
    }
}

[System.Serializable]
public class AllSaveData
{
    public int coin;
    public Vector3 playerPosition;
    public string gridDataJson;
    public List<Crop> crops;
    public List<FarmField> fields;
    public List<CropStorage> storedCropsByID;
    public int currentWaterAmount;
}