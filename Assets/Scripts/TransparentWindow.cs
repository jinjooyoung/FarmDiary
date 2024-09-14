using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class TransparentWindow : MonoBehaviour
{
    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);
    
    private const int GWL_EXSTYLE = -20;
    private const uint WS_EX_LAYERED = 0x80000;
    private const uint WS_EX_TRANSPARENT = 0x20;
    private const uint LWA_COLORKEY = 0x1;
    private const uint LWA_ALPHA = 0x2;

    private IntPtr hWnd;

    void Start()
    {
        hWnd = FindWindow(null, Application.productName);
        if (hWnd != IntPtr.Zero)
        {
            SetWindowLong(hWnd, GWL_EXSTYLE, WS_EX_LAYERED);
            SetLayeredWindowAttributes(hWnd, 000000, 0, LWA_COLORKEY);
        }
    }
}