/*🏷️----------------------------------------------------------------
 *📄 文件名：MainViewModel.cs
 *🏷️
 *👨🏽‍💻 创建者：Ht
 *⏱️ 创建时间：2023/9/22 16:17:56
 *🏷️----------------------------------------------------------------*/


using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using MMClipboard.View;


namespace MMClipboard.ViewModel;

public class MainViewModel : ObservableObject
{
    private object _currentView;

    public object currentView
    {
        get => _currentView;
        private set
        {
            _currentView = value;
            OnPropertyChanged();
        }
    }

    public MainViewModel(Window wd)
    {
        currentView = new ClipboardHistory(wd);
    }
}