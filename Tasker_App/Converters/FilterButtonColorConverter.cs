using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasker_App.Converters
{
    public class FilterButtonColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string currentFilter = value?.ToString() ?? "all";
            string buttonFilter = parameter?.ToString() ?? "";

            return currentFilter == buttonFilter ? Color.FromArgb("#3D6A8C") : Color.FromArgb("#5A8FC7");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
