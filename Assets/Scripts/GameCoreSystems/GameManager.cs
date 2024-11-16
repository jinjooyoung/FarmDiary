using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

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
        InitializePlayerPrefs();
    }

    private void Update()
    {
        testText.text = "���� ����: " + currentCoin;

        if (Input.GetKeyDown(KeyCode.S))
        {
            SaveGameData();
            Debug.Log("���� ������ ���� �Ϸ�!");
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadGameData();
            Debug.Log("���� ������ �ε� �Ϸ�!");
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
        int totalCrops = 50; // �۹��� ������ ID(�ӽ÷� �ص� ���߿� �����ؾ���)

        for (int i = 9; i <= totalCrops; i++)
        {
            string cropKey = "CropUnlocked_" + i; // �� �۹��� �ر� ���� Ű
            PlayerPrefs.DeleteKey(cropKey); // �ش� Ű �����Ͽ� �ʱ�ȭ
        }

        // ��ü �ر� �ε����� �ʱ�ȭ (Optional)
        PlayerPrefs.SetInt("UnlockPlant", 0); // UnlockPlant�� �ʱ�ȭ
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
        PlayerPrefs.SetInt(CoinKey, currentCoin);
        PlayerPrefs.Save();
        //Debug.Log("�� ���� : " + coin);
    }

    // ���� �߰� �޼���
    public static void AddGems(int amount)
    {
        if (amount < 0)
            return;
        currentGem += amount;
        PlayerPrefs.SetInt(GemKey, currentGem);
        PlayerPrefs.Save();
        //Debug.Log("�� ���� : " + gems);
    }

    //==========================================================================

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
            PlayerPrefs.SetInt(CoinKey, currentCoin);
            PlayerPrefs.Save();
        }
        //Debug.Log("�� ���� : " + coin);
    }

    // ���� ���� �޼���
    public static void SubtractGems(int amount)
    {
        if (amount < 0)
            return;
        if (currentGem - amount < 0)
        {
            Debug.Log("������ ������� �ʽ��ϴ�.");
            return;
        }
        else
        {
            currentGem -= amount;
            PlayerPrefs.SetInt(GemKey, currentGem);
            PlayerPrefs.Save();
        }
        //Debug.Log("�� ���� : " + gems);
    }


    public void SaveGameData()
    {
        Vector3 playerPosition = playerTransform.position;

        AllSaveData saveData = new AllSaveData
        {
            coin = currentCoin,
            gem = currentGem,
            playerPosition = playerPosition
        };

        string json = JsonUtility.ToJson(saveData);

        // SaveSystem.Save ȣ�� �� ���� �̸� �߰�
        SaveSystem.Save(json, "GameData.json");
    }

    public void LoadGameData()
    {
        string saveString = SaveSystem.Load("GameData.json"); // ���� �̸� �߰�
        if (!string.IsNullOrEmpty(saveString))
        {
            AllSaveData saveData = JsonUtility.FromJson<AllSaveData>(saveString);
            currentCoin = saveData.coin;
            currentGem = saveData.gem;

            playerTransform.position = saveData.playerPosition;
        }
        else
        {
            Debug.Log("����� �����Ͱ� �����ϴ�.");
        }
    }
}

[System.Serializable]
public class AllSaveData
{
    public int coin;
    public int gem;
    public Vector3 playerPosition;
}
