/*🏷️----------------------------------------------------------------
 *📄 文件名：ClipItemModel.cs
 *🏷️
 *👨🏽‍💻 创建者：Ht
 *⏱️ 创建时间：2023/9/15 11:17:56
 *🏷️----------------------------------------------------------------*/


using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using FreeSql.DataAnnotations;
using MMClipboard.Tool;


namespace MMClipboard.Model;

/// <summary>
/// from、clipType、content、isSelected
/// </summary>
[Table(Name = "main")]
public class ClipItemModel : ObservableObject
{
    [Column(IsIdentity = true)]
    public int id
    {
        get => _id;
        init => SetProperty(ref _id, value);
    }
    private int _id;

    public string from
    {
        get
        {
            LoadImageAsync();
            return TransformExeName(_from);
        }
        init => SetProperty(ref _from, value);
    }
    private string _from = "";

    public string fromExeImgPath
    {
        get => _fromExeImgPath;
        init => SetProperty(ref _fromExeImgPath, value);
    }
    private string _fromExeImgPath = "";

    public ClipType clipType
    {
        get => _clipType;
        set => SetProperty(ref _clipType, value);
    }
    private ClipType _clipType = ClipType.Text;

    /// <summary>
    /// 0 false / 1 true
    /// </summary>
    public int collect
    {
        get => _collect;
        set => SetProperty(ref _collect, value);
    }
    private int _collect;

    [Column(StringLength = 10000)] public string content { get; set; } = "";

    [Column(StringLength = 10000)] public string rtfContent { get; set; } = "";

    public DateTime date
    {
        get => _date;
        init
        {
            dateStr = value.ToString("yyyy-MM-dd HH:mm");
            SetProperty(ref _date, value);
        }
    }
    private DateTime _date = DateTime.Now;

    /// <summary>
    /// 用户copy的图片
    /// </summary>
    public BitmapImage image
    {
        get => _image;
        private set => SetProperty(ref _image, value);
    }
    private BitmapImage _image;

    public BitmapSource exeIcon
    {
        get => _exeIcon;
        private set => SetProperty(ref _exeIcon, value);
    }
    private BitmapSource _exeIcon;

    public BitmapSource fileIcon
    {
        get => _fileIcon;
        private set => SetProperty(ref _fileIcon, value);
    }
    private BitmapSource _fileIcon;

    [Column(IsIgnore = true)] public string dateStr { get; private set; }

    /// <summary>
    /// 程序名称转换
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    private static string TransformExeName(string name)
    {
        return name switch
        {
            "devenv" => "Visual Studio",
            "Code" => "VS Code",
            "DingTalk" => "钉钉",
            "WeChat" => "微信",
            "SnippingTool" => "截图工具",
            "ScreenClippingHost" => "截图工具",
            "explorer" => "文件管理器",
            "ApplicationFrameHost" => "设置",
            "DingTalkSnippingTool" => "钉钉截图",
            "ColorPicker" => "取色器",
            "chrome" => "Chrome",
            "msedge" => "Edge",
            "WindowsTerminal" => "Cmd终端",
            "Taskmgr" => "任务管理器",
            "WeChatAppEx" => "微信公众号",
            "regedit" => "注册表",
            "rider64" => "JetBrains Rider",
            "MMClipboard" => "妙剪记",
            "Notepad" => "记事本",
            "wps" => "WPS",
            _ => name
        };
    }

    /// <summary>
    /// 提前异步加载图片
    /// </summary>
    private void LoadImageAsync()
    {
        LoadIconAsync();
        // 类型不是图片且图片已加载 直接return
        if (clipType is not ClipType.Image || _image is not null)
            return;
        // 加载用户复制的图片
        CacheHelper.LoadCacheImage(content, (img) =>
        {
            if (img != null)
                image = img;
        });
    }

    /// <summary>
    /// 加载图标
    /// </summary>
    public void LoadIconAsync()
    {
        // LoadImageAsync();
        // 加载程序图标
        if (exeIcon == null)
            LoadExeIcon(fromExeImgPath, source => exeIcon = source);

        // 加载文件图标
        if (clipType is ClipType.File && fileIcon == null)
            LoadFileIcon(content, source => fileIcon = source);
    }

    /// <summary>
    /// 加载程序图标
    /// </summary>
    /// <param name="p"></param>
    /// <param name="ac"></param>
    private static void LoadExeIcon(string p, Action<BitmapSource> ac)
    {
        if (string.IsNullOrEmpty(p))
        {
            ac?.Invoke(new BitmapImage(new Uri("/Images/Exe.png", UriKind.Relative)));
            return;
        }
        Application.Current.Dispatcher.InvokeAsync(() =>
        {
            try
            {
                var bf = Icon.ExtractAssociatedIcon(p);
                if (bf == null)
                {
                    ac?.Invoke(new BitmapImage(new Uri("/Images/Exe.png", UriKind.Relative)));
                    return;
                }
                var res = Imaging.CreateBitmapSourceFromHIcon(bf.Handle, new Int32Rect(0, 0, bf.Width, bf.Height), BitmapSizeOptions.FromEmptyOptions());
                ac?.Invoke(res);
            }
            catch (Exception e)
            {
                e.Message.Log();
                ac?.Invoke(new BitmapImage(new Uri("/Images/Exe.png", UriKind.Relative)));
            }
        });
    }

    /// <summary>
    /// 加载文件图标
    /// </summary>
    /// <param name="p"></param>
    /// <param name="ac"></param>
    private static void LoadFileIcon(string p, Action<BitmapSource> ac)
    {
        Application.Current.Dispatcher.InvokeAsync(() =>
        {
            const string name = "pack://application:,,,/Images/FileDefaultLogo.png";
            var bitmapImage = new BitmapImage(new Uri(name, UriKind.Absolute));
            if (string.IsNullOrEmpty(p))
            {
                ac?.Invoke(bitmapImage);
                return;
            }
            // 是文件夹
            if (Directory.Exists(p))
            {
                var r = Directory.Exists(p) ? new BitmapImage(new Uri("pack://application:,,,/Images/DirectoryDefaultLogo.png", UriKind.Absolute)) : bitmapImage;
                ac?.Invoke(r);
                return;
            }

            // 文件不存在
            if (!File.Exists(p))
            {
                ac?.Invoke(bitmapImage);
                return;
            }

            try
            {
                var bf = Icon.ExtractAssociatedIcon(p);
                if (bf == null)
                {
                    ac?.Invoke(bitmapImage);
                    return;
                }
                var res = Imaging.CreateBitmapSourceFromHIcon(bf.Handle, new Int32Rect(0, 0, bf.Width, bf.Height), BitmapSizeOptions.FromEmptyOptions());
                ac?.Invoke(res);
            }
            catch (Exception e)
            {
                e.Message.Log();
                ac?.Invoke(bitmapImage);
            }
        });
    }
}