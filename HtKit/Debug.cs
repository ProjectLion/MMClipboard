/*🏷️----------------------------------------------------------------
 *📄 文件名：Debug.cs
 *🏷️
 *👨🏽‍💻 创建者：Ht
 *⏱️ 创建时间：2023-12-22 10:01:45
 *🏷️----------------------------------------------------------------*/


using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;


namespace HtKit
{
    public static class Debug
    {
        private static string LogDirPath
        {
            get
            {
                var p = AppPath.GetBaseDirectory(@"Logs");
                if (!Directory.Exists(p))
                    Directory.CreateDirectory(p!);
                return p;
            }
        }

        public static void Log(object msg,
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int num = 0,
            [CallerMemberName] string name = "", bool isLog = false)
        {
            System.Diagnostics.Debug.WriteLine($"在 {filePath}, 第 {num} 行, {name} 方法中, Log: ↓↓↓\n{msg}");
#if RELEASE
            if (isLog)
            {
                WriteLog(msg);
            }
#endif
        }

        // 写入本地日志文件
        private static void WriteLog(object msg)
        {
            try
            {
                var now = DateTime.Now;
                var p = Path.Combine(LogDirPath, $"{now:yyyy-MM-dd}.txt");
                if (!File.Exists(p))
                    File.Create(p).Close();
                File.AppendAllText(p, $"{now:HH:mm:ss.fff} {msg}\n");
            }
            catch (Exception ex)
            {
                ex.Log();
            }
        }
    }
}

public static class DebugExtension
{
    public static ICollection DebugAny(this ICollection target,
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int num = 0,
        [CallerMemberName] string name = "")
    {
        var str = target.Cast<object>().Aggregate("", (current, item) => current + $"{item}\n");
        HtKit.Debug.Log(str[..^1], filePath, num, name);
        return target;
    }

    public static T Debug<T>(this T target,
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int num = 0,
        [CallerMemberName] string name = "")
    {
        HtKit.Debug.Log(target, filePath, num, name);
        return target;
    }

    public static T Log<T>(this T target,
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int num = 0,
        [CallerMemberName] string name = "")
    {
        HtKit.Debug.Log(target, filePath, num, name, true);
        return target;
    }
}