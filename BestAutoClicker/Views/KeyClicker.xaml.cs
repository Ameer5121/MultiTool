using BestAutoClicker.Commands;
using BestAutoClicker.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Application = System.Windows.Application;

namespace BestAutoClicker.Views
{
    /// <summary>
    /// Interaction logic for KeyClicker.xaml
    /// </summary>
    public partial class KeyClicker : Page
    {
        private int WM_KEYDOWN = 0x0100;
        private bool _isRecording;
        public KeyClicker()
        {
            InitializeComponent();
        }
        private void RecordKey(object sender, RoutedEventArgs e)
        {
            if (!_isRecording)
            {
                RecordKeyButton.Foreground = Brushes.LightGreen;
                RecordKeyButton.Content = "Press a key...";
                ComponentDispatcher.ThreadPreprocessMessage += RecordHotkey;
            }
        }

        private void RecordHotkey(ref MSG msg, ref bool handled)
        {
            if (msg.message == WM_KEYDOWN)
            {
                Keys key = (Keys)msg.wParam;
                (Application.Current.MainWindow.DataContext as AutoClickerViewModel).KeyToPress = key;
                RecordKeyButton.Foreground = Brushes.White;
                RecordKeyButton.Content = $"Record Key ({key})";
                ComponentDispatcher.ThreadPreprocessMessage -= RecordHotkey;
            }
        }
    }
}
