/*🏷️----------------------------------------------------------------
 *📄 文件名：UIElementHelper.cs
 *🏷️
 *👨🏽‍💻 创建者：Ht
 *⏱️ 创建时间：2024-02-03 15:45:43
 *🏷️----------------------------------------------------------------*/


using System.Windows;
using System.Windows.Media;


namespace MMClipboard.Tool;

public static class UIElementHelper
{
    /// <summary>
    /// 查找可视化树中指定类型的子元素
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
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
    /// 查找可视化树中指定类型的祖先元素
    /// </summary>
    /// <param name="current"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T FindAncestor<T>(DependencyObject current)
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
}