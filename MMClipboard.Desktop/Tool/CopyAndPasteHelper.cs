/*🏷️----------------------------------------------------------------
 *📄 文件名：CopyAndPasteHelper.cs
 *🏷️
 *👨🏽‍💻 创建者：Ht
 *⏱️ 创建时间：2024-02-03 17:14:04
 *🏷️----------------------------------------------------------------*/


using System;
using System.Collections.Specialized;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Clipboard = System.Windows.Clipboard;
using TextDataFormat = System.Windows.TextDataFormat;


namespace MMClipboard.Tool;

public static class CopyAndPasteHelper
{
    /// <summary>
    /// 复制并粘贴文本到其他应用
    /// </summary>
    /// <param name="content"></param>
    /// <param name="pastedAc"></param>
    public static void CopyAndPasteText(string content, Action pastedAc = null)
    {
        Clipboard.Clear();
        Clipboard.Flush();
        SharedInstance.Instance.isCopyFromSelf = true;
        try
        {
            Clipboard.SetText(content, TextDataFormat.UnicodeText);
        }
        catch (Exception e)
        {
            e.Log();
        }
        PasteToOtherApps(pastedAc);
    }

    /// <summary>
    /// 复制并粘贴文件到其他应用
    /// </summary>
    /// <param name="path"></param>
    /// <param name="pastedAc"></param>
    public static void CopyAndPasteFile(string path, Action pastedAc = null)
    {
        Clipboard.Clear();
        Clipboard.Flush();
        SharedInstance.Instance.isCopyFromSelf = true;
        try
        {
            if (!File.Exists(path))
                return;
            StringCollection sc = [path];
            Clipboard.SetFileDropList(sc);
        }
        catch (Exception e)
        {
            e.Debug();
        }
        PasteToOtherApps(pastedAc);
    }

    /// <summary>
    /// 模拟Ctrl+V，粘贴内容到其他应用
    /// </summary>
    /// <param name="pastedAc"></param>
    private static async void PasteToOtherApps(Action pastedAc)
    {
        await Task.Run(() =>
        {
            SendKeys.SendWait("^v");
        });
        pastedAc?.Invoke();
        SharedInstance.Instance.isCopyFromSelf = false;
    }
}