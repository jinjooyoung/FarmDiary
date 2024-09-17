#include "pch.h"
#include <windows.h>
#include <string>

HHOOK hHook = NULL;
HINSTANCE hInstance;
int keyPressCount = 0;
bool isKeyPressed = false; // 키가 눌린 상태를 확인하는 플래그

LRESULT CALLBACK LowLevelKeyboardProc(int nCode, WPARAM wParam, LPARAM lParam);

extern "C" __declspec(dllexport) void SetHook()
{
    if (hHook == NULL)
    {
        hHook = SetWindowsHookEx(WH_KEYBOARD_LL, LowLevelKeyboardProc, GetModuleHandle(NULL), 0);
        if (!hHook)
        {
            MessageBox(NULL, L"Failed to set hook!", L"Error", MB_ICONERROR);
        }
    }
}

extern "C" __declspec(dllexport) void Unhook()
{
    if (hHook)
    {
        UnhookWindowsHookEx(hHook);
        hHook = NULL;
    }
}

LRESULT CALLBACK LowLevelKeyboardProc(int nCode, WPARAM wParam, LPARAM lParam)
{
    if (nCode >= 0)
    {
        KBDLLHOOKSTRUCT* pKeyboard = (KBDLLHOOKSTRUCT*)lParam;

        // 키가 눌렸을 때
        if (wParam == WM_KEYDOWN && !isKeyPressed)
        {
            keyPressCount++;
            isKeyPressed = true; // 키가 눌린 상태로 설정
            std::wstring message = L"Key pressed count: " + std::to_wstring(keyPressCount) + L"\n";
            OutputDebugString(message.c_str());
        }

        // 키가 떼어졌을 때
        if (wParam == WM_KEYUP)
        {
            isKeyPressed = false; // 키가 떼어졌을 때 플래그 해제
        }
    }
    return CallNextHookEx(hHook, nCode, wParam, lParam);
}

// keyPressCount 값을 반환하는 함수
extern "C" __declspec(dllexport) int GetKeyPressCount()
{
    return keyPressCount;
}

// 키보드 입력 카운트를 초기화하는 함수
extern "C" __declspec(dllexport) void ResetKeyPressCount()
{
    keyPressCount = 0;
}

BOOL APIENTRY DllMain(HMODULE hModule, DWORD ul_reason_for_call, LPVOID lpReserved)
{
    if (ul_reason_for_call == DLL_PROCESS_ATTACH)
    {
        hInstance = hModule;
    }
    return TRUE;
}
