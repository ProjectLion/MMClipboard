/*🏷️----------------------------------------------------------------
 *📄 文件名：App.xaml
 *🏷️
 *👨🏽‍💻 创建者：Ht
 *⏱️ 创建时间：2023/8/15 12:17:56
 *🏷️----------------------------------------------------------------*/


using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Hardcodet.Wpf.TaskbarNotification;
using HtKit;
using MMClipboard.Tool;
using MMClipboard.UserConfigs;
using MMClipboard.View;


namespace MMClipboard;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
    public TaskbarIcon taskbarIcon;

    private Mutex AppMutex;

    private const int InstanceMsg = 0x9823;

    protected override void OnStartup(StartupEventArgs e)
    {
        AppMutex = new Mutex(true, "com.ht.mmClipboard", out var createdNew);
        if (!createdNew)
        {
            var current = Process.GetCurrentProcess();
            foreach (var process in Process.GetProcessesByName(current.ProcessName))
            {
                if (process.Id == current.Id) continue;
                if (process.MainWindowHandle != 0)
                {
                    Win32Api.ShowWindow(process.MainWindowHandle, Win32Api.SW_NORMAL);
                }
                else
                {
                    var hWndPtr = Win32Api.FindWindow(null, "HookMsgWd");
                    _ = Win32Api.SendMessage(hWndPtr, InstanceMsg, IntPtr.Zero, IntPtr.Zero);
                }
                break;
            }
            // 退出当前实例程序
            Environment.Exit(0);
        }
        else
        {
#if DEBUG
            RunSelf();
#else
            var identity = System.Security.Principal.WindowsIdentity.GetCurrent();
            System.Security.Principal.WindowsPrincipal principal = new(identity);

            if (principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator))
            {
                RunSelf();
            }
            else
            {
                var startInfo = new ProcessStartInfo(Environment.ProcessPath ?? "")
                {
                    UseShellExecute = true,
                    Verb = "runas"
                };
                try
                {
                    Process.Start(startInfo);
                }
                catch (Exception ex)
                {
                    ex.Message.Log();
                }
                Current.Shutdown();
            }
#endif
        }
    }

    protected override void OnExit(ExitEventArgs e)
    {
        base.OnExit(e);
        DataBaseController.Close();
        if (!SharedInstance.Instance.isNeedUpdate)
            return;
        var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UpdateTool.exe");
        if (!File.Exists(path)) return;
        var info = new ProcessStartInfo(path)
        {
            Arguments = $"com.ht.mmclipboard"
        };
        Process.Start(info);
    }

    private void RunSelf()
    {
        // 主线程异常
        DispatcherUnhandledException += App_DispatcherUnhandledException;
        //Task异常
        TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        //非UI线程未捕获异常处理事件(例如自己创建的一个子线程)
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        MemoryHelper.FlushMemory();
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        taskbarIcon = (TaskbarIcon)FindResource("Taskbar");

        // 读取用户配置
        UserConfig.ReadConfig();
        AutoStart.SetAutoStart("妙剪记", "妙剪记", UserConfig.Default.config.isAutoStart);

        var a = new SysHookWindow()
        {
            Visibility = Visibility.Hidden
        };
        a.Show();

        if (!UserConfig.Default.config.isStartMinimize)
            SharedInstance.ShowSettingWindow();
    }

    private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        try
        {
            if (e.ExceptionObject is not Exception exception) return;
            // 处理未处理的异常
            //SharedInstance.ShowAlert($"发生未处理的异常：{sender}\n{exception}");
            exception.Message.Log();
            if (exception is TypeInitializationException initializationException) initializationException.InnerException?.Message.Log();
        }
        catch (Exception)
        {
            // ignored
        }
    }

    private static void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
    {
        // 处理未处理的异常
        //SharedInstance.ShowAlert($"发生未处理的异常：{sender}\n{e.Exception.Message}");
        e.Exception.Message.Log();
        e.SetObserved();
    }

    private static void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        // 处理未处理的异常
        //SharedInstance.ShowAlert($"发生未处理的异常：{sender}\n{e.Exception.Message}");
        // 可以选择标记异常已处理，以阻止应用程序崩溃

        e.Exception.Message.Log();
        // if (e.Exception is TypeInitializationException)
        // {
        //     (e.Exception as TypeInitializationException).InnerException.Message.Log();
        //     (e.Exception as TypeInitializationException).StackTrace.Log();
        //     (e.Exception as TypeInitializationException).Source.Log();
        //     (e.Exception as TypeInitializationException).Message.Log();
        // }
#if DEBUG
        e.Handled = false;
#else
        e.Handled = true;
#endif
    }
}