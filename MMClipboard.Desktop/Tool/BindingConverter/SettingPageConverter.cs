/*🏷️----------------------------------------------------------------
 *📄 文件名：SettingPageConverter.cs
 *🏷️
 *👨🏽‍💻 创建者：Ht
 *⏱️ 创建时间：2023/12/11 17:19:11
 *🏷️----------------------------------------------------------------*/

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MMClipboard.Tool.BindingConverter
{
    public class ChooseBGImgVisibilityConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Visibility.Collapsed;
            if ((bool)value)
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ChooseBGColorVisibilityConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Visibility.Collapsed;
            if ((bool)value)
                return Visibility.Collapsed;
            else
                return Visibility.Visible;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}