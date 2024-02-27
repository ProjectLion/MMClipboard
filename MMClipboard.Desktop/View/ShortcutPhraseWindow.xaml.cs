/*🏷️----------------------------------------------------------------
 *📄 文件名：ShortcutPhraseWindow.xaml.cs
 *🏷️
 *👨🏽‍💻 创建者：Ht
 *⏱️ 创建时间：2024-01-31 10:06:12
 *🏷️----------------------------------------------------------------*/


using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using HtKit;
using HtUIKit;
using MMClipboard.Model;
using MMClipboard.Tool;
using MMClipboard.ViewModel;
using Application = System.Windows.Application;


namespace MMClipboard.View;

public partial class ShortcutPhraseWindow
{
    private ShortcutPhraseViewModel viewModel => DataContext as ShortcutPhraseViewModel;

    private double fixX = 1;
    private double fixY = 1;

    public ShortcutPhraseWindow()
    {
        InitializeComponent();
        SharedInstance.Instance.phraseWindow = this;
        DataContext = new ShortcutPhraseViewModel(this);
        SharedInstance.Instance.addPhraseAction = () =>
        {
            viewModel?.RefreshData();
        };
    }

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);
        const int WS_EX_NOACTIVE = 0x08000000;
        const int WS_CHILD = 0x40000000;

        var hWnd = new WindowInteropHelper(this).Handle;
        var exStyle = Win32Api.GetWindowLong(hWnd, Win32Api.GWL_EXSTYLE);
        exStyle |= 128;
        exStyle |= WS_CHILD;
        exStyle |= WS_EX_NOACTIVE;
        _ = Win32Api.SetWindowLong(hWnd, Win32Api.GWL_EXSTYLE, exStyle);
    }

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);
        viewModel?.Dispose();
        SharedInstance.Instance.phraseWindow = null;
        SharedInstance.Instance.addPhraseAction = null;
        DataContext = null;
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        SetPosition();
    }

    private void SetPosition()
    {
        var mousePosition = System.Windows.Forms.Cursor.Position;

        var screen = Screen.FromPoint(mousePosition);

        var sw = screen.Bounds.Width;
        var sh = screen.Bounds.Height;

        fixX = sw / SystemParameters.PrimaryScreenWidth;
        fixY = sh / SystemParameters.PrimaryScreenHeight;

        // 计算窗口左上角的位置，以确保窗口显示在鼠标位置
        var left = mousePosition.X / fixX - 200;
        var top = mousePosition.Y / fixY;

        if (mousePosition.Y + 780 > sh)
        {
            top = mousePosition.Y / fixY - 300;
            left = mousePosition.X / fixX;
        }

        if (mousePosition.X - 200 < 0)
            left = mousePosition.X / fixX;
        else if (mousePosition.X + 200 > sw)
            left = mousePosition.X / fixX - 480;

        // 设置窗口的位置
        Left = left;
        Top = top;
    }

    /// <summary>
    /// 窗口移动
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void HeaderMoveAction(object sender, MouseButtonEventArgs e)
    {
        DragMove();
    }

    /// <summary>
    /// 关闭窗口
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CloseAction(object sender, RoutedEventArgs e)
    {
        Close();
    }

    /// <summary>
    /// Item选中事件
    /// item selected event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ListItemSelected(object sender, MouseButtonEventArgs e)
    {
        e.Handled = true;
        if (e.ClickCount != 1) return;
        var listBoxItem = UIElementHelper.FindAncestor<ListBoxItem>((DependencyObject)e.OriginalSource);
        if (listBoxItem == null)
            return;
        viewModel?.CopyText((listBoxItem.Content as ShortcutPhraseModel)?.phrase);
    }

    /// <summary>
    /// 打开设置页面
    /// open setting window
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void SettingBtnClick(object sender, RoutedEventArgs e)
    {
        SharedInstance.ShowSettingWindow(this);
    }

    /// <summary>
    /// 右键菜单
    /// context menu
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CellContextMenuChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is not ContextMenu cm)
            return;
        // 隐藏的时候将item清空
        if (!(bool)e.NewValue)
        {
            // 移除添加过的item
            while (cm.Items.Count > 0)
                cm.Items.RemoveAt(cm.Items.Count - 1);
            return;
        }
        if (cm.Parent is not Popup popup)
            return;
        if (UIElementHelper.FindAncestor<ListBoxItem>(popup.PlacementTarget).DataContext is not ShortcutPhraseModel model)
            return;
        ClipboardHistoryViewModel.CreateMenuItem(cm, "复制并粘贴", () =>
        {
            viewModel?.CopyText(model.phrase);
        });
        if (model.phrase.Ht_IsWebsite())
            ClipboardHistoryViewModel.CreateMenuItem(cm, "打开网页", () =>
            {
                OpenExternalWindowHelper.OpenWebsite(model.phrase);
            });
        else if (model.phrase.Ht_IsFile())
            ClipboardHistoryViewModel.CreateMenuItem(cm, "打开文件所在目录", () =>
            {
                OpenExternalWindowHelper.SelectFileInFolder(model.phrase);
            });
        else if (model.phrase.Ht_IsDirectory())
            ClipboardHistoryViewModel.CreateMenuItem(cm, "打开文件夹", () =>
            {
                OpenExternalWindowHelper.OpenFolder(model.phrase);
            });
        // 删除菜单
        var deleteItem = new MenuItem()
        {
            Header = "删除短语",
            Style = (Style)Application.Current.Resources["CustomMenuItem"],
            Foreground = HtColor.GetBrushWithString("#E53935")
        };
        deleteItem.Click += (_, arg) =>
        {
            viewModel?.DeletePhrase(model);
            arg.Handled = true;
        };
        cm.Items.Add(deleteItem);
    }

    /// <summary>
    /// 添加短语
    /// add phrase
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void AddPhraseBtnClick(object sender, RoutedEventArgs e)
    {
        var positionOnScreen = PointToScreen(new Point(0, 0));
        var left = positionOnScreen.X / fixX;
        var top = (positionOnScreen.Y + 45) / fixY;
        if (SharedInstance.Instance.contentInputWindow != null)
        {
            SharedInstance.Instance.contentInputWindow?.Close();
            return;
        }
        ContentInputWindow contentInputWindow = new("", "添加")
        {
            Top = top,
            Left = left,
            Width = Width,
            Height = 250,
            confirmAction = (content) =>
            {
                viewModel?.AddPhrase(ShortcutPhraseModel.Create(content));
            },
            ShowActivated = true
        };
        contentInputWindow.Show();
        SharedInstance.Instance.contentInputWindow = contentInputWindow;
    }

    /// <summary>
    /// 编辑
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void EditPhraseBtnClick(object sender, RoutedEventArgs e)
    {
        if (sender is not UIButton btn)
            return;
        if (UIElementHelper.FindAncestor<ListBoxItem>(btn).DataContext is not ShortcutPhraseModel model)
            return;
        model.phrase.Debug();
    }
}