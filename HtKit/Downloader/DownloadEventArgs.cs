/*🏷️----------------------------------------------------------------
 *📄 文件名：DownloadEventArgs.cs
 *🏷️
 *👨🏽‍💻 创建者：Ht
 *⏱️ 创建时间：2023-12-22 10:01:45
 *🏷️----------------------------------------------------------------*/


namespace HtKit;

public class DownloadStartedEventArgs
{
    /// <summary>
    /// 文件名
    /// </summary>
    public string fileName { get; set; }

    /// <summary>
    /// 文件大小
    /// </summary>
    public long fileSize { get; set; }
}

public class ProgressChangedEventArgs
{
    /// <summary>
    /// 下载进度
    /// </summary>
    public double progress { get; set; }

    /// <summary>
    /// 总下载量
    /// </summary>
    public long readBytes { get; set; }

    /// <summary>
    /// 瞬时速度
    /// </summary>
    public double instantSpeed { get; set; }

    /// <summary>
    /// 平均速度
    /// </summary>
    public double averageSpeed { get; set; }
}

public class DownloadFailedEventArgs
{
    /// <summary>
    /// 错误信息
    /// </summary>
    public string error { get; set; }
}