/*🏷️----------------------------------------------------------------
 *📄 文件名：AutoStart.cs
 *🏷️
 *👨🏽‍💻 创建者：Ht
 *⏱️ 创建时间：2023-12-22 10:01:45
 *🏷️----------------------------------------------------------------*/


using System;
using System.IO;
using System.Linq;
using Microsoft.Win32;


namespace HtKit;

public abstract class AutoStart
{
    #region 将程序快捷方式添加到系统启动目录，不需要管理员权限

    /// <summary>
    /// 系统自动启动目录
    /// </summary>
    private static string systemStartPath => Environment.GetFolderPath(Environment.SpecialFolder.Startup);

    /// <summary>
    /// 程序完整路径
    /// </summary>
    private static string appExePath => AppPath.GetApplicationExePath();

    /// <summary>
    /// 设置开机启动
    /// </summary>
    /// <param name="name">快捷方式名称</param>
    /// <param name="desc">快捷方式描述</param>
    /// <param name="start">自启开关</param>
    public static void SetAutoStart(string name, string desc, bool start = true)
    {
        var p = Path.Combine(systemStartPath, $"{name}.lnk");
        if (File.Exists(p)) DeleteFile(p);
        if (start) QuickShort.CreateShortcut(systemStartPath, name, appExePath, desc);
    }

    /// <summary>
    /// 删除文件
    /// </summary>
    /// <param name="path">路径</param>
    private static void DeleteFile(string path)
    {
        var attr = File.GetAttributes(path);
        if (attr == FileAttributes.Directory)
            Directory.Delete(path, true);
        else
            File.Delete(path);
    }

    #endregion 将程序快捷方式添加到系统启动目录，不需要管理员权限

    #region 修改注册表，需要管理员权限

    /// <summary>
    /// 将本程序设为开启自启
    /// </summary>
    /// <param name="onOff">自启开关</param>
    /// <returns></returns>
    public static bool SetStart(bool onOff)
    {
        // var appName = Process.GetCurrentProcess().MainModule?.ModuleName;
        var appPath = Environment.ProcessPath;
        return SelfRunning(onOff, "MMClipboard", appPath);
    }

    /// <summary>
    /// 判断注册键值对是否存在，即是否处于开机启动状态
    /// </summary>
    /// <param name="keyName">键值名</param>
    /// <returns></returns>
    private static bool IsExistKey(string keyName)
    {
        try
        {
            var local = Registry.LocalMachine;
            var runs = local.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
            if (runs == null)
            {
                var key2 = local.CreateSubKey("SOFTWARE");
                var key3 = key2?.CreateSubKey("Microsoft");
                var key4 = key3?.CreateSubKey("Windows");
                var key5 = key4?.CreateSubKey("CurrentVersion");
                var key6 = key5?.CreateSubKey("Run");
                runs = key6;
            }
            var runsName = runs?.GetValueNames();
            if (runsName != null)
                if (runsName.Any(strName => strName == keyName))
                    return true;
            local.Close();
            return false;
        }
        catch (Exception e)
        {
            e.Debug();
            return false;
        }
    }

    /// <summary>
    /// 写入或删除注册表键值对,即设为开机启动或开机不启动
    /// </summary>
    /// <param name="isStart">是否开机启动</param>
    /// <param name="exeName">应用程序名</param>
    /// <param name="path">应用程序路径带程序名</param>
    /// <returns></returns>
    private static bool SelfRunning(bool isStart, string exeName, string path)
    {
        try
        {
            var local = Registry.LocalMachine;
            var key = local.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
            if (key == null) local.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
            //若开机自启动则添加键值对
            if (isStart)
            {
                key?.SetValue(exeName, "\"" + path + "\" -autorun");
                key?.Close();
            }
            else //否则删除键值对
            {
                if (IsExistKey(exeName))
                {
                    key?.DeleteValue(exeName);
                    key?.Close();
                }
            }
        }
        catch (Exception ex)
        {
            ex.Message.Log();
            return false;
            //throw;
        }

        return true;
    }

    #endregion 修改注册表，需要管理员权限
}