using System;
using System.Globalization;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace Tasker_App.Converters
{
    public class TaskCompletionColorConverter : IValueConverter
    {
        // Completed: darker muted color; Pending: accent
        private static readonly Color CompletedBg = Color.FromArgb("#2F3942"); // muted dark gray-blue
        private static readonly Color PendingBg = Color.FromArgb("#163143");   // visible but dark

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isCompleted)
            {
                return isCompleted ? CompletedBg : PendingBg;
            }
            return PendingBg;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }
}