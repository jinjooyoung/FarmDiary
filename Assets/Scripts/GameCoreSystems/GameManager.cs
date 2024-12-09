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
        InitializeSaveData();
    }

    void Start()
    {
        //InitializePlayerPrefs();    // �׽�Ʈ�Ҷ��� �ּ� Ǯ��, ���߿� ����

        //�׽�Ʈ �Ѵٰ� �����Ҷ� ���� ���� �ص� ���߿� �����ؾ���
        currentCoin = 2147483647;
        Debug.LogWarning("�׽�Ʈ ���� �Ҵ� �����");

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
            SaveManager.Instance.SaveGameData();
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
            SaveManager.Instance.SaveGameData();
        }
        else
        {
            currentCoin = PlayerPrefs.GetInt(CoinKey, currentCoin);
            SaveManager.Instance.LoadGameData();
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
        // ���� ���� �ÿ� ������ �۾��� ���⿡ �ۼ�
        Debug.Log("������ ����˴ϴ�. ���� �۾��� �����մϴ�.");

        // ���� �޼��� ȣ��
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