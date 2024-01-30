/*🏷️----------------------------------------------------------------
 *📄 文件名：SysHookWindow.xaml.cs
 *🏷️
 *👨🏽‍💻 创建者：Ht
 *⏱️ 创建时间：2023-12-22 10:01:45
 *🏷️----------------------------------------------------------------*/


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Input;
using System.Windows.Interop;
using MMClipboard.Model;
using MMClipboard.Tool;
using MMClipboard.UserConfigs;
using static HtKit.Win32Api;
using Clipboard = System.Windows.Clipboard;
using TextDataFormat = System.Windows.TextDataFormat;


namespace MMClipboard.View;

/// <summary>
/// SysHookWindow.xaml 的交互逻辑
/// </summary>
public partial class SysHookWindow
{
    private const int AltCKeyEventId = 0x2884;

    private IntPtr m_HWndNext;

    // 记录上一次复制的内容，避免重复保存数据
    private string oldClipContent;
    private nint selfHandle;

    public SysHookWindow()
    {
        InitializeComponent();
        SharedInstance.Instance.registerHotKeyAction = (m, k) =>
        {
            UnregisterHotKey(selfHandle, AltCKeyEventId);
            RegisterHot(m, k);
        };
    }

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, nint fsModifiers, int vk);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    // 将窗口添加到剪贴板格式侦听器列表.
    // 微软文档：每当剪贴板的内容发生更改时，该窗口将发布 WM_CLIPBOARDUPDATE: 0x031D 消息。
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern bool AddClipboardFormatListener(IntPtr hWnd);

    // 从系统维护的剪贴板格式侦听器列表中删除给定窗口。
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern bool RemoveClipboardFormatListener(IntPtr hWnd);

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);
        selfHandle = new WindowInteropHelper(this).Handle;
        var source = HwndSource.FromHwnd(selfHandle);
        if (AddClipboardFormatListener(selfHandle))
            "剪切板监听成功".Log();

        source?.AddHook(HwndHook);
        //真正注册快捷键监听处理: 同时注册数字键和小键盘的CTRL+5
        //RegisterHotKey(handle, Ctrl5KeyEventId, (uint)ModifierKeys.Control, (uint)KeyInterop.VirtualKeyFromKey(Key.D5));
        //RegisterHotKey(handle, Ctrl5KeyEventId, (uint)ModifierKeys.Control, (uint)KeyInterop.VirtualKeyFromKey(Key.NumPad5));
        RegisterHot(UserConfig.Default.config.modifierKeys, UserConfig.Default.config.key);
    }

    // 注册快捷键
    private void RegisterHot(ModifierKeys modifierKeys, Key key)
    {
        var m = (int)modifierKeys;
        RegisterHotKey(selfHandle, AltCKeyEventId, m, KeyInterop.VirtualKeyFromKey(key));
    }

    // 窗口消息处理
    private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        const int WM_Hotkey = 0x0312;
        const int WM_CLIPBOARDUPDATE = 0x031D;
        const int InstanceMsg = 0x9823;
        switch (msg)
        {
            case WM_CLIPBOARDUPDATE:
                // 剪切板内容变化，微软文档中说，这个消息在剪切板内容变化时发送
                ClipboardContentChanged();
                break;
            case WM_Hotkey: // 通过RegisterHotKey注册的热键
            {
                HotkeyAction(wParam);
                break;
            }
            case InstanceMsg: // 单例应用消息
            {
                SharedInstance.ShowMainWindow();
                break;
            }
        }

        return IntPtr.Zero;
    }

    private static void HotkeyAction(IntPtr wParam)
    {
        switch (wParam.ToInt32())
        {
            case AltCKeyEventId:
            {
                if (!SharedInstance.Instance.isRecordingShortcutKey)
                    SharedInstance.ShowMainWindow();
            }
                break;
        }
    }

    // 剪切板内容变化
    private void ClipboardContentChanged()
    {
        if (SharedInstance.Instance.isCopyFromSelf)
            return;
        SharedInstance.Instance.isCopyFromSelf = false;
        try
        {
            var hWnd = GetForegroundWindow(); //获取活动窗口句柄
            _ = GetWindowThreadProcessId(hWnd, out var calcID);
            using var clipProcess = Process.GetProcessById(calcID);

            var clipContent = $"{clipProcess.ProcessName}:{calcID}:";

            var now = DateTime.Now;

            var dataArr = new List<ClipItemModel>();

            if (Clipboard.ContainsText())
            {
                /*
                    UnicodeText = 1,
                    Rtf = 2,
                    Html = 3,
                    CommaSeparatedValue = 4,
                    Xaml = 5
                 */
                var text = Clipboard.GetText(TextDataFormat.UnicodeText);
                clipContent += text;
                var mod = new ClipItemModel()
                {
                    from = clipProcess.ProcessName,
                    fromExeImgPath = clipProcess.MainModule != null ? clipProcess.MainModule.FileName : string.Empty,
                    date = now,
                    rtfContent = string.Empty,
                    clipType = ClipType.Text,
                    content = text
                };

                // win11的资源管理器右键复制的文件地址会被双引号包着，将引号去掉
                if (clipProcess.ProcessName == "explorer" && text.First() == '"' && text.Last() == '"')
                    mod.content = text.Trim('"');

                if (Clipboard.ContainsText(TextDataFormat.Rtf))
                {
                    var rtfText = Clipboard.GetText(TextDataFormat.Rtf);
                    mod.rtfContent = rtfText;
                }
                dataArr.Add(mod);
            }
            else if (Clipboard.ContainsFileDropList())
            {
                var files = Clipboard.GetFileDropList().Cast<string>().ToArray();
                foreach (var item in files)
                {
                    var mod = new ClipItemModel()
                    {
                        from = clipProcess.ProcessName,
                        date = now,
                        fromExeImgPath = clipProcess.MainModule != null ? clipProcess.MainModule.FileName : string.Empty,
                        rtfContent = string.Empty,
                        clipType = item.Ht_IsImage() ? ClipType.Image : ClipType.File,
                        content = item
                    };
                    clipContent += item;
                    dataArr.Add(mod);
                }
            }
            else if (Clipboard.ContainsImage())
            {
                var img = Clipboard.GetImage();
                if (img != null)
                {
                    clipContent += $"W:{img.PixelWidth}, H:{img.PixelHeight}";
                    var mod = new ClipItemModel
                    {
                        from = clipProcess.ProcessName,
                        date = now,
                        fromExeImgPath = clipProcess.MainModule != null ? clipProcess.MainModule.FileName : string.Empty,
                        rtfContent = string.Empty,
                        clipType = ClipType.Image,
                        content = CacheHelper.SaveImage(img, now)
                    };
                    dataArr.Add(mod);
                }
            }
            // 复制的内容是重复的就return
            if (clipContent == oldClipContent || dataArr.Count == 0)
                return;
            oldClipContent = clipContent;
            if (DataBaseController.AddDataFromList(dataArr))
                SharedInstance.Instance.reloadDataAction?.Invoke();
        }
        catch (Exception e)
        {
            e.Message.Debug();
        }
    }

    private void Window_Closed(object sender, EventArgs e)
    {
        if (RemoveClipboardFormatListener(selfHandle))
            "剪切板取消监听成功".Log();
    }
}