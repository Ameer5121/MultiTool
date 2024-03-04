using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BestAutoClicker.Resources
{
    public partial class Themes : ResourceDictionary
    {
        private void ShutDown(object sender, RoutedEventArgs e) => Application.Current.Shutdown();
        private void Minimize(object sender, RoutedEventArgs e) => Application.Current.MainWindow.WindowState = WindowState.Minimized;
    }
}
