using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace WPFTool.Helpers
{
    public class HalfConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is double width && values[1] is double height && values[2] is double stroke)
            {
                Console.WriteLine($"[SizeConverter] Calculated Size: {width / 2}, {height / 2}");
                return new Size((width-stroke) / 2, (height-stroke) / 2); 
            }

            return new Size(0, 0);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
