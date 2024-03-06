using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using BestAutoClicker.Helper.Enums;

namespace BestAutoClicker.Helper.Converter
{
    internal class IsCheckedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            AutoClickerMode currentMode = (AutoClickerMode)value;
            AutoClickerMode correspondingCheckBox = (AutoClickerMode)parameter;
            if (currentMode == correspondingCheckBox) return true;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (AutoClickerMode)parameter;
        }
    }
}
