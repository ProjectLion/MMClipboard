/*🏷️----------------------------------------------------------------
 *📄 文件名：ContentInputWindow.xaml.cs
 *🏷️
 *👨🏽‍💻 创建者：Ht
 *⏱️ 创建时间：2023-12-22 10:01:45
 *🏷️----------------------------------------------------------------*/


using System;
using System.Windows;
using System.Windows.Input;


namespace MMClipboard.View;

/// <summary>
/// ContentInputWindow.xaml 的交互逻辑
/// </summary>
public partial class ContentInputWindow
{
    public Action<string> confirmAction;

    private bool isSearch;

    public ContentInputWindow(string content, string confirmTitle)
    {
        InitializeComponent();
        confirmBtn.title = confirmTitle;
        contentTextBox.Text = content;
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        Activate();
        contentTextBox.SelectAll();
        Keyboard.Focus(contentTextBox);
    }

    private void SearchAction(object sender, MouseButtonEventArgs e)
    {
        confirmAction?.Invoke(contentTextBox.Text);
        isSearch = true;
        Close();
    }

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);
        SharedInstance.Instance.contentInputWindow = null;
    }

    protected override void OnDeactivated(EventArgs e)
    {
        base.OnDeactivated(e);
        if (!isSearch) Close();
    }

    private void Window_KeyDown(object sender, KeyEventArgs e)
    {
        // e.Key.Debug();
        if (e.Key != Key.Enter) return;
        confirmAction?.Invoke(contentTextBox.Text);
        isSearch = true;
        Close();
    }
}