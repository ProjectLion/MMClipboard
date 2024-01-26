/*🏷️----------------------------------------------------------------
 *📄 文件名：TaskbarViewModel.cs
 *🏷️
 *👨🏽‍💻 创建者：Ht
 *⏱️ 创建时间：2023/9/22 14:16:36
 *🏷️----------------------------------------------------------------*/

using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MMClipboard.ViewModel
{
    public class TaskbarViewModel : ObservableObject
    {
        public double FixWidth { get; set; }

        public ICommand taskbarMenuCommand { get; private set; } = new RelayCommand<string>(TaskbarMenuAction);
        public RelayCommand clickTaskbarCommand { get; private set; } = new(ClickTaskbar);

        private static void TaskbarMenuAction(string par)
        {
            if (par is null)
                return;
            switch (par)
            {
                case "Setting":
                    SharedInstance.ShowSettingWindow();
                    break;
                case "Quit":
                    Application.Current.Shutdown();
                    break;
            }
        }

        private static void ClickTaskbar()
        {
            SharedInstance.ShowSettingWindow();
        }
    }
}