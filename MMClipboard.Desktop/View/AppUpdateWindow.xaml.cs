/*🏷️----------------------------------------------------------------
 *📄 文件名：AppUpdateWindow.xaml.cs
 *🏷️
 *👨🏽‍💻 创建者：Ht
 *⏱️ 创建时间：2023-12-22 10:01:45
 *🏷️----------------------------------------------------------------*/


using System;
using System.Windows;
using System.Windows.Input;
using MMClipboard.Tool.AppUpdate;
using MMClipboard.ViewModel;


namespace MMClipboard.View;

/// <summary>
/// AppUpdateWindow.xaml 的交互逻辑
/// </summary>
public partial class AppUpdateWindow : Window
{
    public AppUpdateWindow(VersionModel mod)
    {
        InitializeComponent();
        DataContext = new AppUpdateViewModel(this, mod);
    }

    private void CloseBtnClick(object sender, RoutedEventArgs e)
    {
        (DataContext as AppUpdateViewModel)?.Cancel();
        Close();
    }

    private void HeaderMoveAction(object sender, MouseButtonEventArgs e)
    {
        DragMove();
    }

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);
        DataContext = null;
    }

    private void UpdateBtnClick(object sender, RoutedEventArgs e)
    {
        (DataContext as AppUpdateViewModel)?.Update();
    }
}