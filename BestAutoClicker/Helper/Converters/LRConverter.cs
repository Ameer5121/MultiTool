using BestAutoClicker.Helper.Enums;
using BestAutoClicker.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace BestAutoClicker.Helper.Converters
{
    public class LRConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return "L";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value as string == "L") return ClickingMode.LeftClickDown;
            else return ClickingMode.RightClickDown;
        }
    }
}
