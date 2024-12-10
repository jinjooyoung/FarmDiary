using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GridData gridData;

    public Camera mainCam;
    public Text testText;

    private const string CoinKey = "Coin";
    private const int DefaultCoin = 100;

    public static int currentCoin = 0;

    [SerializeField] private FieldManager fieldManager;

    [SerializeField] private PlacementSystem placementSystem;

    [SerializeField] private float autoSaveInterval = 300f; // 자동 저장 간격 (초)
    private float autoSaveTimer; // 자동 저장 타이머

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

        SaveSystem.Init();
        
    }

    void Start()
    {
        SoundManager.instance.PlaySound("background");
        //static 선언된 스크립트들을 받아와서 호출해야하므로 안전하게 start에서 호출
        InitializeSaveData();

        if (PlayerPrefs.GetInt(CoinKey, currentCoin) < 0)
        {
            Debug.LogWarning("코인이 음수가 되어 초기화 되었습니다!");
            currentCoin = 150160;
            PlayerPrefs.SetInt(CoinKey, currentCoin);
            PlayerPrefs.Save();
        }

        // 자동 저장 타이머 초기화
        autoSaveTimer = autoSaveInterval;

        gridData = placementSystem.placedOBJData;

        if (PlayerPrefs.GetInt("TutorialDone", 0) == 0)
        {
            currentCoin = 150160;
        }
    }

    void Update()
    {
        // 타이머 업데이트
        autoSaveTimer -= Time.deltaTime;

        if (autoSaveTimer <= 0)
        {
            SaveGameDatasAsync();
            Debug.Log("자동 저장 완료!");
            autoSaveTimer = autoSaveInterval; // 타이머 리셋
        }

        testText.text = currentCoin.ToString("N0");
    }

    private void InitializeSaveData()
    {
        if (PlayerPrefs.GetInt("TutorialDone", 0) == 0) // 첫 실행이면 = 세이브 데이터가 없으면
        {
            currentCoin = DefaultCoin;
            SaveGameDatasAsync();
        }
        else
        {
            currentCoin = PlayerPrefs.GetInt(CoinKey, currentCoin);
            LoadGameDatasAsync();
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

    // 게임 종료 시 호출되는 메서드
    void OnApplicationQuit()
    {
        SaveGameDatasAsync();
    }

    public async void SaveGameDatasAsync()
    {
        await SaveManager.Instance.SaveGameData();
    }

    public async void LoadGameDatasAsync()
    {
        await SaveManager.Instance.LoadGameData();
    }

    // 코인 추가 메서드
    public static void AddCoins(int amount)
    {
        if (amount < 0)
            return;

        if (currentCoin <= int.MaxValue - amount)
        {
            currentCoin += amount;
        }
        else
        {
            // 오버플로우가 발생할 경우 처리할 로직
            currentCoin = int.MaxValue; // 최대값으로 설정하거나 다른 처리를 할 수 있음
            Debug.LogWarning("오버플로우 방지로 코인 최댓값 고정됨");
        }

        AchievementsDatabase.CoinProgress(currentCoin);
        PlayerPrefs.SetInt(CoinKey, currentCoin);
        PlayerPrefs.Save();
    }

    // 코인 차감 메서드
    public static void SubtractCoins(int amount)
    {
        if (amount < 0)
            return;

        // 현재 코인에서 amount를 빼는 것이 int의 최솟값을 넘지 않도록 체크
        // 애초에 충분한 코인이 있을때만 - 되기 때문에 차감에서는 오버플로우가 일어나지 않긴 하지만 혹시 모르니까 작성해둠
        if (currentCoin - amount < int.MinValue)
        {
            currentCoin = 0;
            Debug.LogWarning("코인 감소로 인한 오버플로우 발생!");
            return;
        }

        // 충분한 코인이 있는지 체크
        if (currentCoin - amount < 0)
        {
            currentCoin = 0;
            Debug.Log("코인이 충분하지 않습니다.");
            return;
        }

        // 코인 차감
        currentCoin -= amount;
        AchievementsDatabase.CoinProgress(currentCoin);
        PlayerPrefs.SetInt(CoinKey, currentCoin);
        PlayerPrefs.Save();
    }
}

/*[System.Serializable]
public class AllSaveData
{
    public int coin;
    public Vector3 playerPosition;
    public string gridDataJson;
    public List<Crop> crops;
    public List<FarmField> fields;
    public List<CropStorage> storedCropsByID;
    public int currentWaterAmount;
}*/