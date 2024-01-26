/*🏷️----------------------------------------------------------------
 *📄 文件名：NodataTipHideConverter.cs
 *🏷️
 *👨🏽‍💻 创建者：Ht
 *⏱️ 创建时间：2023/12/18 12:17:56
 *🏷️----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using MMClipboard.Model;

namespace MMClipboard.Tool.BindingConverter
{
    public class NodataTipHideConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || ((List<ClipItemModel>)value).Count <= 0)
                return Visibility.Visible;
            return Visibility.Hidden;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}