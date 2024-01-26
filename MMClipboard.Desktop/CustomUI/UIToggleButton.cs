/*🏷️----------------------------------------------------------------
 *📄 文件名：UIToggleButton.cs
 *🏷️
 *👨🏽‍💻 创建者：Ht
 *⏱️ 创建时间：2023/12/11 15:02:13
 *🏷️----------------------------------------------------------------*/

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace HtUIKit
{
    public class UIToggleButton : CheckBox
    {
        static UIToggleButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(UIToggleButton),
                new FrameworkPropertyMetadata(typeof(UIToggleButton)));
        }

        public static readonly DependencyProperty OffTextProperty =
            DependencyProperty.Register("OffText", typeof(string), typeof(UIToggleButton),
                new PropertyMetadata("关"));
        public string OffText
        {
            get { return (string)GetValue(OffTextProperty); }
            set { SetValue(OffTextProperty, value); }
        }

        public static readonly DependencyProperty OnTextProperty =
            DependencyProperty.Register("OnText", typeof(string), typeof(UIToggleButton),
                new PropertyMetadata("开"));
        public string OnText
        {
            get { return (string)GetValue(OnTextProperty); }
            set { SetValue(OnTextProperty, value); }
        }

        public static readonly DependencyProperty OnForegroundProperty =
            DependencyProperty.Register("OnForeground", typeof(Brush), typeof(UIToggleButton),
                new PropertyMetadata(Brushes.Silver));
        public Brush OnForeground
        {
            get { return (Brush)GetValue(OnForegroundProperty); }
            set { SetValue(OnForegroundProperty, value); }
        }

        public static readonly DependencyProperty OnBackgroundProperty =
            DependencyProperty.Register("OnBackground", typeof(Brush), typeof(UIToggleButton),
                new PropertyMetadata(Brushes.Green));
        public Brush OnBackground
        {
            get { return (Brush)GetValue(OnBackgroundProperty); }
            set { SetValue(OnBackgroundProperty, value); }
        }
    }
}