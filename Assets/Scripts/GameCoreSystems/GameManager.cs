using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Camera mainCam;

    public Text testText;       // 현재 코인양을 쉽게 확인하기 위해

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
        testText.text = "현재 코인: " + currentCoin.ToString();
    }

    private void InitializePlayerPrefs()
    {
        // 처음 실행되는지 여부를 확인
        if (PlayerPrefs.GetInt(FirstLaunchKey, 0) == 0)     // 나중에 여기에서 작물 해금 정도도 초기화해야함
        {
            // PlayerPrefs 초기화
            PlayerPrefs.SetInt(CoinKey, DefaultCoin);
            PlayerPrefs.SetInt(GemKey, DefaultGem);
            currentCoin = DefaultCoin;
            currentGem = DefaultGem;

            // 처음 실행되었음을 표시
            PlayerPrefs.SetInt(FirstLaunchKey, 1);

            // 변경사항을 저장
            PlayerPrefs.Save();
        }
        else
        {
            // 기존 저장된 값 불러오기
            currentCoin = PlayerPrefs.GetInt(CoinKey, DefaultCoin);
            currentGem = PlayerPrefs.GetInt(GemKey, DefaultGem);
        }
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
}
