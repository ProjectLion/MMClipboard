/*🏷️----------------------------------------------------------------
 *📄 文件名：Downloader.cs
 *🏷️
 *👨🏽‍💻 创建者：Ht
 *⏱️ 创建时间：2023-12-22 10:01:45
 *🏷️----------------------------------------------------------------*/


using System.Threading.Tasks;


namespace HtKit;

public abstract class Downloader
{
    /// <summary>
    /// 默认的下载方式
    /// </summary>
    /// <param name="url"></param>
    /// <param name="path"></param>
    /// <param name="started"></param>
    /// <param name="progressChanged"></param>
    /// <param name="completed"></param>
    /// <param name="failed"></param>
    /// <param name="config"></param>
    /// <returns></returns>
    public static Task DownloadFile(string url, string path, DownloadStartedChangedEventHandler started, DownloadProgressChangedEventHandler progressChanged,
        DownloadCompletedEventHandler completed, DownloadFailedEventHandler failed, DownloadConfig config = default)
    {
        DownloadService service = new(url, path, config);
        service.downloadStarted += started;
        service.progressChanged += progressChanged;
        service.downloadCompleted += completed;
        service.downloadFailed += failed;
        return service.StartDownloadAsync();
    }
}