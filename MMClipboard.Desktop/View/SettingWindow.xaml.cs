/*🏷️----------------------------------------------------------------
 *📄 文件名：SettingWindow.xaml.cs
 *🏷️
 *👨🏽‍💻 创建者：Ht
 *⏱️ 创建时间：2023/12/18 12:17:56
 *🏷️----------------------------------------------------------------*/


using System;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Input;
using HtKit;
using HtUIKit;
using MMClipboard.UserConfigs;
using MMClipboard.ViewModel;


namespace MMClipboard.View;

/// <summary>
/// SettingWindow.xaml 的交互逻辑
/// </summary>
public partial class SettingWindow
{
    public SettingWindow()
    {
        InitializeComponent();
        DataContext = new SettingViewModel();
        SharedInstance.Instance.settingWindow = this;
        updatePlaceText.Text = UserConfig.Default.config.updatePlace == 0 ? "GitHub" : "Gitee";
    }

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);
        DataContext = null;
        SharedInstance.Instance.settingWindow = null;
    }

    /// <summary>
    /// 关闭窗口
    /// Close the window
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CloseAction(object sender, RoutedEventArgs e)
    {
        Close();
        e.Handled = true;
    }

    /// <summary>
    /// 打开GitHub页面
    /// Open GitHub page
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OpenGitHubPageAction(object sender, RoutedEventArgs e)
    {
        try
        {
            Process.Start("explorer.exe", "https://github.com/ProjectLion/MMClipboard");
        }
        catch (Exception ex)
        {
            ex.Log();
        }
    }

    /// <summary>
    /// 窗口移动
    /// The window moves
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void HeaderMoveAction(object sender, MouseButtonEventArgs e)
    {
        DragMove();
    }

    /// <summary>
    /// 选择背景颜色
    /// Select a background color
    /// </summary>
    private void ChooseBGColorAction(object sender, MouseButtonEventArgs e)
    {
        colorPopup.IsOpen = true;
        e.Handled = true;
    }

    /// <summary>
    /// 选择背景图片
    /// Select a background image
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void chooseBGImageBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        ChooseFileManager.SelectImageFile((fileP) =>
        {
            if (string.IsNullOrEmpty(fileP)) return;
            (DataContext as SettingViewModel)?.ChooseBackgroundImg(fileP);
        }, false);
        e.Handled = true;
    }

    /// <summary>
    /// 录制快捷键
    /// Record the shortcut key
    /// </summary>
    private void ShortcutKeyRecordAction(object sender, MouseButtonEventArgs e)
    {
        shortcutKeyText.Text = string.Empty;
        SharedInstance.Instance.isRecordingShortcutKey = true;
    }

    /// <summary>
    /// 监听快捷键录制
    /// Listen for the recording of the shortcut key
    /// </summary>
    /// <param name="e"></param>
    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        e.Handled = true;
        if (!SharedInstance.Instance.isRecordingShortcutKey) return;
        if (e.Key == Key.Enter)
        {
            SharedInstance.Instance.isRecordingShortcutKey = false;
            //UpdateShortStr();
        }
        else
        {
            var key = e.Key == Key.System ? e.SystemKey : e.Key;
            // Ignore modifier keys.
            if (key is Key.LeftShift or Key.RightShift or Key.LeftCtrl or Key.RightCtrl or Key.LeftAlt or Key.RightAlt or Key.LWin or Key.RWin) return;
            StringBuilder shortcutText = new();
            if ((Keyboard.Modifiers & ModifierKeys.Control) != 0) shortcutText.Append("Ctrl+");
            if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0) shortcutText.Append("Shift+");
            if ((Keyboard.Modifiers & ModifierKeys.Alt) != 0) shortcutText.Append("Alt+");

            shortcutText.Append(key.ToString());
            shortcutKeyText.Text = shortcutText.ToString();
            SharedInstance.Instance.registerHotKeyAction?.Invoke(Keyboard.Modifiers, key);
            UserConfig.Default.config.modifierKeys = Keyboard.Modifiers;
            UserConfig.Default.config.key = key;
            UserConfig.SaveConfig();
            SharedInstance.Instance.isRecordingShortcutKey = false;
        }
    }

    /// <summary>
    /// 检查更新
    /// Check for updates
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CheckUpdateBtnAction(object sender, MouseButtonEventArgs e)
    {
        (DataContext as SettingViewModel)?.Update();
    }

    /// <summary>
    /// 切换更新渠道
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ChooseUpdatePlaceAction(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount != 1)
            return;
        e.Handled = true;
        updatePlacePopup.IsOpen = true;
    }

    /// <summary>
    /// 切换更新渠道
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void SelectUpdatePlaceAction(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount != 1)
            return;
        e.Handled = true;
        if (sender is not UIButton btn)
            return;
        UserConfig.Default.config.updatePlace = btn.title switch
        {
            "GitHub" => 0,
            "Gitee" => 1,
            _ => UserConfig.Default.config.updatePlace
        };
        updatePlaceText.Text = btn.title;
        UserConfig.SaveConfig();
        updatePlacePopup.IsOpen = false;
        (DataContext as SettingViewModel)?.CheckUpdate();
    }
}