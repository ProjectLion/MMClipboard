/*🏷️----------------------------------------------------------------
 *📄 文件名：AppPath.cs
 *🏷️
 *👨🏽‍💻 创建者：Ht
 *⏱️ 创建时间：2023-12-22 10:01:45
 *🏷️----------------------------------------------------------------*/


using System;
using System.IO;


namespace HtKit;

public static class AppPath
{
    /// <summary>
    /// 获取程序目录(尾部带斜杠)
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string GetBaseDirectory(string path = "")
    {
        return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"MMClipboard\\{path}");
    }

    /// <summary>
    /// 获取程序exe文件完整路径
    /// </summary>
    /// <returns></returns>
    public static string GetApplicationExePath()
    {
        return Environment.ProcessPath;
    }

    /// <summary>
    /// 获取系统目录文件夹
    /// </summary>
    /// <param name="folder"></param>
    /// <returns></returns>
    public static string GetSystemPath(Environment.SpecialFolder folder)
    {
        return Environment.GetFolderPath(folder);
    }
}