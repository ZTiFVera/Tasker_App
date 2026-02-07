using System;
using System.Globalization;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace Tasker_App.Converters
{
    public class FilterButtonColorConverter : IValueConverter
    {
        private static readonly Color Active = Color.FromArgb("#2F6A8F");
        private static readonly Color Inactive = Color.FromArgb("#35607A");

        // value is current FilterStatus, parameter is expected filter string (e.g., "all")
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var current = value as string ?? string.Empty;
            var expected = parameter as string ?? string.Empty;

            return string.Equals(current, expected, StringComparison.OrdinalIgnoreCase) ? Active : Inactive;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }
}