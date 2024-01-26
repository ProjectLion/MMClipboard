/*🏷️----------------------------------------------------------------
 *📄 文件名：Dispatch.cs
 *🏷️
 *👨🏽‍💻 创建者：Ht
 *⏱️ 创建时间：2023-12-22 10:01:45
 *🏷️----------------------------------------------------------------*/


using System;
using System.Windows;


namespace HtKit;

public static class Dispatch
{
    /// <summary>
    /// 回到主线程(同步)
    /// </summary>
    /// <param name="ac"></param>
    public static void BackToMainThreadSync(Action ac)
    {
        Application.Current.Dispatcher.Invoke(ac);
    }

    /// <summary>
    /// 回到主线程(异步)
    /// </summary>
    /// <param name="ac"></param>
    public static async void BackToMainThreadAsync(Action ac)
    {
        await Application.Current.Dispatcher.InvokeAsync(ac);
    }
}