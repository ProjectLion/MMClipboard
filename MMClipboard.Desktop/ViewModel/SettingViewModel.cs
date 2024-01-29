/*🏷️----------------------------------------------------------------
 *📄 文件名：SettingViewModel.cs
 *🏷️
 *👨🏽‍💻 创建者：Ht
 *⏱️ 创建时间：2023/12/18 12:17:56
 *🏷️----------------------------------------------------------------*/


using System;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using HtKit;
using MMClipboard.Tool.AppUpdate;
using MMClipboard.UserConfigs;
using MMClipboard.View;


namespace MMClipboard.ViewModel;

public class SettingViewModel : ObservableObject
{
    // 是否开机启动
    public bool isAutoStart
    {
        get => _isAutoStart;
        set
        {
            UserConfig.Default.config.isAutoStart = value;
            AutoStart.SetAutoStart("妙剪记", "妙剪记", value);
            UserConfig.SaveConfig();
            SetProperty(ref _isAutoStart, value);
        }
    }
    private bool _isAutoStart = UserConfig.Default.config.isAutoStart;

    // 启动后最小化
    public bool isStartMinimize
    {
        get => _isStartMinimize;
        set
        {
            UserConfig.Default.config.isStartMinimize = value;
            UserConfig.SaveConfig();
            SetProperty(ref _isStartMinimize, value);
        }
    }
    private bool _isStartMinimize = UserConfig.Default.config.isStartMinimize;

    // 是否复制文件
    // public bool isCopyFile
    // {
    //     get => _isCopyFile;
    //     set
    //     {
    //         UserConfig.Default.config.isCopyFile = value;
    //         UserConfig.SaveConfig();
    //         SetProperty(ref _isCopyFile, value);
    //     }
    // }
    // private bool _isCopyFile = UserConfig.Default.config.isCopyFile;

    // 是否为小窗口
    public bool isSmall
    {
        get => _isSmall;
        set
        {
            UserConfig.Default.config.isSmall = value;
            UserConfig.SaveConfig();
            SharedInstance.ChangeWindow(value);
            SetProperty(ref _isSmall, value);
        }
    }
    private bool _isSmall = UserConfig.Default.config.isSmall;

    // 复制完成后关闭
    public bool isCopiedClose
    {
        get => _isCopiedClose;
        set
        {
            UserConfig.Default.config.isCopiedClose = value;
            UserConfig.SaveConfig();
            SetProperty(ref _isCopiedClose, value);
        }
    }
    private bool _isCopiedClose = UserConfig.Default.config.isCopiedClose;

    // 是否使用背景图片
    public bool isUseBackgroundImg
    {
        get => _isUseBackgroundImg;
        set
        {
            UserConfig.Default.config.isUseBackgroundImg = value;
            SharedInstance.Instance.backgroundChangeAction?.Invoke(value);
            backgroundImgVisibility = value ? Visibility.Visible : Visibility.Collapsed;
            UserConfig.SaveConfig();
            SetProperty(ref _isUseBackgroundImg, value);
        }
    }
    private bool _isUseBackgroundImg = UserConfig.Default.config.isUseBackgroundImg;

    public BitmapImage backgroundImg
    {
        get => _backgroundImg;
        set => SetProperty(ref _backgroundImg, value);
    }
    private BitmapImage _backgroundImg = UserConfig.Default.config.bgImgSource;

    public Visibility backgroundImgVisibility
    {
        get => _backgroundImgVisibility;
        set => SetProperty(ref _backgroundImgVisibility, value);
    }
    private Visibility _backgroundImgVisibility = UserConfig.Default.config.isUseBackgroundImg ? Visibility.Visible : Visibility.Collapsed;

    public Color selectColor
    {
        get => _selectColor;
        set
        {
            solidColorBrush = new SolidColorBrush(value);
            SharedInstance.Instance.backgroundColorChangeAction?.Invoke(solidColorBrush);
            UserConfig.Default.config.pageBackgroundColor = value.ToString();
            UserConfig.SaveConfig();
            SetProperty(ref _selectColor, value);
        }
    }
    private Color _selectColor = HtColor.ColorWithHex(UserConfig.Default.config.pageBackgroundColor);

    public SolidColorBrush solidColorBrush
    {
        get => _solidColorBrush;
        private set => SetProperty(ref _solidColorBrush, value);
    }
    private SolidColorBrush _solidColorBrush = HtColor.GetBrushWithString(UserConfig.Default.config.pageBackgroundColor);

    /// <summary>
    /// 本地版本号
    /// </summary>
    public string localVersion
    {
        get => _localVersion;
        private set => SetProperty(ref _localVersion, value);
    }
    private string _localVersion;

    /// <summary>
    /// 今年
    /// </summary>
    public static int thisYear => DateTime.Today.Year;

    /// <summary>
    /// 更新按钮标题
    /// </summary>
    public string updateBtnTitle
    {
        get => _updateBtnTitle;
        private set => SetProperty(ref _updateBtnTitle, value);
    }
    private string _updateBtnTitle = "检查更新";

    public string hotkeyString
    {
        get => _hotkeyString;
        private set => SetProperty(ref _hotkeyString, value);
    }
    private string _hotkeyString;

    private VersionModel m_versionDataModel;

    private bool m_isNeedUpdate;

    /// <summary>
    /// 构造函数
    /// initialization
    /// </summary>
    public SettingViewModel()
    {
        localVersion = AppUpdateManager.ReadLocalVersionNo();
        StringBuilder shortcutText = new();
        var m = UserConfig.Default.config.modifierKeys.ToString().Replace(',', '+');
        shortcutText.Append($"{m}+");
        shortcutText.Append(UserConfig.Default.config.key.ToString());
        hotkeyString = shortcutText.ToString();
        CheckUpdate();
    }

    /// <summary>
    /// 选择背景图片
    /// Choose the background image
    /// </summary>
    /// <param name="fileP"></param>
    public void ChooseBackgroundImg(string fileP)
    {
        try
        {
            var bmp = new BitmapImage(new Uri(fileP));
            backgroundImg = bmp;
            SharedInstance.Instance.backgroundImageChangeAction?.Invoke(backgroundImg);
            UserConfig.Default.config.pageBackgroundImg = fileP;
            UserConfig.SaveConfig();
        }
        catch (Exception e)
        {
            e.Message.Debug();
        }
    }

    /// <summary>
    /// App更新
    /// App updates
    /// </summary>
    public void Update()
    {
        if (m_isNeedUpdate)
            OpenUpdateWindow();
        else
            CheckUpdate();
    }

    /// <summary>
    /// App检查更新
    /// The App checks for updates
    /// </summary>
    public void CheckUpdate()
    {
        AppUpdateManager.Check((update, mod) =>
        {
            m_versionDataModel = mod;
            m_isNeedUpdate = update;
            updateBtnTitle = update ? $"有新版本：{mod.version}" : $"不需要更新：{mod.version}";
        });
    }

    /// <summary>
    /// 打开更新窗口
    /// Open the update window
    /// </summary>
    private void OpenUpdateWindow()
    {
        AppUpdateWindow updateWindow = new(m_versionDataModel)
        {
            ShowActivated = true
        };
        updateWindow.ShowDialog();
    }
}