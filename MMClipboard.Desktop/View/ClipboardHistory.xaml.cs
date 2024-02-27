/*🏷️----------------------------------------------------------------
 *📄 文件名：ClipboardHistory.xaml.cs
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
using System.Windows.Media;
using HtUIKit;
using MMClipboard.Model;
using MMClipboard.Tool;
using MMClipboard.ViewModel;
using ListBox = System.Windows.Controls.ListBox;


namespace MMClipboard.View;

/// <summary>
/// ClipboardHistory.xaml 的交互逻辑
/// </summary>
public partial class ClipboardHistory
{
    private UIButton olbSelectTypeBtn;

    private DateTime m_startDate;

    private bool isCollect;

    private ClipboardHistoryViewModel viewModel => DataContext as ClipboardHistoryViewModel;

    private Window bindWindow;

    public ClipboardHistory(Window wd)
    {
        InitializeComponent();
        bindWindow = wd;
        DataContext = new ClipboardHistoryViewModel(wd);
        SharedInstance.Instance.reloadDataAction = () =>
        {
            ListScrollToFirst();
            (DataContext as ClipboardHistoryViewModel)?.RefreshData();
        };
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        olbSelectTypeBtn = allBtn;
        m_startDate = DataBaseController.GetFirstDateWithHistory();
        calendar.DisplayDateStart = m_startDate;
        calendar.DisplayDateEnd = DateTime.Today;
    }

    private void UserControl_Unloaded(object sender, RoutedEventArgs e)
    {
        viewModel?.Dispose();
        DataContext = null;
        SharedInstance.Instance.reloadDataAction = null;
    }

    private static void ScrollToLeft(ScrollViewer scroll)
    {
        if (scroll == null) return;
        scroll.LineLeft();
        scroll.ScrollToTop();
    }

    private static void ScrollToRight(ScrollViewer scroll)
    {
        if (scroll == null) return;
        scroll.LineRight();
        scroll.ScrollToTop();
    }

    // 鼠标滚动（横向）
    private void ListBoxMouseWheel(object sender, MouseWheelEventArgs e)
    {
        // if (!canScroll) return;
        var listBox = (ListBox)sender;
        var scroll = FindVisualChild<ScrollViewer>(listBox);
        switch (e.Delta)
        {
            case > 0:
                ScrollToLeft(scroll);
                break;
            case < 0:
                ScrollToRight(scroll);
                break;
        }
        e.Handled = true;
    }

    private static T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
    {
        if (obj == null) return null;
        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
        {
            var child = VisualTreeHelper.GetChild(obj, i);
            if (child is T dependencyObject) return dependencyObject;
            var childItem = FindVisualChild<T>(child);
            if (childItem != null) return childItem;
        }
        return null;
    }

    /// <summary>
    /// Item选中事件
    /// Item select event
    /// </summary>
    private void HistoryListItemSelected(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount != 1) return;
        var listBoxItem = FindAncestor<ListBoxItem>((DependencyObject)e.OriginalSource);
        if (listBoxItem == null) return;
        (DataContext as ClipboardHistoryViewModel)?.CopyItem(listBoxItem.Content as ClipItemModel);
    }

    /// <summary>
    /// 删除
    /// </summary>
    private void DeleteItemEvent(object sender, RoutedEventArgs e)
    {
        e.Handled = true;
        if (sender is not UIButton btn) return;
        var a = FindAncestor<ListBoxItem>(btn);
        if (a == null) return;
        (DataContext as ClipboardHistoryViewModel)?.DeleteItem(a.Content as ClipItemModel);
    }

    private static T FindAncestor<T>(DependencyObject current)
        where T : DependencyObject
    {
        do
        {
            if (current is T ancestor) return ancestor;
            current = VisualTreeHelper.GetParent(current);
        }
        while (current != null);

        return null;
    }

    /// <summary>
    /// 收藏
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CollectItemEvent(object sender, RoutedEventArgs e)
    {
        e.Handled = true;
        if (sender is not UIButton btn) return;
        var a = FindAncestor<ListBoxItem>(btn);
        if (a == null) return;
        ClipboardHistoryViewModel.CollectItem(a.Content as ClipItemModel);
    }

    /// <summary>
    /// 删除所有
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void DeleteAllContent(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 1) (DataContext as ClipboardHistoryViewModel)?.DeleteAll(calendar);
    }

    /// <summary>
    /// 关闭
    /// </summary>
    private void CloseEvent(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount != 1) return;
        var parentWindow = Window.GetWindow(this);
        parentWindow?.Close();
    }

    /// <summary>
    /// 筛选按钮点击
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
                (DataContext as ClipboardHistoryViewModel)?.FilterDataWithType(ClipType.All);
                break;
            case "string":
                (DataContext as ClipboardHistoryViewModel)?.FilterDataWithType(ClipType.Text);
                break;
            case "image":
                (DataContext as ClipboardHistoryViewModel)?.FilterDataWithType(ClipType.Image);
                break;
            case "file":
                (DataContext as ClipboardHistoryViewModel)?.FilterDataWithType(ClipType.File);
                break;
        }
        ListScrollToFirst();
    }

    /// <summary>
    /// 收藏按钮点击
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void collectBtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        isCollect = !isCollect;
        ((ClipboardHistoryViewModel)DataContext).isCollect = isCollect ? 1 : 0;
        ((ClipboardHistoryViewModel)DataContext).FilterData();
        ListScrollToFirst();
    }

    /// <summary>
    /// 日历时间选择
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is not Calendar) return;
        if (e.AddedItems.Count <= 0) return;
        var d = Convert.ToDateTime(e.AddedItems[0]);
        if (d == DateTime.MinValue) return;
        (DataContext as ClipboardHistoryViewModel)?.FilterDataWithDate(d);
        calendarParentPopup.IsOpen = false;
        ListScrollToFirst();
    }

    /// <summary>
    /// 选择日期按钮点击
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Calender_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 1) calendarParentPopup.IsOpen = true;
    }

    /// <summary>
    /// "所有时间"按钮点击
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void AllTimeBtn_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        e.Handled = true;
        if (e.ClickCount != 1) return;
        (DataContext as ClipboardHistoryViewModel)?.FilterDataWithDate(DateTime.MinValue);
        calendarParentPopup.IsOpen = false;
        calendar.SelectedDate = null;
        ListScrollToFirst();
    }

    /// <summary>
    /// 搜索内容Border点击
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void SearchBorderClick(object sender, MouseButtonEventArgs e)
    {
        var positionOnScreen = searchBorder.PointToScreen(new Point(0, 0));
        var mousePosition = System.Windows.Forms.Cursor.Position;
        var screen = Screen.FromPoint(mousePosition);
        var sw = screen.Bounds.Width;
        var sh = screen.Bounds.Height;
        var fixX = sw / SystemParameters.PrimaryScreenWidth;
        var fixY = sh / SystemParameters.PrimaryScreenHeight;
        var left = (positionOnScreen.X - 60) / fixX;
        var top = positionOnScreen.Y / fixY + 10;
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
                searchTextBox.Text = content.Ht_IsEmpty() ? "搜索(文字or文件名)" : content;
                (DataContext as ClipboardHistoryViewModel)?.SearchContent(content);
                ListScrollToFirst();
            },
            ShowActivated = true
        };
        contentInputWindow.Show();
        SharedInstance.Instance.contentInputWindow = contentInputWindow;
    }

    /// <summary>
    /// 设置按钮点击
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void SettingBtnClick(object sender, MouseButtonEventArgs e)
    {
        SharedInstance.ShowSettingWindow(bindWindow);
    }

    /// <summary>
    /// 将列表滚动到第一条数据
    /// </summary>
    private void ListScrollToFirst()
    {
        var scroll = FindVisualChild<ScrollViewer>(historyListBox);
        scroll?.ScrollToHorizontalOffset(0);
    }

    /// <summary>
    /// 菜单点击事件
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