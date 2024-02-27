/*🏷️----------------------------------------------------------------
 *📄 文件名：ClipboardHistoryViewModel.cs
 *🏷️
 *👨🏽‍💻 创建者：Ht
 *⏱️ 创建时间：2023/10/10 15:20:20
 *🏷️----------------------------------------------------------------*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using HtKit;
using MMClipboard.Model;
using MMClipboard.Tool;
using MMClipboard.UserConfigs;


namespace MMClipboard.ViewModel;

public class ClipboardHistoryViewModel : ObservableObject, IDisposable
{
    public List<ClipItemModel> clips
    {
        get => _clips;
        set => SetProperty(ref _clips, value);
    }
    private List<ClipItemModel> _clips;

    // window
    private Window m_window;

    // 保存原始数据
    private List<ClipItemModel> m_dataSource;

    public DateTime selectDate
    {
        get => _selectDate;
        private set
        {
            if (value == DateTime.MinValue)
                selectDateStr = "所有时间";
            else if (value == DateTime.Today)
                selectDateStr = "今天";
            else if (value == DateTime.Today.AddDays(-1))
                selectDateStr = "昨天";
            else if (value == DateTime.Today.AddDays(-2))
                selectDateStr = "前天";
            else if (value == DateTime.Today.AddDays(-3))
                selectDateStr = "大前天";
            else
                selectDateStr = value.ToString("MM月dd日");
            SetProperty(ref _selectDate, value);
        }
    }
    private DateTime _selectDate;

    public string selectDateStr
    {
        get => _selectDateStr;
        private set => SetProperty(ref _selectDateStr, value);
    }
    private string _selectDateStr;

    public WindowBackgroundModel backgroundModel { get; } = new();

    private ClipType m_filter = ClipType.All;

    /// <summary>
    /// 是否筛选收藏的内容 0: false, 1: true
    /// </summary>
    public int isCollect
    {
        get => _isCollect;
        set => SetProperty(ref _isCollect, value);
    }
    private int _isCollect;

    private string m_searchStr = string.Empty;

    public ClipboardHistoryViewModel(Window window)
    {
        m_window = window;
        // 这里设置时间后会触发日历的SelectedChanged事件
        selectDate = DateTime.Today;
    }

    /// <summary>
    /// 刷新数据
    /// </summary>
    public void RefreshData()
    {
        FilterDataWithDate(selectDate);
    }

    /// <summary>
    /// 直接复制内容
    /// </summary>
    /// <param name="item"></param>
    public void CopyItem(ClipItemModel item)
    {
        if (item == null) return;
        switch (item.clipType)
        {
            case ClipType.Text:
                CopyText(item.content);
                break;
            case ClipType.Image:
            case ClipType.File:
                CopyFile(item.content);
                break;
        }
    }

    /// <summary>
    /// 复制文本
    /// </summary>
    private void CopyText(string content)
    {
        CopyAndPasteHelper.CopyAndPasteText(content, () =>
        {
            if (UserConfig.Default.config.isCopiedClose)
                m_window.Close();
        });
    }

    /// <summary>
    /// 复制文件
    /// </summary>
    private void CopyFile(string path)
    {
        CopyAndPasteHelper.CopyAndPasteFile(path, () =>
        {
            if (UserConfig.Default.config.isCopiedClose)
                m_window.Close();
        });
    }

    /// <summary>
    /// 根据类型筛选数据
    /// </summary>
    /// <param name="param"></param>
    public void FilterDataWithType(ClipType param)
    {
        m_filter = param;
        FilterData();
    }

    /// <summary>
    /// 根据时间筛选数据
    /// </summary>
    /// <param name="date"></param>
    public void FilterDataWithDate(DateTime date)
    {
        selectDate = date;
        m_dataSource = date == DateTime.MinValue ? DataBaseController.GetAllHistoryData() : DataBaseController.GetHistoryDataWithDate(date);
        FilterData();
    }

    /// <summary>
    /// 筛选
    /// </summary>
    public async void FilterData()
    {
        await Task.Run(() =>
        {
            if (m_filter == ClipType.All)
                clips = m_dataSource.Where((a) =>
                {
                    var isContains = a.content.Contains(m_searchStr, StringComparison.OrdinalIgnoreCase);
                    if (isCollect == 1) return a.collect == 1 && isContains;
                    return isContains;
                }).ToList();
            else
                clips = m_dataSource.Where((a) =>
                {
                    var isContains = a.content.Contains(m_searchStr, StringComparison.OrdinalIgnoreCase);
                    if (isCollect == 1) return a.collect == 1 && a.clipType == m_filter && isContains;
                    return a.clipType == m_filter && isContains;
                }).ToList();
        });
    }

    /// <summary>
    /// 删除数据项
    /// </summary>
    /// <param name="item"></param>
    public void DeleteItem(ClipItemModel item)
    {
        if (item == null) return;
        if (!DataBaseController.DeleteHistoryData(item)) return;
        m_dataSource.Remove(item);
        FilterData();
        if (item.clipType == ClipType.Image)
            CacheHelper.DeleteCacheImage(item.content);
    }

    /// <summary>
    /// 收藏
    /// </summary>
    /// <param name="item"></param>
    public static void CollectItem(ClipItemModel item)
    {
        if (item == null) return;
        if (DataBaseController.UpdateHistoryDataCollectState(item.id, item.collect == 0 ? 1 : 0)) item.collect = item.collect == 0 ? 1 : 0;
    }

    /// <summary>
    /// 内容搜索
    /// </summary>
    /// <param name="content"></param>
    public void SearchContent(string content)
    {
        m_searchStr = content;
        FilterData();
    }

    /// <summary>
    /// 删除当前选中的日期+类型的所有记录，也就是列表上显示出来的所有数据
    /// </summary>
    public void DeleteAll(Calendar calendar)
    {
        // 如果选中日期是所有时间、所有类型并且没有选中收藏，则清空数据，就不需要走foreach了
        if (selectDate == DateTime.MinValue && m_filter == ClipType.All && isCollect == 0)
        {
            DataBaseController.DeleteAllHistoryData();
            CacheHelper.ClearCache(selectDate);
            // 清空所有数据后设置一下日历的起始时间和当前选中时间
            calendar.DisplayDateStart = DateTime.Today;
            calendar.DisplayDateEnd = DateTime.Today;
            selectDate = DateTime.Today;
        }
        else
        {
            // 其他的所有情况都可以直接忽略，直接遍历clips数组进行删除
            // 删除数据的同时如果是图片，并且缓存到本地的就一起删除
            if (m_filter == ClipType.All)
                DataBaseController.DeleteAllHistoryWithDate(selectDate);
            else
                DataBaseController.DeleteHistoryImageWithDate(selectDate, m_filter);
            var temp = _clips;
            foreach (var item in temp.Where(item => item.clipType == ClipType.Image))
                Task.Run(() =>
                {
                    CacheHelper.DeleteCacheImage(item.content);
                });
        }
        RefreshData();
    }

    #region 其他工具方法

    public void CreateContextMenu(ContextMenu cm, ClipItemModel model)
    {
        switch (model.clipType)
        {
            case ClipType.Text:
                CreateMenuItem(cm, "复制文本", () =>
                {
                    CopyText(model.content);
                });
                var phraseMenu = CreateMenuItem(cm, "添加到快捷短语", null);
                var tags = DataBaseController.GetAllPhraseTags();
                if (tags == null || tags.Count == 0)
                    tags = ["默认分组"];
                foreach (var tag in tags)
                    CreateMenuItem(phraseMenu, $"{tag}", () =>
                    {
                        DataBaseController.AddPhrase(ShortcutPhraseModel.Create(model.content, tag));
                        SharedInstance.Instance.addPhraseAction?.Invoke();
                    });
                if (model.content.Ht_IsWebsite())
                    CreateMenuItem(cm, "打开网页", () =>
                    {
                        OpenExternalWindowHelper.OpenWebsite(model.content);
                    });
                else if (model.content.Ht_IsFile())
                    CreateMenuItem(cm, "打开文件所在目录", () =>
                    {
                        OpenExternalWindowHelper.SelectFileInFolder(model.content);
                    });
                else if (model.content.Ht_IsDirectory())
                    CreateMenuItem(cm, "打开文件夹", () =>
                    {
                        OpenExternalWindowHelper.OpenFolder(model.content);
                    });
                break;
            case ClipType.Image:
            case ClipType.File:
                CreateMenuItem(cm, "复制文件(夹)", () =>
                {
                    CopyFile(model.content);
                });
                CreateMenuItem(cm, "复制文件地址", () =>
                {
                    CopyText(model.content);
                });
                if (model.content.Ht_IsFile())
                    CreateMenuItem(cm, "打开文件所在目录", () =>
                    {
                        OpenExternalWindowHelper.SelectFileInFolder(model.content);
                    });
                else if (model.content.Ht_IsDirectory())
                    CreateMenuItem(cm, "打开文件夹", () =>
                    {
                        OpenExternalWindowHelper.OpenFolder(model.content);
                    });
                break;
        }
        // 不做任何操作菜单
        var nothingItem = new MenuItem()
        {
            Header = "不做任何操作",
            Style = (Style)Application.Current.Resources["CustomMenuItem"],
            Foreground = HtColor.GetBrushWithString("#ffb74d")
        };
        nothingItem.Click += CloseMenuItemClick;

        // 删除菜单
        var deleteItem = new MenuItem()
        {
            Header = "删除",
            Style = (Style)Application.Current.Resources["CustomMenuItem"],
            Foreground = HtColor.GetBrushWithString("#E53935")
        };
        deleteItem.Click += (_, arg) =>
        {
            DeleteItem(model);
            arg.Handled = true;
        };

        cm.Items.Add(deleteItem);
        cm.Items.Add(nothingItem);
    }

    /// <summary>
    /// 创建并添加一个MenuItem
    /// Create and add a MenuItem
    /// </summary>
    /// <param name="cm"></param>
    /// <param name="header"></param>
    /// <param name="ac"></param>
    public static MenuItem CreateMenuItem(ItemsControl cm, string header, Action ac)
    {
        var item = new MenuItem()
        {
            Header = header,
            Foreground = Brushes.White,
            Style = (Style)Application.Current.Resources["CustomMenuItem"]
        };
        item.Click += (_, arg) =>
        {
            ac?.Invoke();
            arg.Handled = true;
        };
        cm.Items.Add(item);
        return item;
    }

    /// <summary>
    /// 不做任何操作的MenuItem
    /// Do nothing
    /// </summary>
    private static void CloseMenuItemClick(object sender, RoutedEventArgs e)
    {
        e.Handled = true;
    }

    #endregion

    public void Dispose()
    {
        backgroundModel.Dispose();
    }
}