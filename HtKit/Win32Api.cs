/*🏷️----------------------------------------------------------------
 *📄 文件名：Win32Api.cs
 *🏷️
 *👨🏽‍💻 创建者：Ht
 *⏱️ 创建时间：2023-12-22 10:01:45
 *🏷️----------------------------------------------------------------*/


using System;
using System.Runtime.InteropServices;


namespace HtKit;

public static class Win32Api
{
    public const int WM_KEYDOWN = 0x0100;

    [DllImport("User32.dll", EntryPoint = "PostMessage")]
    public static extern int PostMessage(
        IntPtr hWnd, //   handle   to   destination   window
        int Msg, //   message
        IntPtr wParam, //   first   message   parameter
        IntPtr lParam //   second   message   parameter
    );

    [DllImport("USER32", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
    public static extern IntPtr GetSystemMenu(IntPtr WindowHandle, int bReset);

    [DllImport("User32.dll")]
    public static extern int GetMenuItemCount(IntPtr hMenu);

    [DllImport("USER32", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
    public static extern int AppendMenuW(IntPtr MenuHandle, int Flags, int NewID, string Item);

    [DllImport("USER32", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
    public static extern int InsertMenuW(IntPtr hMenu, int Position, int Flags, int NewId, string Item);

    [DllImport("User32.dll")]
    public static extern bool EnableMenuItem(IntPtr hMenu, int uIDEnableItem, int uEnable);

    [DllImport("user32.dll")]
    public static extern bool DeleteMenu(IntPtr hMenu, uint uPosition, uint uFlags);

    [DllImport("User32.dll")]
    public static extern int DrawMenuBar(IntPtr hWnd);

    internal const uint MF_ENABLED = 0x00000000;
    internal const uint MF_GRAYED = 0x00000001;
    internal const uint MF_DISABLED = 0x00000002;
    internal const uint MF_BYCOMMAND = 0x00000000;
    internal const uint MF_BYPOSITION = 0x00000400;

    [DllImport("user32.dll")]
    public static extern IntPtr SetActiveWindow(IntPtr hWnd);

    [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
    public static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    public static extern int GetFocus();

    [DllImport("user32.dll")]
    public static extern bool SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    public static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);

    [DllImport("user32.dll", EntryPoint = "GetWindowThreadProcessId", SetLastError = true,
        CharSet = CharSet.Unicode, ExactSpelling = true,
        CallingConvention = CallingConvention.StdCall)]
    public static extern uint GetWindowThreadProcessId(IntPtr hwnd, out int ID);

    [DllImport("kernel32.dll")]
    public static extern uint GetCurrentThreadId();

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern int SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

    [DllImport("user32.dll")]
    public static extern uint GetWindowLong(IntPtr hwnd, int nIndex);

    [DllImport("user32.dll", EntryPoint = "SetWindowLong", CharSet = CharSet.Auto)]
    public static extern uint SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

    [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", CharSet = CharSet.Auto)]
    public static extern IntPtr SetWindowLongPtr64(HandleRef hWnd, int nIndex, int dwNewLong);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern long SetWindowPos(IntPtr hwnd, long hWndInsertAfter, long x, long y, long cx, long cy, long wFlags);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool MoveWindow(IntPtr hwnd, int x, int y, int cx, int cy, bool repaint);

    [DllImport("user32.dll", EntryPoint = "ostMessageA", SetLastError = true)]
    public static extern bool PostMessage(IntPtr hwnd, uint Msg, uint wParam, uint lParam);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern IntPtr GetParent(IntPtr hwnd);

    [DllImport("user32.dll", EntryPoint = "ShowWindow", SetLastError = true)]
    public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    [DllImport("User32.dll", EntryPoint = "SendMessage")]
    public static extern int SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern void SwitchToThisWindow(IntPtr hWnd, bool fAltTab);

    // IsIconic、IsZoomed ------ 分别判断窗口是否已最小化、最大化

    [DllImport("user32.dll")]
    public static extern bool IsIconic(IntPtr hWnd);

    [DllImport("user32.dll")]
    public static extern bool IsZoomed(IntPtr hWnd);

    [DllImport("user32.dll")]
    public static extern bool FlashWindow(IntPtr hWnd, bool bInvert);

    public struct POINT
    {
        public int x = 0;
        public int y = 0;

        public POINT(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static bool operator ==(POINT t1, POINT t2)
        {
            return t1.x == t2.x && t1.y == t2.y;
        }

        public static bool operator !=(POINT t1, POINT t2)
        {
            return t1.x != t2.x || t1.y != t2.y;
        }

        public override bool Equals(object obj)
        {
            throw new NotImplementedException();
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 获取鼠标指针的位置
    /// </summary>
    /// <param name="pt"></param>
    /// <returns></returns>
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern bool GetCursorPos(out POINT pt);

    public const int SWP_NOOWNERZORDER = 0x200;
    public const int SWP_NOREDRAW = 0x8;
    public const int SWP_NOZORDER = 0x4;
    public const int SWP_SHOWWINDOW = 0x0040;
    public const int WS_EX_MDICHILD = 0x40;
    public const int SWP_FRAMECHANGED = 0x20;
    public const int SWP_NOACTIVATE = 0x10;
    public const int SWP_ASYNCWINDOWPOS = 0x4000;
    public const int SWP_NOMOVE = 0x2;
    public const int SWP_NOSIZE = 0x1;
    public const int GWL_STYLE = -16;
    public const int WS_VISIBLE = 0x10000000;
    public const int WS_MAXIMIZE = 0x01000000;
    public const int WS_BORDER = 0x00800000;
    public const int WM_CLOSE = 0x10;
    public const int WS_CHILD = 0x40000000;
    public const int WS_POPUP = -2147483648;
    public const int WS_CLIPSIBLINGS = 0x04000000;

    public const int SW_HIDE = 0; //{隐藏, 并且任务栏也没有最小化图标}
    public const int SW_SHOWNORMAL = 1; //{用最近的大小和位置显示, 激活}
    public const int SW_NORMAL = 1; //{同 SW_SHOWNORMAL}
    public const int SW_SHOWMINIMIZED = 2; //{最小化, 激活}
    public const int SW_SHOWMAXIMIZED = 3; //{最大化, 激活}
    public const int SW_MAXIMIZE = 3; //{同 SW_SHOWMAXIMIZED}
    public const int SW_SHOWNOACTIVATE = 4; //{用最近的大小和位置显示, 不激活}
    public const int SW_SHOW = 5; //{同 SW_SHOWNORMAL}
    public const int SW_MINIMIZE = 6; //{最小化, 不激活}
    public const int SW_SHOWMINNOACTIVE = 7; //{同 SW_MINIMIZE}
    public const int SW_SHOWNA = 8; //{同 SW_SHOWNOACTIVATE}
    public const int SW_RESTORE = 9; //{同 SW_SHOWNORMAL}
    public const int SW_SHOWDEFAULT = 10; //{同 SW_SHOWNORMAL}
    public const int SW_MAX = 10; //{同 SW_SHOWNORMAL}

    public const int WM_SETTEXT = 0x000C;

    public const int WM_ACTIVATE = 0x0006;
    public static readonly IntPtr WA_ACTIVE = new(1);
    public static readonly IntPtr WA_INACTIVE = new(0);
}