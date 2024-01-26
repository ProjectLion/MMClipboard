/*🏷️----------------------------------------------------------------
 *📄 文件名：MemoryHelper.cs
 *🏷️
 *👨🏽‍💻 创建者：Ht
 *⏱️ 创建时间：2023-12-22 10:01:45
 *🏷️----------------------------------------------------------------*/


using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Win32;


namespace HtKit;

public class MemoryHelper
{
    private void SetDate()
    {
        CreateKey();
        var currentUser = Registry.CurrentUser;
        var registryKey = currentUser.OpenSubKey("SOFTWARE\\DevExpress\\Components", true);
        registryKey!.GetValue("LastAboutShowedTime");
        var value = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
        registryKey!.SetValue("LastAboutShowedTime", value);
        currentUser.Dispose();
    }

    [DllImport("kernel32.dll")]
    private static extern bool SetProcessWorkingSetSize(IntPtr proc, int min, int max);

    public static void FlushMemory()
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        if (Environment.OSVersion.Platform == PlatformID.Win32NT) SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle, -1, -1);
    }

    private static void CreateKey()
    {
        var currentUser = Registry.CurrentUser;
        if (currentUser.OpenSubKey(@"SOFTWARE\DevExpress\Components", true) == null)
        {
            var registryKey = currentUser.CreateSubKey(@"SOFTWARE\DevExpress\Components");
            if (registryKey is null) return;
            registryKey.CreateSubKey("LastAboutShowedTime")?.SetValue("LastAboutShowedTime", DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));
            registryKey.CreateSubKey("DisableSmartTag")?.SetValue("LastAboutShowedTime", false);
            registryKey.CreateSubKey("SmartTagWidth")?.SetValue("LastAboutShowedTime", 350);
        }

        currentUser.Dispose();
    }

    // public void Cracker(int sleepSpan = 30)
    // {
    //     Task.Factory.StartNew(delegate
    //     {
    //         while (true)
    //             try
    //             {
    //                 SetDate();
    //                 FlushMemory();
    //                 Thread.Sleep(TimeSpan.FromSeconds(sleepSpan));
    //             }
    //             catch (Exception)
    //             {
    //                 // ignored
    //             }
    //     });
    // }
}