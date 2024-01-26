/*🏷️----------------------------------------------------------------
 *📄 文件名：QuickShort.cs
 *🏷️
 *👨🏽‍💻 创建者：Ht
 *⏱️ 创建时间：2023-12-22 10:01:45
 *🏷️----------------------------------------------------------------*/


using System;
using System.IO;
using IWshRuntimeLibrary;
using File = System.IO.File;


namespace HtKit;

/// <summary>
/// 快捷方式相关
/// </summary>
public static class QuickShort
{
    /// <summary>
    /// 自动获取桌面目录
    /// </summary>
    private static string desktopPath => Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

    /// <summary>
    /// 在桌面上创建快捷方式
    /// </summary>
    /// <param name="quickName">快捷方式名称</param>
    public static void CreateDesktopQuick(string quickName = "")
    {
        var p = Path.Combine(desktopPath, $"{quickName}.lnk");
        if (File.Exists(p)) File.Delete(p);
        CreateShortcut(desktopPath, quickName, AppPath.GetApplicationExePath());
    }

    /// <summary>
    ///  向目标路径创建指定文件的快捷方式
    /// </summary>
    /// <param name="directory">目标目录</param>
    /// <param name="shortcutName">快捷方式名字</param>
    /// <param name="targetPath">文件完全路径</param>
    /// <param name="description">描述</param>
    /// <param name="iconLocation">图标地址</param>
    /// <returns>成功或失败</returns>
    public static bool CreateShortcut(string directory, string shortcutName, string targetPath, string description = null, string iconLocation = null)
    {
        if (string.IsNullOrEmpty(directory)) return false;
        try
        {
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory); //目录不存在则创建
            //添加引用 Com 中搜索 Windows Script Host Object Model
            var shortcutPath = Path.Combine(directory, $"{shortcutName}.lnk"); //合成路径
            var shell = new WshShell();
            var shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath); //创建快捷方式对象
            shortcut.TargetPath = targetPath; //指定目标路径
            shortcut.WorkingDirectory = Path.GetDirectoryName(targetPath); //设置起始位置
            shortcut.WindowStyle = 1; //设置运行方式，默认为常规窗口
            shortcut.Description = description; //设置备注
            shortcut.IconLocation = string.IsNullOrWhiteSpace(iconLocation) ? targetPath : iconLocation; //设置图标路径
            shortcut.Save(); //保存快捷方式
            return true;
        }
        catch (Exception ex)
        {
            ex.Message.Debug();
        }
        return false;
    }
}