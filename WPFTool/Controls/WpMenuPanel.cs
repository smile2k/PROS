using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace WPFTool.Controls
{
    public class WpMenuPanel : ItemsControl
    {
        static WpMenuPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WpMenuPanel), new FrameworkPropertyMetadata(typeof(WpMenuPanel)));
        }
    }
}
