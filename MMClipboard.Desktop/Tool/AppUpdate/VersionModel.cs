/*🏷️----------------------------------------------------------------
 *📄 文件名：VersionModel.cs
 *🏷️
 *👨🏽‍💻 创建者：Ht
 *⏱️ 创建时间：2023/12/15 17:32:43
 *🏷️----------------------------------------------------------------*/


namespace MMClipboard.Tool.AppUpdate;

public class VersionModel
{
    /// <summary>
    /// 版本号
    /// </summary>
    public string version { get; set; } = "1.0.0";

    /// <summary>
    /// 更新说明
    /// </summary>
    public string updateMsg { get; set; } = "";

    /// <summary>
    /// 下载地址
    /// </summary>
    public string updateUrl_GitHub { get; set; } = "";

    /// <summary>
    /// 下载地址
    /// </summary>
    public string updateUrl_Gitee { get; set; } = "";

    /// <summary>
    /// 更新时间
    /// </summary>
    public string updateTime { get; set; } = "2023-12-12";

    /// <summary>
    /// 文件大小
    /// </summary>
    public long fileSize { get; set; } = 0;
}