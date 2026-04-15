using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace TestLayoutTimeEstimator.Converters
{
    public class BooleanToStretchConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b && b)
                return Stretch.Fill;      // растягивать на весь Canvas
            return Stretch.Uniform;       // сохранять пропорции
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}