using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Camera mainCam;

    public Text testText;       // 현재 코인양을 쉽게 확인하기 위해

    // 개발 중에는 테스트를 위해서 임시로 9999999999로 선언해둠. 나중에는 0으로 바꿀 예정
    public static ulong coin = 9999999999;  // 전체 코인 수
    public static ulong bio = 9999999999;   // 전체 연료 수
    public static ulong gem = 9999999999;   // 전체 보석 수

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
    }

    private void Update()
    {
        testText.text = "현재 코인: " + coin.ToString();
    }

    // 코인 추가 메서드
    public static void AddCoins(int amount)
    {
        if (amount < 0)
            return;
        coin += (ulong)amount;
        //Debug.Log("총 코인 : " + coin);
    }

    // 연료 추가 메서드
    public static void AddFuel(int amount)
    {
        if (amount < 0)
            return;
        bio += (ulong)amount;
        //Debug.Log("총 바이오 : " + bio);
    }

    // 보석 추가 메서드
    public static void AddGems(int amount)
    {
        if (amount < 0)
            return;
        gem += (ulong)amount;
        //Debug.Log("총 보석 : " + gems);
    }

    //==========================================================================

    // 코인 차감 메서드
    public static void SubtractCoins(int amount)
    {
        if (amount < 0)
            return;
        if (coin - (ulong)amount < 0)
        {
            Debug.Log("코인이 충분하지 않습니다.");
            return;
        }
        else
        {
            coin -= (ulong)amount;
        }
        //Debug.Log("총 코인 : " + coin);
    }

    // 연료 차감 메서드
    public static void SubtractFuel(int amount)
    {
        if (amount < 0)
            return;
        if (bio - (ulong)amount < 0)
        {
            Debug.Log("바이오가 충분하지 않습니다.");
            return;
        }
        else
        {
            bio -= (ulong)amount;
        }
        //Debug.Log("총 바이오 : " + bio);
    }

    // 보석 차감 메서드
    public static void SubtractGems(int amount)
    {
        if (amount < 0)
            return;
        if (gem - (ulong)amount < 0)
        {
            Debug.Log("보석이 충분하지 않습니다.");
            return;
        }
        else
        {
            gem -= (ulong)amount;
        }
        //Debug.Log("총 보석 : " + gems);
    }
}
