using System;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

public class WinApi
{
    #region [ singleton ]
    private static WinApi instance;
    private WinApi() { }
    public static WinApi Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new WinApi();
                instance.Initialize();
            }
            return instance;
        }
    }
    #endregion

    #region [ DllImport ]  
    #region [ Volume ]
    [DllImport("user32.dll")]
    private static extern System.IntPtr GetActiveWindow();

    [DllImport("user32.dll")]
    public static extern IntPtr SendMessageW(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

    private const int APPCOMMAND_VOLUME_MUTE = 0x80000;
    private const int APPCOMMAND_VOLUME_UP = 0xA0000;
    private const int APPCOMMAND_VOLUME_DOWN = 0x90000;
    private const int WM_APPCOMMAND = 0x319;
    #endregion

    #region [ Window Position and Size ]
    private delegate bool EnumWindowsProc(IntPtr hWnd, int lParam);

    [DllImport("user32.dll")]
    static extern IntPtr SetWindowLong(IntPtr hwnd, int _nIndex, int dwNewLong);

    [DllImport("user32.dll")]
    static extern bool SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    [DllImport("user32.dll")]
    static extern IntPtr GetForegroundWindow();

    const UInt32 SWP_NOSIZE = 0x0001;
    const UInt32 SWP_NOMOVE = 0x0002;
    const UInt32 SWP_NOACTIVATE = 0x0010;
    const UInt32 SWP_SHOWWINDOW = 0x0040;

    const int GWL_STYLE = -16;
    const int WS_BORDER = 1;
    #endregion

    #region [ Get Unity Windows Handler ]
    [DllImport("kernel32.dll")]
    static extern uint GetCurrentThreadId();

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool EnumThreadWindows(uint dwThreadId, EnumWindowsProc lpEnumFunc, IntPtr lParam);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    static extern int GetClassName(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
    #endregion
    #endregion

    #region [ Properties ]
    public IntPtr UnityWindowHandle { get; private set; }
    public string UnityWindowClassName { get { return "UnityWndClass"; } }
    #endregion

    #region [ constructor ]
    public void Initialize()
    {
        UpdateUnityWindowHandle();
    }
    #endregion

    #region [ private methods ]
    public void UpdateUnityWindowHandle()
    {
        uint threadId = GetCurrentThreadId();
        EnumThreadWindows(threadId, (hWnd, lParam) =>
        {
            var classText = new StringBuilder(UnityWindowClassName.Length + 1);
            GetClassName(hWnd, classText, classText.Capacity);
            if (classText.ToString() == UnityWindowClassName)
            {
                UnityWindowHandle = hWnd;
                return false;
            }
            return true;
        }, IntPtr.Zero);
    }
    #endregion

    #region [ public methods ]
    public void SetWindowsRectangle(Vector2 position, Vector2 size)
    {
        SetWindowLong(UnityWindowHandle, GWL_STYLE, WS_BORDER);
        bool result = SetWindowPos(UnityWindowHandle, 0, (int)position.x, (int)position.y, (int)size.x, (int)size.y, SWP_SHOWWINDOW);
    }
    public void VolumeMute()
    {
        SendMessageW(
            GetActiveWindow(), 
            WM_APPCOMMAND,
            GetActiveWindow(),
            (IntPtr)APPCOMMAND_VOLUME_MUTE);
    }
    public void VolumeDown()
    {
        SendMessageW(GetActiveWindow(), WM_APPCOMMAND, GetActiveWindow(),
            (IntPtr)APPCOMMAND_VOLUME_DOWN);
    }
    public void VolumeUp()
    {
        SendMessageW(GetActiveWindow(), WM_APPCOMMAND, GetActiveWindow(),
            (IntPtr)APPCOMMAND_VOLUME_UP);
    }
    #endregion
}
