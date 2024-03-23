using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace BestAutoClicker.Helper.Converters
{
    internal class TextBoxStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value;
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => (int.TryParse(value.ToString(), out var valueToReturn)) == true ? valueToReturn : 0;
    }
}
