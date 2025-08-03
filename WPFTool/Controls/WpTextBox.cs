using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WPFTool.Controls
{
    public class WpTextBox : TextBox
    {
        static WpTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WpTextBox), new FrameworkPropertyMetadata(typeof(WpTextBox)));
        }
    }
}
