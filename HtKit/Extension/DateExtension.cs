/*🏷️----------------------------------------------------------------
 *📄 文件名：DateExtension.cs
 *🏷️
 *👨🏽‍💻 创建者：Ht
 *⏱️ 创建时间：2023-12-22 10:01:45
 *🏷️----------------------------------------------------------------*/


using System;


public static class DateExtension
{
    /// <summary>
    /// 获取周信息
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public static string Ht_GetWeek(this DateTime target)
    {
        return target.DayOfWeek switch
        {
            DayOfWeek.Monday => "星期一",
            DayOfWeek.Tuesday => "星期二",
            DayOfWeek.Wednesday => "星期三",
            DayOfWeek.Thursday => "星期四",
            DayOfWeek.Friday => "星期五",
            DayOfWeek.Saturday => "星期六",
            DayOfWeek.Sunday => "星期日",
            _ => ""
        };
    }
}