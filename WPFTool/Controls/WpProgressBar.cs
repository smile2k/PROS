using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WPFTool.Controls
{
    public class WpProgressBar : ProgressBar
    {
        static WpProgressBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WpProgressBar), new FrameworkPropertyMetadata(typeof(WpProgressBar)));
        }

    }
}
