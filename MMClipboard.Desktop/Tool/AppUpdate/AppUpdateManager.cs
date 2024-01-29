/*🏷️----------------------------------------------------------------
 *📄 文件名：AppUpdateManager.cs
 *🏷️
 *👨🏽‍💻 创建者：Ht
 *⏱️ 创建时间：2023/12/15 17:31:24
 *🏷️----------------------------------------------------------------*/


using System;
using System.Threading.Tasks;
using System.Windows;
using HtKit;
using HtKit.NetWorking;
using MMClipboard.UserConfigs;
using Newtonsoft.Json;


namespace MMClipboard.Tool.AppUpdate;

public abstract class AppUpdateManager
{
    public static async void Check(Action<bool, VersionModel> ac)
    {
        var localV = ReadLocalVersionNo();
        Version local = new(localV);
        Version remote;
        await Task.Run(() =>
        {
            GetVersionInfo((mod) =>
            {
                remote = new Version(mod.version);
                var result = local.CompareTo(remote);
                if (result < 0)
                {
                    "需要更新".Debug();
                    Dispatch.BackToMainThreadAsync(() => ac?.Invoke(true, mod));
                }
                else
                {
                    "不需要更新".Debug();
                    Dispatch.BackToMainThreadAsync(() => ac?.Invoke(false, mod));
                }
            });
        });
    }

    private static void GetVersionInfo(Action<VersionModel> action)
    {
        var url = UserConfig.Default.config.updatePlace == 0
            ? "https://raw.githubusercontent.com/ProjectLion/MMClipboard/main/VersionInfo.json"
            : "https://gitee.com/HtReturnTrue/MMClipboard/raw/main/VersionInfo.json";
        NetHelper.GetResponse(url, (data) =>
        {
            action?.Invoke(JsonConvert.DeserializeObject<VersionModel>(data));
        }, (err) =>
        {
            err.Log();
        });
    }

    public static string ReadLocalVersionNo()
    {
        var version = Application.ResourceAssembly.GetName().Version;
        return version is null ? "1.0.0.0" : version.ToString(4);
    }
}