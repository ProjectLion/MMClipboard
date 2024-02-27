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
using System.Windows.Input;
using System.Windows.Threading;
using Hardcodet.Wpf.TaskbarNotification;
using HtKit;
using MMClipboard.Tool;
using MMClipboard.UserConfigs;


namespace MMClipboard;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
    public TaskbarIcon taskbarIcon;

    private static Mutex mutex;

    private const string UniqueEventName = "com.ht.mmclipboard.StartMainEvent";

    private const string UniqueMutexName = "com.ht.mmclipboard";

    private static EventWaitHandle eventWaitHandle;

    private const int InstanceMsg = 0x9823;

    protected override void OnStartup(StartupEventArgs e)
    {
        mutex = new Mutex(true, UniqueMutexName, out var isOwned);
        // 保证mutex不会被GC回收
        GC.KeepAlive(mutex);
        eventWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset, UniqueEventName);
        if (!isOwned)
        {
            // 通知其他实例，do something...
            eventWaitHandle.Set();
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

        ClipboardListener.Remove();
        // 启动更新程序
        if (!SharedInstance.Instance.isNeedUpdate)
            return;
        var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UpdateTool.exe");
        if (!File.Exists(path)) return;
        var info = new ProcessStartInfo(path)
        {
            Arguments = "com.ht.mmclipboard"
        };
        Process.Start(info);
    }

    private void RunSelf()
    {
        // 单例应用
        var thread = new Thread(() =>
        {
            while (eventWaitHandle.WaitOne())
                Dispatcher.CurrentDispatcher.InvokeAsync(SharedInstance.ShowMainWindow);
        })
        {
            IsBackground = true
        };
        thread.Start();

        // 读取用户配置
        UserConfig.ReadConfig();

        // 注册快捷键
        HotKeyManager.RegisterHotKey_ShowMainWindow(UserConfig.Default.config.key, UserConfig.Default.config.modifierKeys);
        HotKeyManager.RegisterHotKey_ShowPhraseWindow(Key.Oem3, ModifierKeys.Alt);
        SharedInstance.Instance.registerHotKeyAction = (m, k) =>
        {
            HotKeyManager.RegisterHotKey_ShowMainWindow(k, m);
            HotKeyManager.RegisterHotKey_ShowPhraseWindow(Key.Oem3, ModifierKeys.Alt);
        };

        // 监听剪贴板事件
        ClipboardListener.Add();

        // 创建任务栏图标
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        taskbarIcon = (TaskbarIcon)FindResource("Taskbar");

        // 根据用户配置, 设置开机启动
        AutoStart.SetAutoStart("妙剪记", "妙剪记", UserConfig.Default.config.isAutoStart);

        // 根据用户配置, 显示设置窗口
        if (!UserConfig.Default.config.isStartMinimize)
            SharedInstance.ShowSettingWindow();

        #region App异常监听

        // 主线程异常
        DispatcherUnhandledException += App_DispatcherUnhandledException;
        //Task异常
        TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        //非UI线程未捕获异常处理事件(例如自己创建的一个子线程)
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

        #endregion
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