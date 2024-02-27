/*🏷️----------------------------------------------------------------
 *📄 文件名：PhraseItemDataConverters.cs
 *🏷️
 *👨🏽‍💻 创建者：Ht
 *⏱️ 创建时间：2024-02-04 16:52:49
 *🏷️----------------------------------------------------------------*/


using System;
using System.Globalization;
using System.Windows.Data;
using HtKit;


namespace MMClipboard.Tool.BindingConverter;

public class PhraseGroupColorConverter : IValueConverter
{
    object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null || ((string)value).Ht_IsEmpty())
            return HtColor.GetBrushWithString("#FFFFFF");
        return HtColor.GetBrushWithString($"#{value}");
    }

    object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}