/*🏷️----------------------------------------------------------------
 *📄 文件名：MainWindow.xaml.cs
 *🏷️
 *👨🏽‍💻 创建者：Ht
 *⏱️ 创建时间：2023-12-22 10:01:45
 *🏷️----------------------------------------------------------------*/


using System;
using System.Runtime.InteropServices;
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

    // Win32 API declarations
    private const int GWL_EXSTYLE = -20;
    private const int WS_EX_TOOLWINDOW = 0x80;

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);
        const int WS_EX_NOACTIVATE = 0x08000000;
        const int WS_CHILD = 0x40000000;
        // Get this window's handle
        var hWnd = new WindowInteropHelper(this).Handle;

        // Get the extended window style
        var exStyle = (int)GetWindowLong(hWnd, GWL_EXSTYLE);

        // Set the WS_EX_TOOLWINDOW style
        exStyle |= WS_EX_TOOLWINDOW;
        exStyle |= WS_CHILD;
        exStyle |= WS_EX_NOACTIVATE;
        SetWindowLong(hWnd, GWL_EXSTYLE, exStyle);
    }

    [DllImport("user32.dll")]
    private static extern IntPtr GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll")]
    private static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);
        DataContext = null;
        SharedInstance.Instance.mainWindow = null;
        SharedInstance.Instance.reloadDataAction = null;
        MemoryHelper.FlushMemory();
    }
}