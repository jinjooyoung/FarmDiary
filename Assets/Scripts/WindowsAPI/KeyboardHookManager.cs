using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class KeyboardHookManager : MonoBehaviour
{
    // Windows API �Լ� ȣ���� ���� P/Invoke ����
    [DllImport("user32.dll")]
    private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll")]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll")]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr GetModuleHandle(string lpModuleName);

    // ��������Ʈ ���� (Ű���� �̺�Ʈ ó��)
    private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

    private LowLevelKeyboardProc _proc;
    private static IntPtr _hookID = IntPtr.Zero;

    //public Text uiText;
    public int keyPressCount = 0;               // ���� ���൵�� �ľ��ϱ� ���� Ű �Է� Ƚ�� ����
    public bool achievementAllClear = false;    // ������ ��� �Ϸ�Ǹ� ���̻� ī��Ʈ ���� �ʱ� ���� bool��



    private const int WH_KEYBOARD_LL = 13; // ���� Ű���� ��
    private const int WM_KEYDOWN = 0x0100; // Ű�� ���� �̺�Ʈ
    private const int WM_KEYUP = 0x0101;   // Ű�� ������ �̺�Ʈ

    private bool[] keyStates = new bool[256]; // �� Ű ���� ���� �迭

    void Awake()
    {
        keyPressCount = 0;
        // ��׶��忡�� ����ǵ��� ����
        Application.runInBackground = true;

        // Ű �Է� Ƚ�� �ʱ�ȭ
        //keyPressCount = 0; // �� ���� �� ī��Ʈ �ʱ�ȭ
        

        // �� ����
        _proc = HookCallback;
        _hookID = SetHook(_proc);
    }

    void OnDestroy()
    {
        // ���� �� �� ����
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

    // �� �ݹ� �Լ�: Ű �Է��� ó��
    private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0)
        {
            int vkCode = Marshal.ReadInt32(lParam); // ���� Ű �ڵ带 ����

            // Ű�� ������ ��
            if (wParam == (IntPtr)WM_KEYDOWN && !keyStates[vkCode])
            {
                if (PlayerPrefs.GetInt("KetboardAllClear", 0) == 0)
                {
                    keyPressCount++; // Ű �Է� Ƚ�� ����

                    // Ű���� ���� ���� 1�� ���� & Ŭ����
                    AchievementsDatabase.AddProgressToAchievement(3, 1);
                    AchievementsDatabase.AddProgressToAchievement(6, 1);
                    AchievementsDatabase.AddProgressToAchievement(7, 1);
                    AchievementsDatabase.AddProgressToAchievement(8, 1);

                    // Ű���� ���� �ر�
                    if (AchievementsDatabase.GetCleared(3))
                    {
                        AchievementsDatabase.UnlockAchievement(6);
                        AchievementsDatabase.UnlockAchievement(7);
                        AchievementsDatabase.UnlockAchievement(8);
                    }
                    else if (AchievementsDatabase.GetCleared(8))
                    {
                        PlayerPrefs.SetInt("KetboardAllClear", 1);      // Ű���� ���� ������ �� Ŭ���� �ؼ� 1�� ����.
                    }
                }
                
                GameManager.AddCoins(1);
                keyStates[vkCode] = true; // Ű ���¸� '����'���� ����
                //UpdateUIText();
            }
            // Ű�� �������� ��
            else if (wParam == (IntPtr)WM_KEYUP)
            {
                keyStates[vkCode] = false; // Ű ���¸� '����'���� ����
            }
        }

        return CallNextHookEx(_hookID, nCode, wParam, lParam);
    }

    /*private void UpdateUIText()
    {
        if (uiText != null)
        {
            uiText.text = $"Ű �Է� Ƚ��: {keyPressCount}";
        }
    }*/
}
