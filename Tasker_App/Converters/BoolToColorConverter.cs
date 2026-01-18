using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasker_App.Converters
{
   
        public class BoolToColorConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (value is bool isSelected)
                {
                    return isSelected ? Color.FromArgb("#5A8FC7") : Color.FromArgb("#4A7BA7");
                }
                return Color.FromArgb("#4A7BA7");
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }
    
}
