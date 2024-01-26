/*🏷️----------------------------------------------------------------
 *📄 文件名：HtColor.cs
 *🏷️
 *👨🏽‍💻 创建者：Ht
 *⏱️ 创建时间：2023-12-22 10:01:45
 *🏷️----------------------------------------------------------------*/


using System;
using System.Windows.Media;


namespace HtKit;

public static class HtColor
{
    /// <summary>
    /// 通过Hex字符串获取颜色
    /// </summary>
    /// <param name="hex"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static Color ColorWithHex(string hex)
    {
        return (Color)ColorConverter.ConvertFromString(hex)!;
    }

    /// <summary>
    /// 使用Color初始化一个Brush
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    public static SolidColorBrush GetBrushWithColor(Color color)
    {
        return new SolidColorBrush(color);
    }

    /// <summary>
    /// 使用颜色字符串初始化一个Brush
    /// </summary>
    /// <param name="colorString"></param>
    /// <returns></returns>
    public static SolidColorBrush GetBrushWithString(string colorString)
    {
        return new SolidColorBrush(ColorWithHex(colorString));
    }
}