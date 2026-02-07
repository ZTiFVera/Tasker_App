using System;
using System.Globalization;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace Tasker_App.Converters
{
    public class BoolToColorConverter : IValueConverter
    {
        // Selected: brighter teal/blue; Unselected: slightly transparent/darker than page background
        private static readonly Color SelectedColor = Color.FromArgb("#1B4965"); // visible highlight
        private static readonly Color UnselectedColor = Color.FromArgb("#0F1419"); // match page bg (looks subtle)

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isSelected)
            {
                return isSelected ? SelectedColor : UnselectedColor;
            }
            return UnselectedColor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }
}