/*🏷️----------------------------------------------------------------
 *📄 文件名：StringExtension.cs
 *🏷️
 *👨🏽‍💻 创建者：Ht
 *⏱️ 创建时间：2023-12-22 10:01:45
 *🏷️----------------------------------------------------------------*/


using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;


/// <summary>
/// String Extension
/// </summary>
public static partial class StringExtension
{
    /// <summary>
    /// 是否空字符串
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public static bool Ht_IsEmpty(this string target)
    {
        return string.IsNullOrEmpty(target) || target.Equals(" ");
    }

    /// <summary>
    /// 移除文件路径中包含一些特殊符号导致本地文件路径加载出错(原因是UrlDecode默认将 + 解析为空格了) "+" "#" "=" 号。
    /// </summary>
    public static string Ht_EncodeFilePath(this string target)
    {
        // 遇到本地文件命名中 含有 "+"号，使用UnityWebRequest加载本地图片时 "+"号被变成空格导致加载出错的问题。
        // "+" 这个符号在http请求中可太他娘的顽强了。弄了一下午才整明白。
        Uri uri = new(target); // 取到
        var uriAbs = uri.AbsoluteUri;
        // 替换 absolute 中的 "+" 号。如果这里不替换的话 会导致url解码时 "+" 被解码变成空格。
        // 所以要将 "+" 替换为url解码 "认识" 的 "%2B"
        var replace = uriAbs.Replace("+", "%2B");
        // 进行Url解码
        var urlDecode = HttpUtility.UrlDecode(replace);

        // 上面的几步是为了将 "E:\xxx\xxxx\xx++xx+.jpg" 转为 "file:///E:/xxx/xxxx/xx++xx+.jpg" 格式

        // 这一步将 url解码后的路径中的"+"号替换为%2B 目前知道的需要替换的有 "+ # ="
        var te1 = urlDecode.Replace("+", "%2B");
        te1 = te1.Replace("#", "%23");
        te1 = te1.Replace("=", "%3D");
        // 再进行url地址的编码。因为在http请求中 request方法会将url地址进行一道decode，所以这里故意将我们的地址进行encode。
        var urlPathEncode = HttpUtility.UrlPathEncode(te1);

        return urlPathEncode;
    }

    /// <summary>
    /// 移除空格
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public static string Ht_RemoveSpace(this string target)
    {
        var s = SpaceRegex().Replace(target, "");
        return s.Replace(" ", "");
    }

    /// <summary>
    /// 获取字节流
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public static byte[] Ht_GetBytes(this string target)
    {
        return Encoding.UTF8.GetBytes(target);
    }

    /// <summary>
    /// base64编码
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public static string Ht_ToBase64String(this string target)
    {
        var b = target.Ht_GetBytes();
        return Convert.ToBase64String(b);
    }

    /// <summary>
    /// 从base64解码
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public static string Ht_Base64ToString(this string target)
    {
        var b = Convert.FromBase64String(target);
        return Encoding.UTF8.GetString(b);
    }

    #region 加密

    // 自定义密钥（必须位8位64字节大小）
    private static readonly string EncryptKey = "MmmT";

    /// <summary>
    /// 加密
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public static string Ht_Encrypt(this string target)
    {
        using var des = DES.Create();
        var keyBt = Encoding.Unicode.GetBytes(EncryptKey);
        var data = Encoding.Unicode.GetBytes(target);
        using MemoryStream MStream = new();
        //使用内存流实例化加密流对象
        using CryptoStream CStream = new(MStream, des.CreateEncryptor(keyBt, keyBt), CryptoStreamMode.Write);
        CStream.Write(data, 0, data.Length);
        CStream.FlushFinalBlock();
        return Convert.ToBase64String(MStream.ToArray());
    }

    /// <summary>
    /// 解密
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public static string Ht_Decrypt(this string target)
    {
        var des = DES.Create();
        var keyBt = Encoding.Unicode.GetBytes(EncryptKey);
        var data = Convert.FromBase64String(target);
        using MemoryStream MStream = new();
        //使用内存流实例化解密流对象
        using CryptoStream CStream = new(MStream, des.CreateDecryptor(keyBt, keyBt), CryptoStreamMode.Write);
        CStream.Write(data, 0, data.Length);
        CStream.FlushFinalBlock();
        return Encoding.Unicode.GetString(MStream.ToArray());
    }

    /// <summary>
    /// MD5加密
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public static string Ht_MD5(this string target)
    {
        if (target == null)
            return "";
        var s = MD5.HashData(Encoding.UTF8.GetBytes(target));
        return s.Aggregate("", (current, t) => current + t.ToString("x2"));
    }

    #endregion 加密

    #region 正则表达相关

    /// <summary>
    /// 是否是数字
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public static bool Ht_IsNumber(this string target)
    {
        return NumberRegex().IsMatch(target);
    }

    /// <summary>
    /// 是否是手机号
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public static bool Ht_IsPhone(this string target)
    {
        return PhoneRegex().IsMatch(target);
    }

    /// <summary>
    /// 是否是邮箱号
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public static bool Ht_IsEmail(this string target)
    {
        return EmailRegex().IsMatch(target);
    }

    /// <summary>
    /// 是否是车牌
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public static bool Ht_IsCarID(this string target)
    {
        return CarIdRegex().IsMatch(target);
    }

    /// <summary>
    /// 是否是银行卡
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public static bool Ht_IsBankCard(this string target)
    {
        return BankCardRegex().IsMatch(target);
    }

    /// <summary>
    /// 是否是网站
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public static bool Ht_IsWebsite(this string target)
    {
        return WebsiteRegex().IsMatch(target);
    }

    [GeneratedRegex("^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$", RegexOptions.Compiled)]
    private static partial Regex EmailRegex();

    [GeneratedRegex(@"^1[3-9]\d{9}$", RegexOptions.Compiled)]
    private static partial Regex PhoneRegex();

    [GeneratedRegex(@"^\d+\b", RegexOptions.Compiled)]
    private static partial Regex NumberRegex();

    [GeneratedRegex(@"^[\u4e00-\u9fa5]{1}[A-Z]{1}D?[A-Z_0-9]{5}$", RegexOptions.Compiled)]
    private static partial Regex CarIdRegex();

    [GeneratedRegex("^62[0-9]{17}$", RegexOptions.Compiled)]
    private static partial Regex BankCardRegex();

    [GeneratedRegex("(https?|ftp|file)://[-A-Za-z0-9+&@#/%?=~_|!:,.;]+[-A-Za-z0-9+&@#/%=~_|]", RegexOptions.Compiled)]
    private static partial Regex WebsiteRegex();

    #endregion 正则表达相关

    /// <summary>
    /// 是否是文件
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public static bool Ht_IsFile(this string target)
    {
        return File.Exists(target);
    }

    /// <summary>
    /// 是否是目录
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public static bool Ht_IsDirectory(this string target)
    {
        return Directory.Exists(target);
    }

    /// <summary>
    /// 是否是图片
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public static bool Ht_IsImage(this string target)
    {
        if (!target.Ht_IsFile()) return false;
        string[] imageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
        var extension = Path.GetExtension(target);
        return imageExtensions.Contains(extension);
    }

    [GeneratedRegex(@"\\s")]
    private static partial Regex SpaceRegex();
}