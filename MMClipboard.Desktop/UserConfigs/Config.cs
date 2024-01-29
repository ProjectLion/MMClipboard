/*🏷️----------------------------------------------------------------
 *📄 文件名：UserConfig.cs
 *🏷️
 *👨🏽‍💻 创建者：Ht
 *⏱️ 创建时间：2023/12/7 10:23:40
 *🏷️----------------------------------------------------------------*/


using System;
using System.IO;
using System.Windows.Input;
using System.Windows.Media.Imaging;


namespace MMClipboard.UserConfigs;

public class Config
{
    /// <summary>
    /// 开机启动 默认true
    /// </summary>
    public bool isAutoStart { get; set; } = true;

    /// <summary>
    /// 启动后最小化 默认false
    /// </summary>
    public bool isStartMinimize { get; set; }

    /*
      暂时去掉该功能，我觉得这个功能不是那么必要。
     /// <summary>
    /// 在复制文件时是否复制一份备份到程序目录 默认true
    /// </summary>
    public bool isCopyFile { get; set; } = true;
     */

    /// <summary>
    /// 是否使用小窗口 默认true
    /// </summary>
    public bool isSmall { get; set; } = true;

    /// <summary>
    /// 复制完成后是否关闭窗口 默认true
    /// </summary>
    public bool isCopiedClose { get; set; } = true;

    /// <summary>
    /// 页面背景是否使用高斯模糊图片 默认true
    /// </summary>
    public bool isUseBackgroundImg { get; set; } = true;

    /// <summary>
    /// 页面背景图片
    /// </summary>
    public string pageBackgroundImg { get; set; } = string.Empty;

    /// <summary>
    /// 页面背景色 默认#181818
    /// </summary>
    public string pageBackgroundColor { get; set; } = "#181818";

    /// <summary>
    /// 快捷键 默认Alt+C
    /// </summary>
    public ModifierKeys modifierKeys { get; set; } = ModifierKeys.Alt;

    public Key key { get; set; } = Key.C;

    /// <summary>
    /// 更新渠道。0：GitHub 1：Gitee。默认为GitHub
    /// </summary>
    public int updatePlace { get; set; } = 0;

    /// <summary>
    /// 背景图
    /// </summary>
    public BitmapImage bgImgSource
    {
        get
        {
            const string filename = "pack://application:,,,/Images/DefaultBg.png";
            var uri = new Uri(filename, UriKind.Absolute);
            try
            {
                if (isUseBackgroundImg && !pageBackgroundImg.Ht_IsEmpty() && File.Exists(pageBackgroundImg))
                {
                    var image = new BitmapImage(new Uri(pageBackgroundImg, UriKind.Absolute));
                    return image;
                }
            }
            catch (Exception e)
            {
                e.Message.Debug();
            }
            var bmp = new BitmapImage(uri);
            return bmp;
        }
    }
}