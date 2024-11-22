using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class KeyboardHookManager : MonoBehaviour
{
    // Windows API 함수 호출을 위한 P/Invoke 선언
    [DllImport("user32.dll")]
    private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll")]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll")]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr GetModuleHandle(string lpModuleName);

    // 델리게이트 정의 (키보드 이벤트 처리)
    private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

    private LowLevelKeyboardProc _proc;
    private static IntPtr _hookID = IntPtr.Zero;

    //public Text uiText;
    public int keyPressCount = 0;               // 업적 진행도를 파악하기 위한 키 입력 횟수 저장
    public bool achievementAllClear = false;    // 업적이 모두 완료되면 더이상 카운트 하지 않기 위해 bool값



    private const int WH_KEYBOARD_LL = 13; // 전역 키보드 훅
    private const int WM_KEYDOWN = 0x0100; // 키가 눌린 이벤트
    private const int WM_KEYUP = 0x0101;   // 키가 떼어진 이벤트

    private bool[] keyStates = new bool[256]; // 각 키 상태 추적 배열

    void Awake()
    {
        keyPressCount = 0;
        // 백그라운드에서 실행되도록 설정
        Application.runInBackground = true;

        // 키 입력 횟수 초기화
        //keyPressCount = 0; // 앱 시작 시 카운트 초기화
        

        // 훅 설정
        _proc = HookCallback;
        _hookID = SetHook(_proc);
    }

    void OnDestroy()
    {
        // 종료 시 훅 해제
        UnhookWindowsHookEx(_hookID);
    }

    private IntPtr SetHook(LowLevelKeyboardProc proc)
    {
        using (Process curProcess = Process.GetCurrentProcess())
        using (ProcessModule curModule = curProcess.MainModule)
        {
            return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
        }
    }

    // 훅 콜백 함수: 키 입력을 처리
    private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0)
        {
            int vkCode = Marshal.ReadInt32(lParam); // 가상 키 코드를 읽음

            // 키가 눌렸을 때
            if (wParam == (IntPtr)WM_KEYDOWN && !keyStates[vkCode])
            {
                if (PlayerPrefs.GetInt("KetboardAllClear", 0) == 0)
                {
                    keyPressCount++; // 키 입력 횟수 증가

                    // 키보드 관련 업적 1씩 증가 & 클리어
                    AchievementsDatabase.AddProgressToAchievement(3, 1);
                    AchievementsDatabase.AddProgressToAchievement(6, 1);
                    AchievementsDatabase.AddProgressToAchievement(7, 1);
                    AchievementsDatabase.AddProgressToAchievement(8, 1);

                    // 키보드 업적 해금
                    if (AchievementsDatabase.GetCleared(3))
                    {
                        AchievementsDatabase.UnlockAchievement(6);
                        AchievementsDatabase.UnlockAchievement(7);
                        AchievementsDatabase.UnlockAchievement(8);
                    }
                    else if (AchievementsDatabase.GetCleared(8))
                    {
                        PlayerPrefs.SetInt("KetboardAllClear", 1);      // 키보드 관련 업적을 다 클리어 해서 1로 변경.
                    }
                }
                
                GameManager.AddCoins(1);
                keyStates[vkCode] = true; // 키 상태를 '눌림'으로 설정
                //UpdateUIText();
            }
            // 키가 떼어졌을 때
            else if (wParam == (IntPtr)WM_KEYUP)
            {
                keyStates[vkCode] = false; // 키 상태를 '떼짐'으로 설정
            }
        }

        return CallNextHookEx(_hookID, nCode, wParam, lParam);
    }

    /*private void UpdateUIText()
    {
        if (uiText != null)
        {
            uiText.text = $"키 입력 횟수: {keyPressCount}";
        }
    }*/
}
