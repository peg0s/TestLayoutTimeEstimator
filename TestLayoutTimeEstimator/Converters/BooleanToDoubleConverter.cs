using System;
using System.Globalization;
using System.Windows.Data;

namespace TestLayoutTimeEstimator.Converters
{
    public class BooleanToDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool b && b ? 1.5 : 1.0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double d)
                return Math.Abs(d - 1.5) < 0.001;
            return false;
        }
    }
}