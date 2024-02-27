/*🏷️----------------------------------------------------------------
 *📄 文件名：SmallWindow.xaml.cs
 *🏷️
 *👨🏽‍💻 创建者：Ht
 *⏱️ 创建时间：2023-12-22 10:01:45
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
using ListBox = System.Windows.Controls.ListBox;


namespace MMClipboard.View;

/// <summary>
/// SmallWindow.xaml 的交互逻辑
/// </summary>
public partial class SmallWindow
{
    private UIButton olbSelectTypeBtn;

    private double fixX = 1;
    private double fixY = 1;

    private bool isCollect;

    private ClipboardHistoryViewModel viewModel => DataContext as ClipboardHistoryViewModel;

    public SmallWindow()
    {
        InitializeComponent();
        DataContext = new ClipboardHistoryViewModel(this);
        SharedInstance.Instance.reloadDataAction = () =>
        {
            if (historyListBox.Items.Count > 0)
            {
                var firstItem = historyListBox.Items[0];
                if (firstItem != null)
                    historyListBox.ScrollIntoView(firstItem);
            }
            viewModel?.RefreshData();
        };
        olbSelectTypeBtn = allBtn;
        var mStartDate = DataBaseController.GetFirstDateWithHistory();
        calendar.DisplayDateStart = mStartDate;
        calendar.DisplayDateEnd = DateTime.Today;
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
            left = mousePosition.X / fixX - 400;

        // 设置窗口的位置
        Left = left;
        Top = top;
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
        Win32Api.SetWindowLong(hWnd, Win32Api.GWL_EXSTYLE, exStyle);
    }

    /// <summary>
    /// 关闭
    /// Close
    /// </summary>
    private void CloseAction(object sender, RoutedEventArgs e)
    {
        SharedInstance.Instance.contentInputWindow?.Close();
        Close();
        e.Handled = true;
    }

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);
        viewModel?.Dispose();
        DataContext = null;
        SharedInstance.Instance.mainWindow = null;
        SharedInstance.Instance.reloadDataAction = null;
        MemoryHelper.FlushMemory();
    }

    /// <summary>
    /// 窗口移动
    /// Window move
    /// </summary>
    private void HeaderMoveAction(object sender, MouseButtonEventArgs e)
    {
        DragMove();
    }

    /// <summary>
    /// 鼠标滚动（竖向）
    /// Mouse wheel (vertical)
    /// </summary>
    private void ListBoxMouseWheel(object sender, MouseWheelEventArgs e)
    {
        var listBox = (ListBox)sender;
        var scroll = UIElementHelper.FindVisualChild<ScrollViewer>(listBox);
        switch (e.Delta)
        {
            case > 0:
                scroll?.LineUp();
                break;
            case < 0:
                scroll?.LineDown();
                break;
        }
        e.Handled = true;
    }

    /// <summary>
    /// Item选中事件
    /// Item selected event
    /// </summary>
    private void HistoryListItemSelected(object sender, MouseButtonEventArgs e)
    {
        e.Handled = true;
        if (e.ClickCount != 1) return;
        var listBoxItem = UIElementHelper.FindAncestor<ListBoxItem>((DependencyObject)e.OriginalSource);
        if (listBoxItem == null)
            return;
        viewModel?.CopyItem(listBoxItem.Content as ClipItemModel);
    }

    /// <summary>
    /// 删除Item
    /// Delete item
    /// </summary>
    private void DeleteItemEvent(object sender, RoutedEventArgs e)
    {
        e.Handled = true;
        if (sender is not UIButton btn) return;
        var a = UIElementHelper.FindAncestor<ListBoxItem>(btn);
        if (a == null)
            return;
        viewModel?.DeleteItem(a.Content as ClipItemModel);
    }

    /// <summary>
    /// 收藏Item
    /// Collect item
    /// </summary>
    private void CollectItemEvent(object sender, RoutedEventArgs e)
    {
        e.Handled = true;
        if (sender is not UIButton btn) return;
        var a = UIElementHelper.FindAncestor<ListBoxItem>(btn);
        if (a == null) return;
        ClipboardHistoryViewModel.CollectItem(a.Content as ClipItemModel);
    }

    /// <summary>
    /// 删除所有
    /// Delete all
    /// </summary>
    private void DeleteAllContent(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 1) viewModel?.DeleteAll(calendar);
    }

    /// <summary>
    /// 筛选按钮点击
    /// Filter button click
    /// </summary>
    private void FilterButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is not UIButton btn) return;
        olbSelectTypeBtn.borderWidth = new Thickness(0);
        btn.borderWidth = new Thickness(1);
        olbSelectTypeBtn = btn;
        switch (btn.Tag)
        {
            case "all":
                viewModel?.FilterDataWithType(ClipType.All);
                break;
            case "string":
                viewModel?.FilterDataWithType(ClipType.Text);
                break;
            case "image":
                viewModel?.FilterDataWithType(ClipType.Image);
                break;
            case "file":
                viewModel?.FilterDataWithType(ClipType.File);
                break;
        }
        ListScrollToFirst();
    }

    /// <summary>
    /// 收藏按钮点击
    /// Collect button click
    /// </summary>
    private void collectBtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        isCollect = !isCollect;
        ((ClipboardHistoryViewModel)DataContext).isCollect = isCollect ? 1 : 0;
        ((ClipboardHistoryViewModel)DataContext).FilterData();
        ListScrollToFirst();
    }

    /// <summary>
    /// 日历组件日期选择事件
    /// Calendar SelectedDatesChanged
    /// </summary>
    private void Calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is not Calendar) return;
        if (e.AddedItems.Count <= 0) return;
        var d = Convert.ToDateTime(e.AddedItems[0]);
        if (d == DateTime.MinValue) return;
        viewModel?.FilterDataWithDate(d);
        calendarParentPopup.IsOpen = false;
        ListScrollToFirst();
    }

    /// <summary>
    /// 打开日历
    /// Open calendar view
    /// </summary>
    private void Calender_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 1) calendarParentPopup.IsOpen = true;
    }

    /// <summary>
    /// 所有时间按钮点击
    /// </summary>
    private void AllTimeBtn_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        e.Handled = true;
        if (e.ClickCount != 1) return;
        viewModel?.FilterDataWithDate(DateTime.MinValue);
        calendarParentPopup.IsOpen = false;
        calendar.SelectedDate = null;
        ListScrollToFirst();
    }

    /// <summary>
    /// 显示搜索框
    /// Show search window
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void SearchGridMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        var positionOnScreen = searchGrid.PointToScreen(new Point(0, 0));
        var left = (positionOnScreen.X - 60) / fixX;
        var top = positionOnScreen.Y / fixY;
        if (SharedInstance.Instance.contentInputWindow != null)
        {
            SharedInstance.Instance.contentInputWindow?.Close();
            return;
        }
        ContentInputWindow contentInputWindow = new(searchTextBox.Text, "搜索")
        {
            Top = top,
            Left = left,
            confirmAction = (content) =>
            {
                searchTextBox.Text = content.Ht_IsEmpty() ? "搜索" : content;
                viewModel?.SearchContent(content);
                ListScrollToFirst();
            },
            ShowActivated = true
        };
        contentInputWindow.Show();
        SharedInstance.Instance.contentInputWindow = contentInputWindow;
    }

    /// <summary>
    /// 打开设置窗口
    /// Open setting window
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void SettingBtnClick(object sender, RoutedEventArgs e)
    {
        SharedInstance.ShowSettingWindow(this);
    }

    /// <summary>
    /// 将列表滚动到第一条数据
    /// </summary>
    private void ListScrollToFirst()
    {
        var scroll = UIElementHelper.FindVisualChild<ScrollViewer>(historyListBox);
        scroll?.ScrollToTop();
    }

    /// <summary>
    /// 菜单点击事件
    /// Context menu click event
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

        if ((popup.PlacementTarget as Border).DataContext is not ClipItemModel model)
            return;

        viewModel?.CreateContextMenu(cm, model);
    }
}