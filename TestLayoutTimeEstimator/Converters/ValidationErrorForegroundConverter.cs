using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace TestLayoutTimeEstimator.Converters
{
    public class ValidationErrorForegroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is System.Collections.IEnumerable errors && errors.GetEnumerator().MoveNext())
                return Brushes.Red;
            return Brushes.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}