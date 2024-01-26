/*🏷️----------------------------------------------------------------
 *📄 文件名：IntExtension.cs
 *🏷️
 *👨🏽‍💻 创建者：Ht
 *⏱️ 创建时间：2023-12-22 10:01:45
 *🏷️----------------------------------------------------------------*/


using System;


public static class IntExtension
{
    /// <summary>
    /// 遍历n次
    /// </summary>
    /// <param name="target"></param>
    /// <param name="ac"></param>
    public static void Ht_For(this int target, Action ac)
    {
        if (target <= 0 || ac == null) return;
        for (var i = 0; i < target; i++) ac();
    }

    /// <summary>
    /// 遍历n次 带参数
    /// </summary>
    /// <param name="target"></param>
    /// <param name="ac"></param>
    public static void Ht_For(this int target, Action<int> ac)
    {
        if (target <= 0 || ac == null) return;
        for (var i = 0; i < target; i++) ac(i);
    }

    /// <summary>
    /// 倒循环 带参数
    /// </summary>
    /// <param name="target"></param>
    /// <param name="ac"></param>
    public static void Ht_InversionFor(this int target, Action<int> ac)
    {
        if (target <= 0 || ac == null) return;
        for (var i = target; i > 0; i--) ac(i);
    }

    /// <summary>
    /// 阿拉伯数字转中文数字
    /// </summary>
    /// <param name="target"></param>
    /// <param name="isUpper"> 是否大写(大写：壹佰贰拾叁，小写：一百二十三) </param>
    /// <returns></returns>
    public static string Ht_ToChineseNumber(this int target, bool isUpper = true)
    {
        if (target == 0)
            return "零";

        var x = target.ToString();
        var result = "";
        string[] pArrayNum = { "零", "壹", "贰", "叁", "肆", "伍", "陆", "柒", "捌", "玖" };
        string[] pArrayDigit = { "", "拾", "佰", "仟" };
        string[] pArrayUnits = { "", "万", "亿", "万亿" };

        if (!isUpper)
        {
            pArrayNum = new[] { "零", "一", "二", "三", "四", "五", "六", "七", "八", "九" };
            pArrayDigit = new[] { "", "十", "百", "千" };
        }

        var finger = 0;
        var pIntM = x.Length % 4;
        int pIntK;
        if (pIntM > 0)
            pIntK = x.Length / 4 + 1;
        else
            pIntK = x.Length / 4;

        //外层循环,四位一组,每组最后加上单位: ",万亿,",",亿,",",万,"
        for (var i = pIntK; i > 0; i--)
        {
            var pIntL = 4;
            if (i == pIntK && pIntM != 0)
                pIntL = pIntM;

            //得到一组四位数
            var four = x.Substring(finger, pIntL);
            var P_int_l = four.Length;
            //内层循环在该组中的每一位数上循环
            for (var j = 0; j < P_int_l; j++)
            {
                //处理组中的每一位数加上所在的位
                var n = Convert.ToInt32(four.Substring(j, 1));
                if (n == 0)
                {
                    if (j < P_int_l - 1 && Convert.ToInt32(four.Substring(j + 1, 1)) > 0 && !result.EndsWith(pArrayNum[n]))
                        result += pArrayNum[n];
                }
                else
                {
                    if (!(n == 1 && result.EndsWith(pArrayNum[0]) | (result.Length == 0) && j == P_int_l - 2))
                        result += pArrayNum[n];
                    result += pArrayDigit[P_int_l - j - 1];
                }
            }
            finger += pIntL;

            if (i < pIntK)
            {
                if (Convert.ToInt32(four) != 0)
                    result += pArrayUnits[i - 1];
            }
            else
            {
                //处理最高位的一组,最后必须加上单位
                result += pArrayUnits[i - 1];
            }
        }
        return result;
    }
}