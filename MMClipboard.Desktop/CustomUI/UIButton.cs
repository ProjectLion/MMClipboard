/*🏷️----------------------------------------------------------------
 *📄 文件名：UIButton.cs
 *🏷️
 *👨🏽‍💻 创建者：Ht
 *⏱️ 创建时间：2023/9/5 09:45:12
 *🏷️----------------------------------------------------------------*/

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace HtUIKit
{
    public class UIButton : Button
    {
        private Border m_backgroundBorder;
        private Border m_highLightBorder;

        #region 定义一个关联属性，在自定义的XAML中就能找到该属性
        /// <summary>
        /// 标题
        /// </summary>
        [Bindable(true)]
        [Category("自定义")]
        public string title
        {
            get => (string)GetValue(titleProperty);
            set => SetValue(titleProperty, value);
        }
        public static readonly DependencyProperty titleProperty = DependencyProperty.Register(nameof(title), typeof(string), typeof(UIButton));

        /// <summary>
        /// 标题颜色
        /// </summary>
        [Bindable(true)]
        [Category("自定义")]
        public Brush titleColor
        {
            get => (Brush)GetValue(titleColorProperty);
            set => SetValue(titleColorProperty, value);
        }
        public static readonly DependencyProperty titleColorProperty = DependencyProperty.Register(nameof(titleColor), typeof(Brush), typeof(UIButton));

        /// <summary>
        /// 按钮图片
        /// </summary>
        [Bindable(true)]
        [Category("自定义")]
        public ImageSource image
        {
            get => (ImageSource)GetValue(imageProperty);
            set => SetValue(imageProperty, value);
        }
        public static readonly DependencyProperty imageProperty = DependencyProperty.Register(nameof(image), typeof(ImageSource), typeof(UIButton));

        /// <summary>
        /// 按钮大小，设置这个值的话Width = Height，设置一个正方形按钮
        /// </summary>
        [Bindable(true)]
        [Category("自定义")]
        public double size
        {
            get => (double)GetValue(sizeProperty);
            set
            {
                Width = value;
                Height = value;
                SetValue(sizeProperty, value);
            }
        }
        public static readonly DependencyProperty sizeProperty = DependencyProperty.Register(nameof(size), typeof(double), typeof(UIButton));

        /// <summary>
        /// 高亮颜色
        /// </summary>
        [Bindable(true)]
        [Category("自定义")]
        public Brush highLightColor
        {
            get => (Brush)GetValue(highLightColorProperty);
            set => SetValue(highLightColorProperty, value);
        }
        public static readonly DependencyProperty highLightColorProperty = DependencyProperty.Register(nameof(highLightColor), typeof(Brush), typeof(UIButton));

        /// <summary>
        /// 圆角大小
        /// </summary>
        [Bindable(true)]
        [Category("自定义")]
        public CornerRadius cornerRadius
        {
            get => (CornerRadius)GetValue(cornerRadiusProperty);
            set => SetValue(cornerRadiusProperty, value);
        }
        public static readonly DependencyProperty cornerRadiusProperty = DependencyProperty.Register(nameof(cornerRadius), typeof(CornerRadius), typeof(UIButton));

        /// <summary>
        /// 图片偏移量
        /// </summary>
        [Bindable(true)]
        [Category("自定义")]
        public Thickness imageMargin
        {
            get => (Thickness)GetValue(imageMarginProperty);
            set => SetValue(imageMarginProperty, value);
        }
        public static readonly DependencyProperty imageMarginProperty = DependencyProperty.Register(nameof(imageMargin), typeof(Thickness), typeof(UIButton));

        /// <summary>
        /// 是否可点击
        /// </summary>
        [Bindable(true)]
        [Category("自定义")]
        public bool enable
        {
            get => (bool)GetValue(enableProperty);
            set => SetValue(enableProperty, value);
        }
        public static readonly DependencyProperty enableProperty = DependencyProperty.Register(nameof(enable), typeof(bool), typeof(UIButton), new PropertyMetadata(true));

        /// <summary>
        /// 边框大小
        /// </summary>
        [Bindable(true)]
        [Category("自定义")]
        public Thickness borderWidth
        {
            get => (Thickness)GetValue(borderWidthProperty);
            set => SetValue(borderWidthProperty, value);
        }
        public static readonly DependencyProperty borderWidthProperty = DependencyProperty.Register(nameof(borderWidth), typeof(Thickness), typeof(UIButton));

        /// <summary>
        /// 边框颜色
        /// </summary>
        [Bindable(true)]
        [Category("自定义")]
        public Brush borderColor
        {
            get => (Brush)GetValue(borderColorProperty);
            set => SetValue(borderColorProperty, value);
        }
        public static readonly DependencyProperty borderColorProperty = DependencyProperty.Register(nameof(borderColor), typeof(Brush), typeof(UIButton));

        /// <summary>
        /// 标题字体
        /// </summary>
        [Bindable(true)]
        [Category("自定义")]
        public FontFamily font
        {
            get => (FontFamily)GetValue(fontProperty);
            set => SetValue(fontProperty, value);
        }
        public static readonly DependencyProperty fontProperty = DependencyProperty.Register(nameof(font), typeof(FontFamily), typeof(UIButton));

        /// <summary>
        /// 字体字重
        /// </summary>
        [Category("自定义")]
        public FontWeight fontWeight
        {
            get => (FontWeight)GetValue(fontWeightProperty);
            set => SetValue(fontWeightProperty, value);
        }
        public static readonly DependencyProperty fontWeightProperty = DependencyProperty.Register(nameof(fontWeight), typeof(FontWeight), typeof(UIButton));

        /// <summary>
        /// 标题字体大小
        /// </summary>
        [Bindable(true)]
        [Category("自定义")]
        public double fontSize
        {
            get => (double)GetValue(fontSizeProperty);
            set => SetValue(fontSizeProperty, value);
        }
        public static readonly DependencyProperty fontSizeProperty = DependencyProperty.Register(nameof(fontSize), typeof(double), typeof(UIButton));

        /// <summary>
        /// ToolTip
        /// </summary>
        [Bindable(true)]
        [Category("自定义")]
        public string toolTip
        {
            get => (string)GetValue(toolTipProperty);
            set => SetValue(toolTipProperty, value);
        }
        public static readonly DependencyProperty toolTipProperty = DependencyProperty.Register(nameof(toolTip), typeof(string), typeof(UIButton), new PropertyMetadata(default(string)));

        #endregion 定义一个关联属性，在自定义的XAML中就能找到该属性

        // 调用顺序 OnApplyTemplate -> OnRender -> OnRenderSizeChanged

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            m_backgroundBorder = (Border)GetTemplateChild("BackgroundBorder");
            m_highLightBorder = (Border)GetTemplateChild("HighLightBorder");
            m_highLightBorder!.Opacity = 0;

            m_backgroundBorder!.Width = Width;
            m_backgroundBorder.Height = Height;
            m_highLightBorder.Width = Width;
            m_highLightBorder.Height = Height;
            if (string.IsNullOrEmpty(toolTip)) return;
            ToolTip t = new()
            {
                Content = toolTip,
                Style = (Style)Application.Current.Resources["UIToolTip_Normal"]
            };
            ToolTip = t;
        }

        #region override events
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            Cursor = enable ? Cursors.Hand : Cursors.Arrow;
            if (!enable) return;
            HighLightAnimation(true);
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            if (!enable) return;
            HighLightAnimation(false);
            Cursor = Cursors.Arrow;
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            e.Handled = true;
            if (!enable) return;
            if (e.ClickCount != 1) return;
            HighLightAnimation(false);
            Command?.Execute(CommandParameter ?? this);
        }

        #endregion override events

        #region private mothed
        private void HighLightAnimation(bool isEnter)
        {
            var animation = new DoubleAnimation()
            {
                From = m_highLightBorder.Opacity,
                To = isEnter ? 1 : 0,
                Duration = new Duration(TimeSpan.FromSeconds(0.15f)),
            };
            m_highLightBorder.BeginAnimation(OpacityProperty, animation);
        }

        #endregion private mothed
    }

    public struct CustomRadius
    {
        public double RadiusX { get; set; }
        public double RadiusY { get; set; }
    }
}