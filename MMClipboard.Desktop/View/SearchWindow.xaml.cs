/*🏷️----------------------------------------------------------------
 *📄 文件名：SearchWindow.xaml.cs
 *🏷️
 *👨🏽‍💻 创建者：Ht
 *⏱️ 创建时间：2023-12-22 10:01:45
 *🏷️----------------------------------------------------------------*/


using System;
using System.Windows;
using System.Windows.Input;


namespace MMClipboard.View;

/// <summary>
/// SearchWindow.xaml 的交互逻辑
/// </summary>
public partial class SearchWindow : Window
{
    public Action<string> searchAction;

    private bool isSearch = false;

    public SearchWindow()
    {
        InitializeComponent();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        Activate();
        searchTextBox.SelectAll();
        Keyboard.Focus(searchTextBox);
    }

    private void SearchAction(object sender, MouseButtonEventArgs e)
    {
        searchAction?.Invoke(searchTextBox.Text);
        isSearch = true;
        Close();
    }

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);
        SharedInstance.Instance.searchWindow = null;
    }

    protected override void OnDeactivated(EventArgs e)
    {
        base.OnDeactivated(e);
        if (!isSearch) Close();
    }

    private void Window_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter) return;
        searchAction?.Invoke(searchTextBox.Text);
        isSearch = true;
        Close();
    }
}