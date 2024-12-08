using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] private List<SaveDatas.PrefabData> seedPrefabs; // 씨앗 프리팹 리스트
    [SerializeField] private List<SaveDatas.PrefabData> fieldPrefabs; // 밭 프리팹 리스트

    private Dictionary<int, GameObject> seedDictionary; // 씨앗 ID-프리팹 매핑
    private Dictionary<int, GameObject> fieldDictionary; // 밭 ID-프리팹 매핑

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

    [SerializeField] private float autoSaveInterval = 300f; // 자동 저장 간격 (초)
    private float autoSaveTimer; // 자동 저장 타이머

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
            gridData = new GridData(); // 필요한 경우 ScriptableObject나 새 인스턴스 생성
        }

        // 씨앗 Dictionary 초기화
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
                Debug.LogWarning($"중복된 Seed ID: {seedData.id}. 확인 필요!");
            }
        }

        // 밭 Dictionary 초기화
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
                Debug.LogWarning($"중복된 Field ID: {fieldData.id}. 확인 필요!");
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
            Debug.LogWarning($"Seed ID {id}에 해당하는 프리팹이 없습니다.");
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
            Debug.LogWarning($"Field ID {id}에 해당하는 프리팹이 없습니다.");
            return null;
        }
    }

    void Start()
    {
        //InitializePlayerPrefs();    // 테스트할때만 주석 풀기, 나중에 삭제

        // 자동 저장 타이머 초기화
        autoSaveTimer = autoSaveInterval;

        // 기존 데이터 초기화
        crop.Clear();
        field.Clear();

        gridData = placementSystem.placedOBJData;

        if (PlayerPrefs.GetInt("TutorialDone", 0) == 0)
        {
            currentCoin = 150160;
        }

        // 씬에 배치된 Crop과 Field 추가
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
        // 타이머 업데이트
        autoSaveTimer -= Time.deltaTime;

        if (autoSaveTimer <= 0)
        {
            SaveGameData();
            Debug.Log("자동 저장 완료!");
            autoSaveTimer = autoSaveInterval; // 타이머 리셋
        }

        RemoveMissingFields();
        RemoveMissingCrops();

        testText.text = currentCoin.ToString("N0");

        float currentTime = Time.time;

        foreach (var crop in crop)
        {
            crop.CheckGrowth(currentTime); // 현재 시간 전달
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
        int totalCrops = 61; // 작물의 마지막 ID

        for (int i = 9; i <= totalCrops; i++)
        {
            string cropKey = "CropUnlocked_" + i; // 각 작물의 해금 상태 키
            PlayerPrefs.DeleteKey(cropKey); // 해당 키 삭제하여 초기화
        }
        PlayerPrefs.SetInt("TutorialKeyboard", 0);
        PlayerPrefs.SetInt("KeyboardAllClear", 0);
        PlayerPrefs.SetInt("TutorialDone", 0);

        fieldManager.InitializeUnlockedFields();

        // 전체 해금 인덱스도 초기화 (Optional)
        PlayerPrefs.SetInt("UnlockPlant", 2); // UnlockPlant도 초기화
        PlayerPrefs.Save(); // 변경사항 저장
    }

    public void QuitGame()
    {
        // 플레이어 프리퍼런스를 저장하고 게임 종료
        PlayerPrefs.Save();

#if UNITY_EDITOR
        // 에디터에서는 플레이 모드를 중지합니다.
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // 빌드된 애플리케이션에서는 게임을 종료합니다.
        Application.Quit();
#endif
    }

    // 코인 추가 메서드
    public static void AddCoins(int amount)
    {
        if (amount < 0)
            return;
        currentCoin += amount;
        AchievementsDatabase.CoinProgress(currentCoin);
        PlayerPrefs.SetInt(CoinKey, currentCoin);
        PlayerPrefs.Save();
        //Debug.Log("총 코인 : " + coin);
    }

    // 코인 차감 메서드
    public static void SubtractCoins(int amount)
    {
        if (amount < 0)
            return;
        if (currentCoin - amount < 0)
        {
            Debug.Log("코인이 충분하지 않습니다.");
            return;
        }
        else
        {
            currentCoin -= amount;
            AchievementsDatabase.CoinProgress(currentCoin);
            PlayerPrefs.SetInt(CoinKey, currentCoin);
            PlayerPrefs.Save();
        }
        //Debug.Log("총 코인 : " + coin);
    }

    //==========================================================================

    public void SaveGameData()
    {
        // 누락된 객체 제거
        RemoveMissingCrops();
        RemoveMissingFields();

        Vector3 playerPosition = playerTransform.position;

        // 밭 데이터 수집
        List<GridFieldSave> fieldSaves = new();
        foreach (var field in field)
        {
            Vector3 worldPosition = field.transform.position;

            Renderer renderer = field.GetComponent<Renderer>(); // Renderer 가져오기
            float alpha = renderer != null ? renderer.material.color.a : 1.0f; // 알파 값 가져오기

            var fieldData = new GridFieldSave
            {
                position = worldPosition, // Vector3를 그대로 저장
                id = field.ID,
                placementData = new PlacementData(
                    field.occupiedPositions,
                    field.ID,
                    field.PlacedObjectIndex,
                    field.cropState,
                    field.seedPlantedState,
                    alpha // 알파 값 저장
                )
            };
            fieldSaves.Add(fieldData);

            Debug.Log($"밭 저장: ID={field.ID}, 월드 위치={worldPosition}, 알파 값={alpha}");
        }

        // 작물 데이터 수집
        List<GridCropSave> cropSaves = new();
        foreach (var crop in crop)
        {
            Vector3 worldPosition = crop.transform.position;

            Renderer renderer = crop.GetComponent<Renderer>(); // Renderer 가져오기
            float alpha = renderer != null ? renderer.material.color.a : 1.0f; // 알파 값 가져오기

            var cropData = new GridCropSave
            {
                position = worldPosition, // Vector3를 그대로 저장
                id = crop.ID,
                placementData = new PlacementData(
                    crop.occupiedPositions,
                    crop.ID,
                    crop.PlacedObjectIndex,
                    crop.cropState,
                    crop.seedPlantedState,
                    alpha // 알파 값 저장
                ),
                currentStage = crop.currentStage,           // 성장 단계 저장
                cropState = crop.cropState,                 // 작물 상태 저장
                growthStartTime = crop.growthStartTime,      // 성장 시작 시간 저장
                growthTimes = crop.growthTimes
            };
            cropSaves.Add(cropData);

            Debug.Log($"작물 저장: ID={crop.ID}, 월드 위치={worldPosition}, 알파 값={alpha}");
        }

        // 저장 객체 생성
        GridDataSave gridSaveData = new GridDataSave
        {
            crops = cropSaves,
            fields = fieldSaves
        };

        // 저장소 데이터 수집
        var storage = FindObjectOfType<Storage>();
        List<CropStorage> storedCrops = storage != null ? storage.storedCropsByID : new List<CropStorage>();

        // AIStateManager에서 물의 양 가져오기
        var aiStateManager = FindObjectOfType<AIStateManager>();
        int currentWaterAmount = aiStateManager != null ? aiStateManager.currentWaterAmount : 0;

        // AllSaveData 객체 생성
        AllSaveData saveData = new AllSaveData
        {
            coin = currentCoin,
            playerPosition = playerPosition,
            gridDataJson = JsonUtility.ToJson(gridSaveData),
            storedCropsByID = storedCrops, // 저장소 데이터 추가
            currentWaterAmount = currentWaterAmount // 물의 양 저장
        };

        // JSON 직렬화 및 저장
        string json = JsonUtility.ToJson(saveData);
        SaveSystem.Save(json, "GameData.json");
    }

    public void LoadGameData()
    {
        string saveString = SaveSystem.Load("GameData.json");

        if (!string.IsNullOrEmpty(saveString))
        {
            AllSaveData saveData = JsonUtility.FromJson<AllSaveData>(saveString);

            // 기본 데이터 적용
            currentCoin = saveData.coin;
            playerTransform.position = saveData.playerPosition;

            // GridData 역직렬화
            GridDataSave gridSaveData = JsonUtility.FromJson<GridDataSave>(saveData.gridDataJson);

            // 밭 데이터 로드
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

                        // 부모의 자식 SpriteRenderer들에 알파 값 복원
                        SpriteRenderer[] childRenderers = newField.GetComponentsInChildren<SpriteRenderer>();
                        foreach (var spriteRenderer in childRenderers)
                        {
                            Color color = spriteRenderer.color;
                            color.a = fieldSave.placementData.alpha; // 저장된 알파 값 적용
                            spriteRenderer.color = color;
                        }

                        field.Add(farmField);
                    }
                }
            }

            // 작물 데이터 로드
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
                        cropInstance.currentStage = cropSave.currentStage; // 성장 단계 복원
                        cropInstance.cropState = cropSave.cropState; // 상태 복원
                        cropInstance.growthStartTime = cropSave.growthStartTime;
                        cropInstance.growthTimes = cropSave.growthTimes;

                        cropInstance.UpdateCropVisual(); // 시각적 상태 업데이트
                        cropInstance.UpdateSortingLayer(); // 소팅 레이어 복원

                        // 부모의 자식 SpriteRenderer들에 알파 값 복원
                        SpriteRenderer[] childRenderers = newCrop.GetComponentsInChildren<SpriteRenderer>();
                        foreach (var spriteRenderer in childRenderers)
                        {
                            Color color = spriteRenderer.color;
                            color.a = cropSave.placementData.alpha; // 저장된 알파 값 적용
                            spriteRenderer.color = color;
                        }

                        crop.Add(cropInstance);
                    }
                }
            }

            // 저장소 데이터 로드
            var storage = FindObjectOfType<Storage>();
            if (storage != null)
            {
                storage.storedCropsByID = saveData.storedCropsByID;
                Debug.Log("저장소 데이터가 성공적으로 로드되었습니다.");
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