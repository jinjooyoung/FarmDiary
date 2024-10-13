using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Camera mainCam;

    public Text testText;       // ���� ���ξ��� ���� Ȯ���ϱ� ����

    // ���� �߿��� �׽�Ʈ�� ���ؼ� �ӽ÷� 9999999999�� �����ص�. ���߿��� 0���� �ٲ� ����
    public static ulong coin = 9999999999;  // ��ü ���� ��
    public static ulong bio = 9999999999;   // ��ü ���� ��
    public static ulong gem = 9999999999;   // ��ü ���� ��

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
        testText.text = "���� ����: " + coin.ToString();
    }

    // ���� �߰� �޼���
    public static void AddCoins(int amount)
    {
        if (amount < 0)
            return;
        coin += (ulong)amount;
        //Debug.Log("�� ���� : " + coin);
    }

    // ���� �߰� �޼���
    public static void AddFuel(int amount)
    {
        if (amount < 0)
            return;
        bio += (ulong)amount;
        //Debug.Log("�� ���̿� : " + bio);
    }

    // ���� �߰� �޼���
    public static void AddGems(int amount)
    {
        if (amount < 0)
            return;
        gem += (ulong)amount;
        //Debug.Log("�� ���� : " + gems);
    }

    //==========================================================================

    // ���� ���� �޼���
    public static void SubtractCoins(int amount)
    {
        if (amount < 0)
            return;
        if (coin - (ulong)amount < 0)
        {
            Debug.Log("������ ������� �ʽ��ϴ�.");
            return;
        }
        else
        {
            coin -= (ulong)amount;
        }
        //Debug.Log("�� ���� : " + coin);
    }

    // ���� ���� �޼���
    public static void SubtractFuel(int amount)
    {
        if (amount < 0)
            return;
        if (bio - (ulong)amount < 0)
        {
            Debug.Log("���̿��� ������� �ʽ��ϴ�.");
            return;
        }
        else
        {
            bio -= (ulong)amount;
        }
        //Debug.Log("�� ���̿� : " + bio);
    }

    // ���� ���� �޼���
    public static void SubtractGems(int amount)
    {
        if (amount < 0)
            return;
        if (gem - (ulong)amount < 0)
        {
            Debug.Log("������ ������� �ʽ��ϴ�.");
            return;
        }
        else
        {
            gem -= (ulong)amount;
        }
        //Debug.Log("�� ���� : " + gems);
    }
}
