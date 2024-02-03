/*🏷️----------------------------------------------------------------
 *📄 文件名：WindowBackgroundModel.cs
 *🏷️
 *👨🏽‍💻 创建者：Ht
 *⏱️ 创建时间：2024-02-03 11:45:20
 *🏷️----------------------------------------------------------------*/


using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using HtKit;
using MMClipboard.UserConfigs;


namespace MMClipboard.ViewModel;

public class WindowBackgroundModel : ObservableObject, IDisposable
{
    public BitmapSource backgroundImage
    {
        get => _backgroundImage;
        private set => SetProperty(ref _backgroundImage, value);
    }
    private BitmapSource _backgroundImage = UserConfig.Default.config.bgImgSource;

    public Visibility backgroundImageVisibility
    {
        get => _backgroundImageVisibility;
        private set => SetProperty(ref _backgroundImageVisibility, value);
    }
    private Visibility _backgroundImageVisibility = UserConfig.Default.config.isUseBackgroundImg ? Visibility.Visible : Visibility.Collapsed;

    public SolidColorBrush backgroundColor
    {
        get => _solidColorBrush;
        private set => SetProperty(ref _solidColorBrush, value);
    }
    private SolidColorBrush _solidColorBrush = HtColor.GetBrushWithString(UserConfig.Default.config.pageBackgroundColor);

    public WindowBackgroundModel()
    {
        SharedInstance.Instance.backgroundColorChangeDelegate += BackgroundColorChange;
        SharedInstance.Instance.backgroundChangeDelegate += BackgroundChange;
        SharedInstance.Instance.backgroundImageChangeDelegate += BackgroundImageChange;
    }

    private void BackgroundChange(bool use)
    {
        backgroundImageVisibility = use ? Visibility.Visible : Visibility.Collapsed;
    }

    private void BackgroundColorChange(SolidColorBrush color)
    {
        backgroundColor = color;
    }

    private void BackgroundImageChange(BitmapSource img)
    {
        backgroundImage = img;
    }

    public void Dispose()
    {
        SharedInstance.Instance.backgroundColorChangeDelegate -= BackgroundColorChange;
        SharedInstance.Instance.backgroundChangeDelegate -= BackgroundChange;
        SharedInstance.Instance.backgroundImageChangeDelegate -= BackgroundImageChange;
    }
}