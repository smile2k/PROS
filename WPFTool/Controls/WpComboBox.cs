using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WPFTool.Controls
{
    public class WpComboBox : ComboBox
    {
        static WpComboBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WpComboBox), new FrameworkPropertyMetadata(typeof(WpComboBox)));
        }


        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register(nameof(CornerRadius), typeof(CornerRadius), typeof(WpComboBox),
                new FrameworkPropertyMetadata(new CornerRadius(4), FrameworkPropertyMetadataOptions.AffectsRender));
    }
}
