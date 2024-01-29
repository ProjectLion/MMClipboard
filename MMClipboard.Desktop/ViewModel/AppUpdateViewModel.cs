/*🏷️----------------------------------------------------------------
 *📄 文件名：AppUpdateViewModel.cs
 *🏷️
 *👨🏽‍💻 创建者：Ht
 *⏱️ 创建时间：2023/12/18 16:20:36
 *🏷️----------------------------------------------------------------*/


using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using Escher.Tool;
using HtKit;
using MMClipboard.Tool.AppUpdate;
using MMClipboard.UserConfigs;
using MMClipboard.View;


namespace MMClipboard.ViewModel;

public class AppUpdateViewModel : ObservableObject
{
    public string versionInfo
    {
        get => _versionInfo;
        private set => SetProperty(ref _versionInfo, value);
    }
    private string _versionInfo;

    public string downloadSize
    {
        get => _downloadSize;
        private set => SetProperty(ref _downloadSize, value);
    }
    private string _downloadSize = "0Mb/0Mb";

    public string downloadSpeed
    {
        get => _downloadSpeed;
        private set => SetProperty(ref _downloadSpeed, value);
    }
    private string _downloadSpeed = "0MB/s";

    public string downloadTime
    {
        get => _downloadTime;
        private set => SetProperty(ref _downloadTime, value);
    }
    private string _downloadTime = "23:59:59";

    public double downloadProgress
    {
        get => _downloadProgress;
        set => SetProperty(ref _downloadProgress, value);
    }
    private double _downloadProgress;

    private AppUpdateWindow bindWindow;

    private VersionModel versionDataModel;

    private DownloadService downloadService;

    public AppUpdateViewModel(AppUpdateWindow window, VersionModel mod)
    {
        bindWindow = window;

        versionDataModel = mod;
        versionInfo = $"版本号:  {mod.version}\n更新包大小:  {mod.fileSize / 1024f / 1024:F1}MB\n更新时间:  {mod.updateTime}\n更新内容:  {mod.updateMsg}";
        downloadSize = $"0MB/{mod.fileSize.Ht_ConvertToMB()}";
    }

    #region methods

    public async void Update()
    {
        bindWindow.updateTip.Visibility = Visibility.Collapsed;
        bindWindow.downloadInfoGrid.Visibility = Visibility.Visible;
        // 下载
        var downloadToF = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Temp");
        if (!Directory.Exists(downloadToF))
            Directory.CreateDirectory(downloadToF);

        var url = UserConfig.Default.config.updatePlace == 0 ? versionDataModel.updateUrl_GitHub : versionDataModel.updateUrl_Gitee;
        var fp = Path.Combine(downloadToF, "temp.zip");
        long totalSize = 0;

        downloadService = new DownloadService(url, fp, new DownloadConfig());
        downloadService.downloadStarted += (e) =>
        {
            totalSize = e.fileSize;
            downloadSize = $"0MB/{e.fileSize.Ht_ConvertToMB()}";
        };
        downloadService.progressChanged += (e) =>
        {
            downloadSpeed = $"{e.instantSpeed.Ht_ConvertToMB()}/s";
            downloadSize = $"{e.readBytes.Ht_ConvertToMB()}/{totalSize.Ht_ConvertToMB()}";
            downloadProgress = e.progress * 100;

            if (e.instantSpeed <= 0)
            {
                downloadTime = "23:59:59";
            }
            else
            {
                var s = (totalSize - e.readBytes) / (long)e.instantSpeed;
                downloadTime = s.Ht_SecondConvertToHMS();
            }
        };
        downloadService.downloadCompleted += () =>
        {
            Dispatch.BackToMainThreadAsync(() =>
            {
                bindWindow.downloadInfoGrid.Visibility = Visibility.Hidden;
                bindWindow.installingText.Visibility = Visibility.Visible;
                bindWindow.installingText.Text = "下载完成\n正在安装...";
                ExtractZip(fp, downloadToF);
            });
        };
        downloadService.downloadFailed += (e) =>
        {
            e.error.Log();
            Dispatch.BackToMainThreadAsync(() =>
            {
                bindWindow.downloadInfoGrid.Visibility = Visibility.Hidden;
                bindWindow.installingText.Visibility = Visibility.Visible;
                bindWindow.installingText.Text = "下载失败";
            });
        };
        await downloadService.StartDownloadAsync();
    }

    public void Cancel()
    {
        downloadService?.Cancel();
    }

    #endregion methods

    /// <summary>
    /// 解压
    /// </summary>
    /// <param name="fileP">文件地址</param>
    /// <param name="toP">目标文件夹</param>
    private async void ExtractZip(string fileP, string toP)
    {
        await HtZipHelper.Unzip(fileP, toP, (currentI, total) =>
            {
                Dispatch.BackToMainThreadSync(() =>
                {
                    bindWindow.installingText.Text = $"下载完成\n正在安装 {(float)currentI / total * 100:F0}%";
                });
            }, () =>
            { }, (err) =>
            {
                err.Log();
                Dispatch.BackToMainThreadSync(() =>
                {
                    bindWindow.installingText.Text = $"安装出错，请重试\n{err}。";
                });
            });
        // 把更新程序移到项目根目录下
        var updateToolP = Path.Combine(toP, "UpdateTool.exe");
        if (File.Exists(updateToolP))
            File.Move(updateToolP, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UpdateTool.exe"), true);
        if (File.Exists(fileP))
            File.Delete(fileP);
        bindWindow.installingText.Text = "安装完成\n将自动重启应用";

        await Task.Delay(1000);
        SharedInstance.Instance.isNeedUpdate = true;
        Application.Current.Shutdown();
    }
}

public static class LongExtension
{
    public static string Ht_SecondConvertToHMS(this long duration)
    {
        switch (duration)
        {
            case > 24 * 60 * 60:
                return "23:59:59";
            case <= 0:
                return "00:00:00";
        }

        TimeSpan ts = new(0, 0, Convert.ToInt32(duration));
        var str = "";
        if (ts.Hours > 0)
            str = $"{ts.Hours:00}" + ":" + $"{ts.Minutes:00}" + ":" + $"{ts.Seconds:00}";
        if (ts is { Hours: 0, Minutes: > 0 })
            str = "00:" + $"{ts.Minutes:00}" + ":" + $"{ts.Seconds:00}";
        if (ts is { Hours: 0, Minutes: 0 })
            str = "00:00:" + $"{ts.Seconds:00}";
        return str;
    }
}

public static class DoubleExtension
{
    /// <summary>
    /// 保留n位小数（默认保留2位）
    /// </summary>
    /// <param name="target"></param>
    /// <param name="format"></param>
    /// <returns></returns>
    public static string Ht_ConvertTo(this double target, int format = 2)
    {
        return $"{target.ToString($"F{format}")}";
    }

    /// <summary>
    /// 转换单位
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public static string Ht_ConvertToMB(this long target)
    {
        if (target == 0)
            return "0KB";

        string output;
        if (target >= 1024 * 1024 * 1024)
        {
            var gb = target / 1024f / 1024 / 1024;
            output = $"{gb:F1}GB";
        }
        else if (target >= 1024 * 1024)
        {
            var mb = target / 1024f / 1024;
            output = $"{mb:F1}MB";
        }
        else if (target >= 1024)
        {
            var kb = target / 1024f;
            output = $"{kb:F1}KB";
        }
        else
        {
            output = $"{target:F1}Bytes";
        }
        return output;
    }

    /// <summary>
    /// 转换单位
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public static string Ht_ConvertToMB(this double target)
    {
        if (target == 0)
            return "0KB";

        string output;
        if (target >= 1024 * 1024 * 1024)
        {
            var gb = target / 1024f / 1024 / 1024;
            output = $"{gb:F1}GB";
        }
        else if (target >= 1024 * 1024)
        {
            var mb = target / 1024f / 1024;
            output = $"{mb:F1}MB";
        }
        else if (target >= 1024)
        {
            var kb = target / 1024f;
            output = $"{kb:F1}KB";
        }
        else
        {
            output = $"{target:F1}Bytes";
        }
        return output;
    }
}