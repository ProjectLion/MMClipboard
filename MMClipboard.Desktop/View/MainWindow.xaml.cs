/*🏷️----------------------------------------------------------------
 *📄 文件名：MainWindow.xaml.cs
 *🏷️
 *👨🏽‍💻 创建者：Ht
 *⏱️ 创建时间：2023-12-22 10:01:45
 *🏷️----------------------------------------------------------------*/


using System;
using System.Windows.Interop;
using HtKit;
using MMClipboard.ViewModel;


namespace MMClipboard.View;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel(this);
    }

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);
        const int WS_EX_NOACTIVATE = 0x08000000;
        const int WS_CHILD = 0x40000000;
        // Get this window's handle
        var hWnd = new WindowInteropHelper(this).Handle;
        var exStyle = Win32Api.GetWindowLong(hWnd, Win32Api.GWL_EXSTYLE);
        exStyle |= 128;
        exStyle |= WS_CHILD;
        exStyle |= WS_EX_NOACTIVATE;
        Win32Api.SetWindowLong(hWnd, Win32Api.GWL_EXSTYLE, exStyle);
    }

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);
        DataContext = null;
        SharedInstance.Instance.mainWindow = null;
        SharedInstance.Instance.reloadDataAction = null;
        MemoryHelper.FlushMemory();
    }
}