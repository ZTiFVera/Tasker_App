using System;
using System.Globalization;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace Tasker_App.Converters
{
    public class StringToColorConverter : IValueConverter
    {
        // Common name map for quick fallback
        private static readonly System.Collections.Generic.Dictionary<string, Color> NamedColors = new(StringComparer.OrdinalIgnoreCase)
        {
            { "red", Colors.Red },
            { "green", Colors.Green },
            { "blue", Colors.Blue },
            { "orange", Colors.Orange },
            { "yellow", Colors.Yellow },
            { "purple", Colors.Purple },
            { "gray", Colors.Gray },
            { "grey", Colors.Gray },
            { "white", Colors.White },
            { "black", Colors.Black }
        };

        private static readonly Color Fallback = Color.FromArgb("#4A7BA7"); // visible accent

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value is string s)
                {
                    s = s.Trim();
                    if (string.IsNullOrWhiteSpace(s))
                        return Fallback;

                    // If user passed a hex without '#', try to fix it
                    if (!s.StartsWith("#") && (s.Length == 6 || s.Length == 8))
                        s = "#" + s;

                    // Try parse hex (e.g. #RRGGBB or #AARRGGBB)
                    try
                    {
                        return Color.FromArgb(s);
                    }
                    catch
                    {
                        // Try known named colors
                        if (NamedColors.TryGetValue(s, out var named))
                            return named;

                        // Try parsing as hex without '#', or other formats by adding '#'
                        if (s.Length == 6 || s.Length == 8)
                        {
                            try { return Color.FromArgb("#" + s); } catch { }
                        }
                    }
                }
            }
            catch
            {
                // ignore and fallback
            }

            return Fallback;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }
}