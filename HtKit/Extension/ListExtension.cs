/*🏷️----------------------------------------------------------------
 *📄 文件名：ListExtension.cs
 *🏷️
 *👨🏽‍💻 创建者：Ht
 *⏱️ 创建时间：2023-12-22 10:01:45
 *🏷️----------------------------------------------------------------*/


using System;
using System.Collections.Generic;


public static class ListExtension
{
    /// <summary>
    /// 遍历
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="target"></param>
    /// <param name="ac"></param>
    public static void Ht_For<T>(this List<T> target, Action<int, T> ac)
    {
        if (target.Count <= 0 || ac == null) return;
        for (var i = 0; i < target.Count; i++) ac(i, target[i]);
    }

    /// <summary>
    /// 反向遍历
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="target"></param>
    /// <param name="ac"></param>
    public static void Ht_InversionFor<T>(this List<T> target, Action<int, T> ac)
    {
        if (target.Count <= 0 || ac == null) return;
        for (var i = target.Count - 1; i >= 0; i--) ac(i, target[i]);
    }
}