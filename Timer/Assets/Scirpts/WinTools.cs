using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

public class WinTools
{
    private static IntPtr _hWnd = IntPtr.Zero;

    public delegate bool WNDENUMPROC(IntPtr hwnd, uint lParam);

    public static class Messagebox
    {
        [DllImport("User32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
        public static extern int MessageBox(IntPtr handle, String message, String title, int type);
    }

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern bool SwitchToThisWindow(IntPtr hWnd, bool fAltTab);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool EnumWindows(WNDENUMPROC lpEnumFunc, uint lParam);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern IntPtr GetParent(IntPtr hWnd);

    [DllImport("user32.dll")]
    public static extern uint GetWindowThreadProcessId(IntPtr hWnd, ref uint lpdwProcessId);

    [DllImport("kernel32.dll", EntryPoint = "SetLastError")]
    public static extern void SetLastError(uint dwErrCode);

    private static IntPtr GetCurrentWindowHandle()
    {
        if (_hWnd == IntPtr.Zero)
        {
            uint uiPid = (uint) Process.GetCurrentProcess().Id; // 当前进程 ID
            EnumWindows(EnumWindowsProc, uiPid);
        }

        return _hWnd;
    }

    private static bool EnumWindowsProc(IntPtr hwnd, uint lParam)
    {
        uint uiPid = 0;
        if (GetParent(hwnd) == IntPtr.Zero)
        {
            GetWindowThreadProcessId(hwnd, ref uiPid);
            if (uiPid == lParam) // 找到进程对应的主窗口句柄
            {
                _hWnd = hwnd;
                SetLastError(0); // 设置无错误
                return false; // 返回 false 以终止枚举窗口
            }
        }

        return true;
    }

    public static void ShowForward()
    {
        for (int i = 0; i < 2; i++)
        {
            if (_hWnd != IntPtr.Zero)
            {
                SwitchToThisWindow(_hWnd, true);
                break;
            }

            GetCurrentWindowHandle();
        }
    }
}