/*🏷️----------------------------------------------------------------
 *📄 文件名：CacheHelper.cs
 *🏷️
 *👨🏽‍💻 创建者：Ht
 *⏱️ 创建时间：2023/12/7 17:08:35
 *🏷️----------------------------------------------------------------*/


using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using HtKit;
using MMClipboard.Model;


namespace MMClipboard.Tool;

public static class CacheHelper
{
    private static string CacheDir(DateTime date)
    {
        var dp = Path.Combine(AppPath.GetBaseDirectory(), $"Cache\\{date:yyyyMMdd}");
        if (!Directory.Exists(dp))
            Directory.CreateDirectory(dp);
        return dp;
    }

    private static string GetCacheImagePath(DateTime date)
    {
        var dp = Path.Combine(AppPath.GetBaseDirectory(), $"Cache\\{date:yyyyMMdd}");
        if (!Directory.Exists(dp))
            Directory.CreateDirectory(dp);
        var timestamp = (long)(date - new DateTime(1970, 1, 1)).TotalMilliseconds;
        var dbFilePath = Path.Combine(dp, $"mm_{timestamp}.png");
        return dbFilePath;
    }

    /// <summary>
    /// 复制文件
    /// Copy file
    /// </summary>
    /// <param name="originFilePath"></param>
    /// <param name="date"></param>
    /// <returns></returns>
    public static string CopyFile(string originFilePath, DateTime date)
    {
        var fileName = Path.GetFileName(originFilePath);
        var cacheP = Path.Combine(AppPath.GetBaseDirectory(), $"{CacheDir(date)}\\{fileName}");
        if (File.Exists(cacheP))
            return cacheP;
        try
        {
            File.Copy(originFilePath, cacheP);
            // 设置文件权限为可读可写，否则在删除父级文件夹时会报错
            File.SetAttributes(cacheP, FileAttributes.Normal);
            return cacheP;
        }
        catch (Exception e)
        {
            e.Debug();
            return originFilePath;
        }
    }

    /// <summary>
    /// 保存图片
    /// </summary>
    /// <param name="bitmap"></param>
    /// <param name="date"></param>
    public static string SaveImage(BitmapSource bitmap, DateTime date)
    {
        var p = GetCacheImagePath(date);
        var encoder = new BmpBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(bitmap));

        // 创建一个文件流以保存 PNG 文件
        using var stream = new FileStream(p, FileMode.Create);
        encoder.Save(stream);
        stream.Flush();
        return p;
    }

    /// <summary>
    /// 清空某一天的缓存
    /// Clear the cache for a date
    /// </summary>
    /// <param name="date"></param>
    public static void ClearCache(DateTime date)
    {
        try
        {
            if (date == DateTime.MinValue)
            {
                Directory.Delete(Path.Combine(AppPath.GetBaseDirectory(), "Cache"), true);
            }
            else
            {
                var dp = Path.Combine(AppPath.GetBaseDirectory(), $"Cache\\{date:yyyyMMdd}");
                if (!Directory.Exists(dp)) return;
                Directory.Delete(dp, true);
            }
        }
        catch (Exception e)
        {
            e.Message.Log();
        }
    }

    /// <summary>
    /// 删除缓存图片
    /// </summary>
    /// <param name="item"></param>
    public static void DeleteCacheImage(ClipItemModel item)
    {
        if (item.clipType is not ClipType.Image)
            return;
        // 只删除缓存到程序目录下的图片文件,其他的用户的图片文件不能去动
        if (!item.content.Contains(@"Cache\") || !Path.GetFileName(item.content).Contains("mm_"))
            return;
        try
        {
            File.Delete(item.content);
        }
        catch (Exception e)
        {
            e.Log();
        }
    }

    /// <summary>
    /// 加载本地缓存图片
    /// </summary>
    /// <param name="path"></param>
    /// <param name="ac"></param>
    /// <returns></returns>
    public static void LoadCacheImage(string path, Action<BitmapImage> ac)
    {
        var p = path;
        if (!File.Exists(p))
        {
            ac?.Invoke(new BitmapImage(new Uri("/Images/defaultImage.png", UriKind.Relative)));
            return;
        }
        Task.Run(() =>
        {
            using var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            var buffer = new byte[fileStream.Length];
            _ = fileStream.Read(buffer, 0, (int)fileStream.Length);
            fileStream.Flush();
            fileStream.Close();
            LoadImageAsync(buffer, ac);
        });
    }

    private static void LoadImageAsync(byte[] buffer, Action<BitmapImage> ac)
    {
        // InvokeAsync()方法用于异步执行，防止在UI线程阻塞。
        // 不能使用Await，否则会阻塞UI线程。
        Application.Current.Dispatcher.InvokeAsync(() =>
        {
            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnDemand;
                bitmap.StreamSource = new MemoryStream(buffer);
                bitmap.DecodePixelHeight = 200;
                bitmap.EndInit();
                if (bitmap.CanFreeze)
                    bitmap.Freeze();
                ac?.Invoke(bitmap);
            }
            catch (Exception e)
            {
                e.Log();
                ac?.Invoke(new BitmapImage(new Uri("/Images/defaultImage.png", UriKind.Relative)));
            }
        });
    }
}