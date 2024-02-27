/*🏷️----------------------------------------------------------------
 *📄 文件名：OpenExternalWindowHelper.cs
 *🏷️
 *👨🏽‍💻 创建者：Ht
 *⏱️ 创建时间：2024-02-04 11:14:42
 *🏷️----------------------------------------------------------------*/


using System;
using System.Diagnostics;
using System.IO;


namespace MMClipboard.Tool;

/// <summary>
/// 打开外部窗口的工具类
/// </summary>
public static class OpenExternalWindowHelper
{
    /// <summary>
    /// 打开网页
    /// </summary>
    /// <param name="url"></param>
    public static void OpenWebsite(string url)
    {
        try
        {
            Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
        }
        catch (Exception e)
        {
            e.Message.Log();
        }
    }

    /// <summary>
    /// 打开文件夹并选中文件
    /// </summary>
    /// <param name="path"></param>
    public static void SelectFileInFolder(string path)
    {
        if (!File.Exists(path))
            return;
        try
        {
            Process.Start("explorer.exe", $"/select,\"{path}\"");
        }
        catch (Exception e)
        {
            e.Message.Log();
        }
    }

    /// <summary>
    /// 打开文件夹
    /// </summary>
    /// <param name="path"></param>
    public static void OpenFolder(string path)
    {
        if (!Directory.Exists(path))
            return;
        try
        {
            Process.Start("explorer.exe", path);
        }
        catch (Exception e)
        {
            e.Message.Log();
        }
    }
}