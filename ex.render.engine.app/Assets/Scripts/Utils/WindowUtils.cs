using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;

public class WindowUtils
{
    public static void CloseFirefoxWindow()
    {
        string processName = "firefox";
        CloseWindow(processName);
    }

    public static void SendToBottom(string lpWindowName)
    {
        var wndHnd = FindWindow(null, lpWindowName);
        if (wndHnd.Equals(IntPtr.Zero))
            UnityEngine.Debug.Log("[WindowUtils] Send to bottom not found " + lpWindowName);
        SetWindowPos(wndHnd, new IntPtr(1), 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);
    }

    public static void SendToBottom(IntPtr wndHnd)
    {
        if (wndHnd.Equals(IntPtr.Zero))
        {
            SetWindowPos(wndHnd, new IntPtr(1), 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);
        }
    }

    public static void SendToForeground(IntPtr wndHnd)
    {
        if (wndHnd.Equals(IntPtr.Zero))
        {
            keybd_event(0, 0, 0, 0);
            //SetForegroundWindow(wndHnd);
            SetWindowPos(wndHnd, new IntPtr(0), 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);

        }
    }

    public static void SendToForeground(string lpWindowName)
    {
        var wndHnd = FindWindow(null, lpWindowName);
        if (wndHnd.Equals(IntPtr.Zero))
            UnityEngine.Debug.Log("[WindowUtils] Send to foreground not found " + lpWindowName);

        keybd_event(0, 0, 0, 0);
        //SetForegroundWindow(wndHnd);
        SetWindowPos(wndHnd, new IntPtr(0), 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);

    }


    public static void MaximizeWindowByProcessName(string processName)
    {
        Process[] processes = Process.GetProcessesByName(processName);
        foreach (var process in processes)
        {
            ShowWindowAsync(process.Handle, SW_SHOWMAXIMIZED);
        }
    }

    public static void MaximizeWindow(string lpWindowName)
    {
        var wndHnd = FindWindow(null, lpWindowName);
        if (wndHnd.Equals(IntPtr.Zero))
            UnityEngine.Debug.Log("Maximize not found "+lpWindowName);

        SetForegroundWindow(wndHnd);
        //ShowWindow(wndHnd, ShowWindowEnum.ShowNormal);
        ShowWindowAsync(wndHnd, SW_SHOWMAXIMIZED);
    }

    public static void MinimizeWindow(string lpWindowName)
    {
        var wndHnd = FindWindow(null, lpWindowName);
        ShowWindowAsync(wndHnd, SW_SHOWMINIMIZED);
    }

    public static void CloseWindow(string processName)
    {            
        Process[] processes = Process.GetProcessesByName(processName);
        if (processes.Length > 0)
        {
            foreach (var process in processes)
            {                    
                IDictionary<IntPtr, string> windows = List_Windows_By_PID(process.Id);
                foreach (KeyValuePair<IntPtr, string> pair in windows)
                {
                    /*
                    var placement = new WINDOWPLACEMENT();
                    GetWindowPlacement(pair.Key, ref placement);
 
                    if (placement.showCmd == SW_SHOWMINIMIZED)
                    {
                        //if minimized, show maximized
                        ShowWindowAsync(pair.Key, SW_SHOWMAXIMIZED);
                    }
                    else
                    {
                        //default to minimize
                        ShowWindowAsync(pair.Key, SW_SHOWMINIMIZED);
                    }*/
                }
            }
        }
    }


    [DllImport("user32.dll")]
    private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

    [DllImport("user32.dll")]
    static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    static readonly IntPtr HWND_BOTTOM = new IntPtr(1);
    const UInt32 SWP_NOSIZE = 0x0001;
    const UInt32 SWP_NOMOVE = 0x0002;
    const UInt32 SWP_NOACTIVATE = 0x0010;
    const UInt32 SWP_SHOWWINDOW = 0x0040;
    //

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
    static extern bool ShowWindow(IntPtr hWnd, ShowWindowEnum flags);
    private enum ShowWindowEnum
    {
        Hide = 0,
        ShowNormal = 1, ShowMinimized = 2, ShowMaximized = 3,
        Maximize = 3, ShowNormalNoActivate = 4, Show = 5,
        Minimize = 6, ShowMinNoActivate = 7, ShowNoActivate = 8,
        Restore = 9, ShowDefault = 10, ForceMinimized = 11
    };

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    public static extern int SetForegroundWindow(IntPtr hwnd);

    //

    [DllImport("user32.dll", SetLastError = true)]
    static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    private const int SW_SHOWNORMAL = 1;
    private const int SW_SHOWMINIMIZED = 2;
    private const int SW_SHOWMAXIMIZED = 3;
 
    /*
    private struct WINDOWPLACEMENT
    {
        public int length;
        public int flags;
        public int showCmd;
        public System.Drawing.Point ptMinPosition;
        public System.Drawing.Point ptMaxPosition;
        public System.Drawing.Rectangle rcNormalPosition;
    }
 */
    [DllImport("user32.dll")]
    private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);
 
    private delegate bool EnumWindowsProc(IntPtr hWnd, int lParam);
 
    [DllImport("user32.dll", SetLastError = true)]
    private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
 
    [DllImport("USER32.DLL")]
    private static extern bool EnumWindows(EnumWindowsProc enumFunc, int lParam);
 
    [DllImport("USER32.DLL")]
    private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
 
    [DllImport("USER32.DLL")]
    private static extern int GetWindowTextLength(IntPtr hWnd);
 
    [DllImport("USER32.DLL")]
    private static extern bool IsWindowVisible(IntPtr hWnd);
 
    [DllImport("USER32.DLL")]
    private static extern IntPtr GetShellWindow();
 
    //[DllImport("USER32.DLL")]
    //private static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);
 
 
    public static IDictionary<IntPtr, string> List_Windows_By_PID(int processID)
    {
        IntPtr hShellWindow = GetShellWindow();
        Dictionary<IntPtr, string> dictWindows = new Dictionary<IntPtr, string>();
 
        EnumWindows(delegate(IntPtr hWnd, int lParam)
        {
            //ignore the shell window
            if (hWnd == hShellWindow)
            {
                return true;
            }
 
            //ignore non-visible windows
            if (!IsWindowVisible(hWnd))
            {
                return true;
            }
 
            //ignore windows with no text
            int length = GetWindowTextLength(hWnd);
            if (length == 0)
            {
                return true;
            }
 
            uint windowPid;
            GetWindowThreadProcessId(hWnd, out windowPid);
 
            //ignore windows from a different process
            if (windowPid != processID)
            {
                return true;
            }
 
            StringBuilder stringBuilder = new StringBuilder(length);
            GetWindowText(hWnd, stringBuilder, length + 1);
            dictWindows.Add(hWnd, stringBuilder.ToString());
 
            return true;
 
        }, 0);
 
        return dictWindows;
    }        
}
