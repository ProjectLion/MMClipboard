/*🏷️----------------------------------------------------------------
 *📄 文件名：DownloadService.cs
 *🏷️
 *👨🏽‍💻 创建者：Ht
 *⏱️ 创建时间：2023-12-22 10:01:45
 *🏷️----------------------------------------------------------------*/


using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;


namespace HtKit;

public delegate void DownloadStartedChangedEventHandler(DownloadStartedEventArgs e);

public delegate void DownloadProgressChangedEventHandler(ProgressChangedEventArgs e);

public delegate void DownloadCompletedEventHandler();

public delegate void DownloadFailedEventHandler(DownloadFailedEventArgs e);

public class DownloadService
{
    private static HttpClient httpClient
    {
        get
        {
            _httpClient ??= new HttpClient();
            return _httpClient;
        }
    }
    private static HttpClient _httpClient;

    private readonly string m_downloadUrl;
    private readonly string m_savePath;
    private DownloadConfig m_config;

    public DownloadProgressChangedEventHandler progressChanged;
    public DownloadStartedChangedEventHandler downloadStarted;
    public DownloadCompletedEventHandler downloadCompleted;
    public DownloadFailedEventHandler downloadFailed;

    public DownloadService(string downloadUrl, string savePath, DownloadConfig config)
    {
        m_downloadUrl = downloadUrl;
        m_savePath = savePath;
        m_config = config ?? new DownloadConfig();
    }

    // 创建 CancellationTokenSource
    private CancellationTokenSource cts = new();

    // 总文件大小
    private long totalFileSize;

    // 一秒钟下载的数据量
    private long oneSecondReadBytes;

    // 瞬时速度
    private double instantSpeed;

    // 平均速度
    private double averageSpeed;
    private Stopwatch stopwatch;

    public async Task StartDownloadAsync()
    {
        await Task.Run(async () =>
        {
            if (m_config.RangeDownload) // 断点下载
                await DownloadRange();
            else
                await Download();
        });
    }

    /// <summary>
    /// 直接下载
    /// </summary>
    /// <returns></returns>
    private async Task Download()
    {
        try
        {
            oneSecondReadBytes = 0;
            instantSpeed = 0;
            long downloadedSize = 0;
            stopwatch = null;
            using var response = await httpClient.GetAsync(m_downloadUrl, HttpCompletionOption.ResponseHeadersRead, cts.Token);
            response.EnsureSuccessStatusCode();
            // 文件总大小
            totalFileSize = response.Content.Headers.ContentLength ?? 0;
            // 读取文件
            await using var contentStream = await response.Content.ReadAsStreamAsync();
            await using var fileStream = new FileStream(m_savePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite, m_config.BufferBlockSize, true);

            // 开始下载回调
            Application.Current.Dispatcher.Invoke(() =>
            {
                downloadStarted?.Invoke(new DownloadStartedEventArgs() { fileName = m_savePath, fileSize = totalFileSize });
            });

            var buffer = new byte[m_config.BufferBlockSize];
            int bytesRead;
            var startTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            stopwatch = Stopwatch.StartNew();
            while ((bytesRead = await contentStream.ReadAsync(buffer)) > 0)
            {
                // Check for cancellation
                cts.Token.ThrowIfCancellationRequested();
                await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead));
                oneSecondReadBytes += bytesRead;
                downloadedSize += bytesRead;
                UpdateProgressAndSpeed(downloadedSize, downloadedSize, startTime);
            }
            stopwatch?.Stop();
            fileStream.Close();

            Application.Current.Dispatcher.Invoke(() =>
            {
                downloadCompleted?.Invoke();
            });
        }
        catch (Exception ex)
        {
            if (File.Exists(m_savePath))
                File.Delete(m_savePath);
            downloadFailed?.Invoke(new DownloadFailedEventArgs() { error = ex is OperationCanceledException ? "" : ex.Message });
        }
    }

    /// <summary>
    /// 断点下载
    /// </summary>
    /// <returns></returns>
    private async Task DownloadRange()
    {
        // 已下载的文件大小
        long downloadedSize = 0;
        // 当前从服务器接受的大小
        long currentReadSize = 0;

        var fileExtension = Path.GetExtension(m_savePath);

        var tempFilePath = m_savePath.Replace(fileExtension, ".temp");

        // 读取旧文件
        // 检查文件是否已经存在，如果存在，则获取已下载的字节数
        var existFile = File.Exists(tempFilePath);
        if (existFile)
        {
            FileInfo fileInfo = new(tempFilePath);
            downloadedSize = fileInfo.Length;
        }

        try
        {
            oneSecondReadBytes = 0;
            instantSpeed = 0;
            stopwatch = null;
            // 创建HttpRequestMessage对象
            var request = new HttpRequestMessage(HttpMethod.Get, m_downloadUrl);
            // 设置断点续传的起始位置
            request.Headers.Range = new RangeHeaderValue(downloadedSize, null);

            using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cts.Token);
            response.EnsureSuccessStatusCode();
            // 文件总大小
            totalFileSize = (response.Content.Headers.ContentLength ?? 0) + downloadedSize;

            // 读取数据流
            await using var contentStream = await response.Content.ReadAsStreamAsync();
            await using var fileStream = new FileStream(tempFilePath, existFile ? FileMode.Append : FileMode.Create, FileAccess.Write, FileShare.ReadWrite,
                m_config.BufferBlockSize,
                true);

            // 开始下载回调
            downloadStarted?.Invoke(new DownloadStartedEventArgs() { fileName = m_savePath, fileSize = totalFileSize });

            var buffer = new byte[m_config.BufferBlockSize];
            int bytesRead;
            var startTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            stopwatch = Stopwatch.StartNew();
            while ((bytesRead = await contentStream.ReadAsync(buffer)) > 0)
            {
                cts.Token.ThrowIfCancellationRequested();
                await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead));
                oneSecondReadBytes += bytesRead;
                downloadedSize += bytesRead;
                currentReadSize += bytesRead;
                UpdateProgressAndSpeed(downloadedSize, currentReadSize, startTime);
            }
            stopwatch?.Stop();
            fileStream.Close();
            // 下载完成修改文件名
            try
            {
                File.Move(tempFilePath, m_savePath);
                downloadCompleted?.Invoke();
            }
            catch (Exception e)
            {
                e.Message.Debug();
                downloadFailed?.Invoke(new DownloadFailedEventArgs() { error = e.Message });
            }
        }
        catch (Exception ex)
        {
            downloadFailed?.Invoke(new DownloadFailedEventArgs() { error = ex is OperationCanceledException ? "" : ex.Message });
        }
    }

    /// <summary>
    /// 取消下载
    /// </summary>
    public void Cancel()
    {
        Task.Run(() =>
        {
            cts?.Cancel();
        });
    }

    public void Pause()
    { }

    /// <summary>
    /// 更新进度和下载速度
    /// </summary>
    /// <param name="totalBytesRead"></param>
    /// <param name="downloadedSize"></param>
    /// <param name="startTime"></param>
    private void UpdateProgressAndSpeed(long totalBytesRead, long downloadedSize, long startTime)
    {
        var progress = (double)totalBytesRead / totalFileSize;
        var utcnow = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        // 每100毫秒计算瞬时速度
        if (stopwatch.ElapsedMilliseconds >= 1000)
        {
            // 瞬时速度
            instantSpeed = oneSecondReadBytes / (stopwatch.Elapsed.TotalMilliseconds / 1000);
            // 重新计时
            oneSecondReadBytes = 0;
            stopwatch.Restart();
        }
        // 总时长
        var totalTime = utcnow - startTime;
        // 平均速度
        averageSpeed = downloadedSize / (totalTime / 1000.0);

        //$"进度: {progress} ,瞬时速度: {instantSpeed / 1024 / 1024} MB/s, 平均速度: {averageSpeed / 1024 / 1024} MB/s".Debug();

        progressChanged?.Invoke(new ProgressChangedEventArgs
        {
            progress = progress,
            instantSpeed = instantSpeed,
            averageSpeed = averageSpeed,
            readBytes = totalBytesRead
        });
    }
}