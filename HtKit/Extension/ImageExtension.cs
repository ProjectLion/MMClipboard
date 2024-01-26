/*🏷️----------------------------------------------------------------
 *📄 文件名：ImageExtension.cs
 *🏷️
 *👨🏽‍💻 创建者：Ht
 *⏱️ 创建时间：2023-12-22 10:01:45
 *🏷️----------------------------------------------------------------*/


using System;
using System.Windows.Controls;
using System.Windows.Media.Imaging;


public static class ImageExtension
{
    /// <summary>
    /// 加载目标位置图片
    /// </summary>
    /// <param name="target"></param>
    /// <param name="url"></param>
    public static void Ht_Load(this Image target, string url)
    {
        target.Source = BitmapFrame.Create(new Uri(url), BitmapCreateOptions.None, BitmapCacheOption.Default);
    }
}