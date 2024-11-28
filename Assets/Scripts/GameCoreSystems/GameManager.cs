using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] private List<PrefabData> seedPrefabs; // 씨앗 프리팹 리스트
    [SerializeField] private List<PrefabData> fieldPrefabs; // 밭 프리팹 리스트

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

    [SerializeField] private PlacementSystem placementSystem;

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
        InitializePlayerPrefs();
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
        gridData = placementSystem.placedOBJData;

        if (PlayerPrefs.GetInt("TutorialDone", 0) == 0)
        {
            currentCoin = 210;
        }

        Crop[] crops = FindObjectsOfType<Crop>();
        FarmField[] fields = FindObjectsOfType<FarmField>();

        crop.AddRange(crops);
        field.AddRange(fields);
    }

    public void RemoveMissingCrops()
    {
        crop.RemoveAll(c => c == null);
    }

    void Update()
    {
        RemoveMissingCrops();

        testText.text = "현재 코인: " + currentCoin;

        if (Input.GetKeyDown(KeyCode.S))
        {
            SaveGameData();
            Debug.Log("게임 데이터 저장 완료!");
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadGameData();
            Debug.Log("게임 데이터 로드 완료!");
        }

        float currentTime = Time.time;

        foreach (var crop in crop)
        {
            crop.CheckGrowth(currentTime); // 현재 시간 전달
        }
    }

    private void InitializePlayerPrefs()
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

    public void ResetCropKeys()
    {
        int totalCrops = 61; // 작물의 마지막 ID

        for (int i = 9; i <= totalCrops; i++)
        {
            string cropKey = "CropUnlocked_" + i; // 각 작물의 해금 상태 키
            PlayerPrefs.DeleteKey(cropKey); // 해당 키 삭제하여 초기화
        }

        // 전체 해금 인덱스도 초기화 (Optional)
        PlayerPrefs.SetInt("UnlockPlant", 0); // UnlockPlant도 초기화
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
        PlayerPrefs.SetInt(CoinKey, currentCoin);
        PlayerPrefs.Save();
        //Debug.Log("총 코인 : " + coin);
    }

    // 보석 추가 메서드
    public static void AddGems(int amount)
    {
        if (amount < 0)
            return;
        currentGem += amount;
        PlayerPrefs.SetInt(GemKey, currentGem);
        PlayerPrefs.Save();
        //Debug.Log("총 보석 : " + gems);
    }

    //==========================================================================

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
            PlayerPrefs.SetInt(CoinKey, currentCoin);
            PlayerPrefs.Save();
        }
        //Debug.Log("총 코인 : " + coin);
    }

    // 보석 차감 메서드
    public static void SubtractGems(int amount)
    {
        if (amount < 0)
            return;
        if (currentGem - amount < 0)
        {
            Debug.Log("보석이 충분하지 않습니다.");
            return;
        }
        else
        {
            currentGem -= amount;
            PlayerPrefs.SetInt(GemKey, currentGem);
            PlayerPrefs.Save();
        }
        //Debug.Log("총 보석 : " + gems);
    }

    public void SaveGameData()
    {
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

        // AllSaveData 객체 생성
        AllSaveData saveData = new AllSaveData
        {
            coin = currentCoin,
            gem = currentGem,
            playerPosition = playerPosition,
            gridDataJson = JsonUtility.ToJson(gridSaveData)
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
            currentGem = saveData.gem;
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
    public int gem;
    public Vector3 playerPosition;
    public string gridDataJson;
    public List<Crop> crops;
    public List<FarmField> fields;
}
