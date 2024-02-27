/*🏷️----------------------------------------------------------------
 *📄 文件名：ClipboardListener.cs
 *🏷️
 *👨🏽‍💻 创建者：Ht
 *⏱️ 创建时间：2024-02-25 18:19:38
 *🏷️----------------------------------------------------------------*/


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using HtKit;
using MMClipboard.Model;


namespace MMClipboard.Tool;

public static class ClipboardListener
{
    internal static readonly IntPtr HwndMessage = -3;

    // 将窗口添加到剪贴板格式侦听器列表.
    // 微软文档：每当剪贴板的内容发生更改时，该窗口将发布 WM_CLIPBOARDUPDATE: 0x031D 消息。
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern bool AddClipboardFormatListener(IntPtr hWnd);

    // 从系统维护的剪贴板格式侦听器列表中删除给定窗口。
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern bool RemoveClipboardFormatListener(IntPtr hWnd);

    private static HwndSource source;

    private static string oldClipContent;

    public static void Add()
    {
        var parameters = new HwndSourceParameters("mmclipboard hook")
        {
            HwndSourceHook = HwndHook,
            ParentWindow = HwndMessage
        };
        source ??= new HwndSource(parameters);
        if (AddClipboardFormatListener(source.Handle))
            "剪切板监听成功".Log();
        source.AddHook(HwndHook);
    }

    public static void Remove()
    {
        RemoveClipboardFormatListener(source.Handle);
    }

    private static IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        const int WM_CLIPBOARDUPDATE = 0x031D;
        switch (msg)
        {
            case WM_CLIPBOARDUPDATE:
                // 剪切板内容变化，微软文档中说，这个消息在剪切板内容变化时发送
                "剪切板内容变化，微软文档中说，这个消息在剪切板内容变化时发送".Debug();
                ClipboardContentChanged();
                break;
        }
        return IntPtr.Zero;
    }

    private static void ClipboardContentChanged()
    {
        // 如果是当前App做的复制粘贴操作则不复制。
        if (SharedInstance.Instance.isCopyFromSelf)
            return;
        try
        {
            var hWnd = Win32Api.GetForegroundWindow(); //获取活动窗口句柄
            _ = Win32Api.GetWindowThreadProcessId(hWnd, out var calcID);
            using var clipProcess = Process.GetProcessById(calcID);
            if (Clipboard.ContainsText())
                SaveText(clipProcess);
            else if (Clipboard.ContainsFileDropList())
                SaveFile(clipProcess);
            else if (Clipboard.ContainsImage())
                SaveImage(clipProcess);
            SharedInstance.Instance.isCopyFromSelf = false;
        }
        catch (Exception e)
        {
            e.Message.Debug();
        }
    }

    /// <summary>
    /// 保存文本
    /// </summary>
    /// <param name="clipProcess"></param>
    private static void SaveText(Process clipProcess)
    {
        /*
                    UnicodeText = 1,
                    Rtf = 2,
                    Html = 3,
                    CommaSeparatedValue = 4,
                    Xaml = 5
                 */
        var text = Clipboard.GetText(TextDataFormat.UnicodeText);
        if (oldClipContent == text)
            return;
        var now = DateTime.Now;
        oldClipContent = text;
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
        if (DataBaseController.AddHistoryDataFromList([mod]))
            SharedInstance.Instance.reloadDataAction?.Invoke();
    }

    /// <summary>
    /// 保存图片
    /// </summary>
    /// <param name="clipProcess"></param>
    private static void SaveImage(Process clipProcess)
    {
        var img = Clipboard.GetImage();
        if (img == null)
            return;
        var now = DateTime.Now;
        // $"W:{img.PixelWidth}, H:{img.PixelHeight}".Debug();
        var mod = new ClipItemModel
        {
            from = clipProcess.ProcessName,
            date = now,
            fromExeImgPath = clipProcess.MainModule != null ? clipProcess.MainModule.FileName : string.Empty,
            rtfContent = string.Empty,
            clipType = ClipType.Image,
            content = CacheHelper.SaveImage(img, now)
        };
        if (DataBaseController.AddHistoryDataFromList([mod]))
            SharedInstance.Instance.reloadDataAction?.Invoke();
    }

    /// <summary>
    /// 保存文件
    /// </summary>
    /// <param name="clipProcess"></param>
    private static void SaveFile(Process clipProcess)
    {
        var files = Clipboard.GetFileDropList().Cast<string>().ToArray();
        var clipContent = "";
        var l = new List<ClipItemModel>();
        var now = DateTime.Now;
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
            l.Add(mod);
        }
        if (oldClipContent == clipContent)
            return;
        if (DataBaseController.AddHistoryDataFromList(l))
            SharedInstance.Instance.reloadDataAction?.Invoke();
    }
}