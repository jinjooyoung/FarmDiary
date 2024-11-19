using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

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
    [SerializeField] private List<FarmField> field = new List<FarmField>();

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

        SaveSystem.Init();
        InitializePlayerPrefs();
    }

    void Start()
    {
        gridData = placementSystem.placedOBJData;

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
        int totalCrops = 50; // 작물의 마지막 ID(임시로 해둠 나중에 수정해야함)

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

        // AllSaveData 객체 생성
        AllSaveData saveData = new AllSaveData
        {
            coin = currentCoin,
            gem = currentGem,
            playerPosition = playerPosition,
            gridDataJson = gridData.SaveFieldsAndCrops(), // 그리드 데이터 저장
            crops = crop, 
            fields = field
        };

        // 객체를 JSON으로 직렬화
        string json = JsonUtility.ToJson(saveData);

        // 파일로 저장
        SaveSystem.Save(json, "GameData.json");
    }

    public void LoadGameData()
    {
        string saveString = SaveSystem.Load("GameData.json");

        if (!string.IsNullOrEmpty(saveString))
        {
            // JSON을 객체로 역직렬화
            AllSaveData saveData = JsonUtility.FromJson<AllSaveData>(saveString);

            // 로드된 데이터 적용
            currentCoin = saveData.coin;
            currentGem = saveData.gem;
            playerTransform.position = saveData.playerPosition;
            gridData.LoadFieldsAndCrops(saveData.gridDataJson);

            // crops와 fields 리스트 로드
            crop = saveData.crops;
            field = saveData.fields;
        }
        else
        {
            Debug.Log("저장된 데이터가 없습니다.");
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
