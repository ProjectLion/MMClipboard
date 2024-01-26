/*🏷️----------------------------------------------------------------
 *📄 文件名：DownloadConfig.cs
 *🏷️
 *👨🏽‍💻 创建者：Ht
 *⏱️ 创建时间：2023-12-22 10:01:45
 *🏷️----------------------------------------------------------------*/


using System;


namespace HtKit;

public class DownloadConfig
{
    private int _chunkCount = 1;
    private long _maximumBytesPerSecond = long.MaxValue;
    private int _parallelCount;

    /// <summary>
    /// 数据缓冲区的buffer大小 默认1024 * 50
    /// </summary>
    public int BufferBlockSize { get; set; } = 1024 * 50;

    /// <summary>
    /// 下载前检查磁盘空间 默认true
    /// </summary>
    public bool CheckDiskSizeBeforeDownload { get; set; } = true;

    /// <summary>
    /// 切片数量 默认为1(不切片)
    /// </summary>
    public int ChunkCount
    {
        get => _chunkCount;
        set => _chunkCount = Math.Max(1, value);
    }

    /// <summary>
    /// 最大的下载速度 默认无限制
    /// </summary>
    public long MaximumBytesPerSecond
    {
        get => _maximumBytesPerSecond;
        set => _maximumBytesPerSecond = value <= 0 ? long.MaxValue : value;
    }

    /// <summary>
    /// 重试次数 默认无限制
    /// </summary>
    public int MaxTryAgainOnFail { get; set; } = int.MaxValue;

    /// <summary>
    /// 是否并行下载 默认为false
    /// </summary>
    public bool ParallelDownload { get; set; } = false;

    /// <summary>
    /// 并行任务数量 默认为0
    /// </summary>
    public int ParallelCount
    {
        get => _parallelCount <= 0 ? ChunkCount : _parallelCount;
        set => _parallelCount = value;
    }

    /// <summary>
    /// 开启断点下载
    /// </summary>
    public bool RangeDownload { get; set; } = false;

    /// <summary>
    /// 起始范围 默认(0,0)
    /// </summary>
    public (long, long) Range { get; set; } = (0, 0);

    /// <summary>
    /// 下载超时时间(毫秒) 默认1000
    /// </summary>
    public int Timeout { get; set; } = 1000;
}