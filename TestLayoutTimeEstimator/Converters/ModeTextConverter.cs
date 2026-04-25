using System;
using System.Globalization;
using System.Windows.Data;

namespace TestLayoutTimeEstimator.Converters
{
    public class ModeTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value is bool isSelection && isSelection) ? "🖱️ Режим перемещения" : "✏️ Режим выделения";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}