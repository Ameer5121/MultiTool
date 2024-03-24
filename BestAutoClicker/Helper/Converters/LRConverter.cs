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
            if ((ClickingMode)value == ClickingMode.LeftClickDown) return 0;
            return 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((int)value == 0) return ClickingMode.LeftClickDown;
            return ClickingMode.RightClickDown;
        }
    }
}
