using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Runtime.InteropServices;

public class TransparentWindow : MonoBehaviour
{
    // Windows API 호출을 위한 구조체 정의
    [StructLayout(LayoutKind.Sequential)]
    public struct Margins
    {
        public int Left, Right, Top, Bottom;
    }

    // Window 핸들 및 Margins 저장 변수
    private IntPtr hwnd = IntPtr.Zero;
    private Margins windowMargins;

    // UI Text 컴포넌트를 참조하는 퍼블릭 변수
    public Text pointerStatusText;   // 포인터 상태를 표시할 Text
    public Text isTopmostText;       // 창의 Topmost 상태를 표시할 Text

    // 창이 최상위 상태인지 여부를 저장하는 변수
    public bool isTopmost = false;

    // Windows API 호출을 위한 함수 선언
    [DllImport("user32.dll", SetLastError = true)]
    static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    [DllImport("user32.dll")]
    static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

    [DllImport("user32.dll")]
    static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll")]
    static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

    [DllImport("user32.dll")]
    static extern bool GetWindowRect(IntPtr hwnd, out Margins rect); // 윈도우의 위치와 크기 가져오기

    [DllImport("dwmapi.dll")]
    static extern void DwmExtendFrameIntoClientArea(IntPtr hWnd, ref Margins pMarInset);

    // 상수 정의
    const int GWL_EXSTYLE = -20; // 확장 스타일의 인덱스
    const uint WS_EX_LAYERED = 0x80000; // 레이어드 창 스타일
    const uint WS_EX_TRANSPARENT = 0x20; // 투명 창 스타일
    const uint LWA_COLORKEY = 0x00000001; // 색상 키
    private const uint LWA_ALPHA = 0x2; // 알파 값

    const uint SWP_NOSIZE = 0x0001; // 크기 변경 없음
    const uint SWP_NOMOVE = 0x0002; // 위치 변경 없음
    const uint SWP_NOACTIVATE = 0x0010; // 활성화 변경 없음
    const uint SWP_SHOWWINDOW = 0x0040; // 창 표시

    static readonly IntPtr HWND_TOPMOST = new IntPtr(-1); // 최상위 창
    static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2); // 일반 창

    private bool wasPointerOverUILastFrame = false;

    void Start()
    {
        MakeWindowTransparent(); // 윈도우를 투명하게 설정
        Application.runInBackground = true; // 애플리케이션이 백그라운드에서도 실행되도록 설정
        isTopmost = false; // 초기 상태는 최상위 창이 아님
    }

    void Update()
    {
        SetClickThrough(); // 클릭을 통해서만 창이 투명하게 보이도록 설정
        UpdatePointerStatusText(); // UI 텍스트 업데이트
    }

    // 현재 마우스 포인터가 UI 요소 위에 있는지 확인
    private bool FocusForInput()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        return results.Count > 0;
    }

    // UI 위에 포인터가 있는지에 따라 클릭 가능한 상태로 설정
    private void SetClickThrough()
    {
        bool isPointerOverUI = FocusForInput();
        IntPtr hwnd = GetUnityWindowHandle();

        if (hwnd == IntPtr.Zero) return; // 핸들이 유효하지 않으면 반환

        // 포인터가 UI 위에 있을 때와 아닐 때 스타일을 설정
        uint newStyle = isPointerOverUI ? WS_EX_LAYERED : (WS_EX_LAYERED | WS_EX_TRANSPARENT);
        SetWindowLong(hwnd, GWL_EXSTYLE, newStyle);
        SetLayeredWindowAttributes(hwnd, 0, byte.MaxValue, LWA_ALPHA);
    }

    // 윈도우를 투명하게 설정
    void MakeWindowTransparent()
    {
        IntPtr hwnd = GetUnityWindowHandle();
        if (hwnd == IntPtr.Zero) return; // 핸들이 유효하지 않으면 반환

        uint newStyle = WS_EX_LAYERED | WS_EX_TRANSPARENT;
        SetWindowLong(hwnd, GWL_EXSTYLE, newStyle);
        SetLayeredWindowAttributes(hwnd, 0, byte.MaxValue, LWA_ALPHA);

        // Dwmapi를 통해 프레임 확장
        Margins margins = new Margins() { Left = -1, Right = -1, Top = -1, Bottom = -1 };
        DwmExtendFrameIntoClientArea(hwnd, ref margins);
    }

    // Unity 윈도우 핸들 얻기
    IntPtr GetUnityWindowHandle()
    {
        return FindWindow(null, Application.productName);
    }

    // 버튼 클릭 시 호출할 메서드: Topmost 상태 토글
    public void ToggleTopmost()
    {
        isTopmost = !isTopmost; // 현재 상태를 반전
        UpdateTopmost(); // 상태에 따라 윈도우 업데이트
    }

    // 윈도우를 최상위 상태로 변경하거나 일반 상태로 복원
    private void UpdateTopmost()
    {
        IntPtr hwnd = GetUnityWindowHandle();
        if (hwnd == IntPtr.Zero) return; // 핸들이 유효하지 않으면 반환

        if (isTopmost)
        {
            // 창을 최상위로 설정
            SetWindowPos(hwnd, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE | SWP_NOACTIVATE | SWP_SHOWWINDOW);
        }
        else
        {
            // 창을 일반 상태로 설정
            SetWindowPos(hwnd, HWND_NOTOPMOST, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE | SWP_NOACTIVATE | SWP_SHOWWINDOW);
        }
    }

    // Windows API를 통해 창 핸들 얻기
    [DllImport("user32.dll", SetLastError = true)]
    static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    // 포인터 상태와 Topmost 상태를 UI에 업데이트
    private void UpdatePointerStatusText()
    {
        bool isPointerOverUI = FocusForInput();

        if (pointerStatusText != null)
        {
            // 포인터가 UI 위에 있는지와 Topmost 상태를 표시
            pointerStatusText.text = $"Pointer Over UI: {isPointerOverUI}";
            isTopmostText.text = $"Is Topmost: {isTopmost}"; // 수정된 부분
        }
    }
}
