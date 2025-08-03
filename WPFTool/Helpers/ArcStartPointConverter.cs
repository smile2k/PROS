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
    public class ArcStartPointConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 3 ||
                !(values[0] is double width) ||
                !(values[1] is double height) ||
                !(values[2] is double strokeThickness))
                return new Point(0, 0);

            double centerX = width / 2;
            double centerY = height / 2;

            // StartPoint của cung (ví dụ: 12 giờ)
            double startX = centerX;
            double startY = centerY - (centerY - strokeThickness / 2);

            return new Point(startX, startY);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }

}
