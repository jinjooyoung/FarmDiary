using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class KeyboardManager : MonoBehaviour
{
    [DllImport("KeyboardHook")]
    private static extern void SetHook();

    [DllImport("KeyboardHook")]
    private static extern void Unhook();

    [DllImport("KeyboardHook")]
    private static extern int GetKeyPressCount();

    [DllImport("KeyboardHook")]
    private static extern void ResetKeyPressCount(); // C++에서 만든 초기화 함수

    public Text uiText;
    private int keyPressCount = 0;

    void Start()
    {
        SetHook();
    }

    void Update()
    {
        int currentCount = GetKeyPressCount();

        if (currentCount != keyPressCount)
        {
            keyPressCount = currentCount;
            UpdateUIText();
        }
    }

    void OnApplicationQuit()
    {
        ResetKeyPressCount(); // 애플리케이션 종료 시 카운트 초기화
        Unhook(); // 훅 해제
    }

    private void UpdateUIText()
    {
        if (uiText != null)
        {
            uiText.text = $"키 입렵 횟수: {keyPressCount}";
        }
    }
}
