using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Runtime.InteropServices;

public class TransparentWindow : MonoBehaviour
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Margins
    {
        public int Left, Right, Top, Bottom;
    }
    
    private IntPtr hwnd = IntPtr.Zero;
    private Margins windowMargins;
    
    public Text pointerStatusText;   // UI Text 컴포넌트를 참조하는 퍼블릭 변수
    
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

    const int GWL_EXSTYLE = -20;
    const uint WS_EX_LAYERED = 0x80000;
    const uint WS_EX_TRANSPARENT = 0x20;
    const uint LWA_COLORKEY = 0x00000001;
    private const uint LWA_ALPHA = 0x2;

    private bool wasPointerOverUILastFrame = false;

    void Start()
    {
        MakeWindowTransparent();
        // 애플리케이션이 백그라운드에서도 실행될 수 있도록 설정
        Application.runInBackground = true;
    }

    void Update()
    {
        SetClickThrough();
        UpdatePointerStatusText();
    }

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
    
    private void SetClickThrough()
    {
        bool isPointerOverUI = FocusForInput();
        IntPtr hwnd = GetUnityWindowHandle();

        if (hwnd == IntPtr.Zero) return;

        uint newStyle = isPointerOverUI ? WS_EX_LAYERED : (WS_EX_LAYERED | WS_EX_TRANSPARENT);
        SetWindowLong(hwnd, GWL_EXSTYLE, newStyle);
        SetLayeredWindowAttributes(hwnd, 0, byte.MaxValue, LWA_ALPHA);
    }

    void MakeWindowTransparent()
    {
        IntPtr hwnd = GetUnityWindowHandle();
        if (hwnd == IntPtr.Zero) return;
        
        uint newStyle = WS_EX_LAYERED | WS_EX_TRANSPARENT;
        SetWindowLong(hwnd, GWL_EXSTYLE, newStyle);
        SetLayeredWindowAttributes(hwnd, 0, byte.MaxValue, LWA_ALPHA);

        // Dwmapi를 통해 프레임 확장
        Margins margins = new Margins() { Left = -1, Right = -1, Top = -1, Bottom = -1 };
        DwmExtendFrameIntoClientArea(hwnd, ref margins);
    }

    IntPtr GetUnityWindowHandle()
    {
        // Unity 윈도우 핸들 얻기
        return FindWindow(null, Application.productName);
    }

    [DllImport("user32.dll", SetLastError = true)]
    static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    // 테스트를 위해 쓴 코드 완성되면 지울 예정
    private void UpdatePointerStatusText()
    {
        bool isPointerOverUI = FocusForInput();

        if (pointerStatusText != null)
        {
            // 포인터가 UI 위에 있는지, 바탕화면 위에 있는지 정보를 표시
            pointerStatusText.text = $"Pointer Over UI: {isPointerOverUI}";
        }
    }
}
