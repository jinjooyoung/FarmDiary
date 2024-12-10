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

    [SerializeField] private float autoSaveInterval = 300f; // �ڵ� ���� ���� (��)
    private float autoSaveTimer; // �ڵ� ���� Ÿ�̸�

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
        //static ����� ��ũ��Ʈ���� �޾ƿͼ� ȣ���ؾ��ϹǷ� �����ϰ� start���� ȣ��
        InitializeSaveData();

        if (PlayerPrefs.GetInt(CoinKey, currentCoin) < 0)
        {
            Debug.LogWarning("������ ������ �Ǿ� �ʱ�ȭ �Ǿ����ϴ�!");
            currentCoin = 150160;
            PlayerPrefs.SetInt(CoinKey, currentCoin);
            PlayerPrefs.Save();
        }

        // �ڵ� ���� Ÿ�̸� �ʱ�ȭ
        autoSaveTimer = autoSaveInterval;

        gridData = placementSystem.placedOBJData;

        if (PlayerPrefs.GetInt("TutorialDone", 0) == 0)
        {
            currentCoin = 150160;
        }
    }

    void Update()
    {
        // Ÿ�̸� ������Ʈ
        autoSaveTimer -= Time.deltaTime;

        if (autoSaveTimer <= 0)
        {
            SaveGameDatasAsync();
            Debug.Log("�ڵ� ���� �Ϸ�!");
            autoSaveTimer = autoSaveInterval; // Ÿ�̸� ����
        }

        testText.text = currentCoin.ToString("N0");
    }

    private void InitializeSaveData()
    {
        if (PlayerPrefs.GetInt("TutorialDone", 0) == 0) // ù �����̸� = ���̺� �����Ͱ� ������
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

    // ���� ���� �� ȣ��Ǵ� �޼���
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

    // ���� �߰� �޼���
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
            // �����÷ο찡 �߻��� ��� ó���� ����
            currentCoin = int.MaxValue; // �ִ밪���� �����ϰų� �ٸ� ó���� �� �� ����
            Debug.LogWarning("�����÷ο� ������ ���� �ִ� ������");
        }

        AchievementsDatabase.CoinProgress(currentCoin);
        PlayerPrefs.SetInt(CoinKey, currentCoin);
        PlayerPrefs.Save();
    }

    // ���� ���� �޼���
    public static void SubtractCoins(int amount)
    {
        if (amount < 0)
            return;

        // ���� ���ο��� amount�� ���� ���� int�� �ּڰ��� ���� �ʵ��� üũ
        // ���ʿ� ����� ������ �������� - �Ǳ� ������ ���������� �����÷ο찡 �Ͼ�� �ʱ� ������ Ȥ�� �𸣴ϱ� �ۼ��ص�
        if (currentCoin - amount < int.MinValue)
        {
            currentCoin = 0;
            Debug.LogWarning("���� ���ҷ� ���� �����÷ο� �߻�!");
            return;
        }

        // ����� ������ �ִ��� üũ
        if (currentCoin - amount < 0)
        {
            currentCoin = 0;
            Debug.Log("������ ������� �ʽ��ϴ�.");
            return;
        }

        // ���� ����
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