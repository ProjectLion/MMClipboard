/*🏷️----------------------------------------------------------------
 *📄 文件名：SharedInstance.cs
 *🏷️
 *👨🏽‍💻 创建者：Ht
 *⏱️ 创建时间：2023/9/22 17:31:40
 *🏷️----------------------------------------------------------------*/


using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MMClipboard.UserConfigs;
using MMClipboard.View;


namespace MMClipboard;

public sealed class SharedInstance
{
    private static volatile SharedInstance instance;
    private static readonly object lockObj = new();
    public Action<bool> backgroundChangeAction;

    public Action<SolidColorBrush> backgroundColorChangeAction;
    public Action<BitmapSource> backgroundImageChangeAction;

    /// <summary>
    /// 是否是自身复制的
    /// </summary>
    public bool isCopyFromSelf = false;

    /// <summary>
    /// 是否需要更新
    /// </summary>
    public bool isNeedUpdate = false;

    /// <summary>
    /// 是否正在录制快捷键
    /// </summary>
    public bool isRecordingShortcutKey = false;

    /// <summary>
    /// 主窗口
    /// </summary>
    public Window mainWindow;

    /// <summary>
    /// 自定义的快捷键
    /// </summary>
    public Action<ModifierKeys, Key> registerHotKeyAction;

    /// <summary>
    /// 通知主窗口刷新数据
    /// </summary>
    public Action reloadDataAction;

    /// <summary>
    /// 搜索窗口
    /// </summary>
    public SearchWindow searchWindow;

    /// <summary>
    /// 设置窗口
    /// </summary>
    public SettingWindow settingWindow;

    private SharedInstance()
    { }

    public static SharedInstance Instance
    {
        get
        {
            if (instance == null)
                lock (lockObj)
                {
                    if (null == instance) instance = new SharedInstance();
                }

            return instance;
        }
    }

    public static void Reset()
    {
        instance = null;
    }

    /// <summary>
    /// 显示主窗口
    /// </summary>
    public static void ShowMainWindow()
    {
        if (Instance.mainWindow != null)
        {
            Instance.mainWindow.Close();
            return;
        }
        if (UserConfig.Default.config.isSmall)
            ShowSmallW();
        else
            ShowMainW();
    }

    private static void ShowMainW()
    {
        MainWindow mw = new();
        Instance.mainWindow = mw;
        mw.Show();
    }

    private static void ShowSmallW()
    {
        SmallWindow sw = new();
        Instance.mainWindow = sw;
        sw.Show();
    }

    /// <summary>
    /// 显示设置窗口
    /// </summary>
    public static void ShowSettingWindow(Window owner = null)
    {
        if (Instance.settingWindow is null)
        {
            SettingWindow settingWindow = new();
            if (owner != null)
            {
                settingWindow.Owner = owner;
                settingWindow.ShowDialog();
                return;
            }
            settingWindow.Show();
        }
        else
        {
            Instance.settingWindow.Topmost = true;
            Instance.settingWindow.WindowState = WindowState.Normal;
            Instance.settingWindow.Activate();
        }
    }

    /// <summary>
    /// 在需要时切换显示窗口的类型
    /// </summary>
    /// <param name="isSmall"></param>
    public static void ChangeWindow(bool isSmall)
    {
        Instance.mainWindow?.Close();
        if (isSmall)
            ShowSmallW();
        else
            ShowMainW();
    }
}