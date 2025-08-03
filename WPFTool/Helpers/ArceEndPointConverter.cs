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
    public class ArcEndPointConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 4 ||
                !(values[0] is double width) ||
                !(values[1] is double height) ||
                !(values[2] is double strokeThickness) ||
                !(values[3] is double angleDeg))
                return new Point(0, 0);

            double centerX = width / 2;
            double centerY = height / 2;
            double radiusX = centerX - strokeThickness / 2;
            double radiusY = centerY - strokeThickness / 2;

            double angleRad = angleDeg * Math.PI / 180.0;

            double x = centerX + radiusX * Math.Sin(angleRad);
            double y = centerY - radiusY * Math.Cos(angleRad); // trục Y ngược

            return new Point(x, y);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }
}
