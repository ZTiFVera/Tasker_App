using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace Tasker_App.Converters
{
    public class PercentageToProgressConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double percent)
            {
                var progress = percent / 100.0;
                if (progress < 0) progress = 0;
                if (progress > 1) progress = 1;
                return progress;
            }
            return 0.0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }
}