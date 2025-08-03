using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WPFTool.Controls
{
    public class WpButton : Button
    {
        static WpButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WpButton), new FrameworkPropertyMetadata(typeof(WpButton)));
        }

    }
}
