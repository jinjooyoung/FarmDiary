using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Camera mainCam;

    public Text testText;       // ���� ���ξ��� ���� Ȯ���ϱ� ����

    private const string FirstLaunchKey = "IsFirstLaunch";
    private const string CoinKey = "Coin";
    private const string GemKey = "Gem";
    private const int DefaultCoin = 100;
    private const int DefaultGem = 0;

    public static int currentCoin = 0;
    public static int currentGem = 0;
    

    private void Awake()
    {
        InitializePlayerPrefs();
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        testText.text = "���� ����: " + currentCoin.ToString();
    }

    private void InitializePlayerPrefs()
    {
        // ó�� ����Ǵ��� ���θ� Ȯ��
        if (PlayerPrefs.GetInt(FirstLaunchKey, 0) == 0)     // ���߿� ���⿡�� �۹� �ر� ������ �ʱ�ȭ�ؾ���
        {
            // PlayerPrefs �ʱ�ȭ
            PlayerPrefs.SetInt(CoinKey, DefaultCoin);
            PlayerPrefs.SetInt(GemKey, DefaultGem);
            currentCoin = DefaultCoin;
            currentGem = DefaultGem;

            // ó�� ����Ǿ����� ǥ��
            PlayerPrefs.SetInt(FirstLaunchKey, 1);

            // ��������� ����
            PlayerPrefs.Save();
        }
        else
        {
            // ���� ����� �� �ҷ�����
            currentCoin = PlayerPrefs.GetInt(CoinKey, DefaultCoin);
            currentGem = PlayerPrefs.GetInt(GemKey, DefaultGem);
        }
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
}
