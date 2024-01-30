/*🏷️----------------------------------------------------------------
 *📄 文件名：HistoryItemDataConverters.cs
 *🏷️
 *👨🏽‍💻 创建者：Ht
 *⏱️ 创建时间：2023/9/18 09:07:56
 *🏷️----------------------------------------------------------------*/


using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using HtKit;


namespace MMClipboard.Tool.BindingConverter;

#region 文本类型的文字裁剪，优化TextBlock的显示性能，多余的字没必要显示

public class HistoryItemTextLengthConverter : IValueConverter
{
    object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var tep = value?.ToString();
        if (tep is null)
            return "";
        return tep.Length > 1000 ? tep[..999] : tep;
    }

    object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

#endregion 根据类型的不同切换背景色

#region 根据类型的不同切换背景色

public class HistoryItemBackgroundColorConverter : IValueConverter
{
    object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null)
            return (SolidColorBrush)Application.Current.FindResource("筛选按钮.Text");
        return (ClipType)value switch
        {
            ClipType.Text => (SolidColorBrush)Application.Current.FindResource("筛选按钮.Text"),
            ClipType.Image => (SolidColorBrush)Application.Current.FindResource("筛选按钮.Image"),
            ClipType.File => (SolidColorBrush)Application.Current.FindResource("筛选按钮.File"),
            _ => (SolidColorBrush)Application.Current.FindResource("筛选按钮.Text")
        };
    }

    object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

#endregion 根据类型的不同切换背景色

#region 根据类型显示不同的控件

public class HistoryItemShowTextConverter : IValueConverter
{
    object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null)
            return Visibility.Collapsed;
        return (ClipType)value == ClipType.Text ? Visibility.Visible : Visibility.Collapsed;
    }

    object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class HistoryItemShowImageConverter : IValueConverter
{
    object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null)
            return Visibility.Collapsed;
        return (ClipType)value == ClipType.Image ? Visibility.Visible : Visibility.Collapsed;
    }

    object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class HistoryItemShowFileConverter : IValueConverter
{
    object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null)
            return Visibility.Collapsed;
        return (ClipType)value == ClipType.File ? Visibility.Visible : Visibility.Collapsed;
    }

    object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

#endregion 根据类型显示不同的控件

#region 复制的文件(夹)名

public class HistoryItemFileNameConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null || string.IsNullOrEmpty(value.ToString()))
            return null;
        var path = value.ToString();
        if (File.Exists(path))
            return Path.GetFileName(path);
        return Directory.Exists(path) ? Path.GetFileName(path) : "文件(夹)不存在";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

#endregion 复制的文件Logo

#region 收藏状态

public class HistoryItemCollectStateConverter : IValueConverter
{
    object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var url = "/Images/noCollect.png";
        if (value != null)
            url = value.ToString() == "1" ? "/Images/Collected.png" : "/Images/noCollect.png";
        return new BitmapImage(new Uri(url, UriKind.Relative));
    }

    object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class HistoryItemCollectStateBgColorConverter : IValueConverter
{
    object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var colorStr = "#00673AB7";
        if (value != null && value.ToString() == "1")
            colorStr = "#FF673AB7";
        return HtColor.GetBrushWithString(colorStr);
    }

    object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

#endregion