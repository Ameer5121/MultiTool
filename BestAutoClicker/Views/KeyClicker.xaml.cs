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
using MessageBox = System.Windows.MessageBox;

namespace BestAutoClicker.Views
{
    /// <summary>
    /// Interaction logic for KeyClicker.xaml
    /// </summary>
    public partial class KeyClicker : Page
    {
        private int WM_KEYDOWN = 0x0100;
        private bool _isRecording;
        private bool _multiRecording = false;
        private List<Keys> _keys;

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
                _isRecording = true;
            }
        }

        private void RecordHotkey(ref MSG msg, ref bool handled)
        {
            if (msg.message == WM_KEYDOWN)
            {
                Keys key = (Keys)msg.wParam;
                if (!_multiRecording)
                {
                    (Application.Current.MainWindow.DataContext as AutoClickerViewModel).KeyToPress = key;
                    RecordKeyButton.Foreground = Brushes.White;
                    RecordKeyButton.Content = $"Record Key ({key})";
                    ComponentDispatcher.ThreadPreprocessMessage -= RecordHotkey;
                    _isRecording = false;
                }
                else _keys.Add(key);
            }
        }

        private void RecordMultiKey(object sender, RoutedEventArgs e)
        {
            if (!_isRecording) 
            {
                _multiRecording = true;
                _isRecording = true;
                _keys = new List<Keys>();
                RecordMultipleKeys.Content = "Press keys...";
                ComponentDispatcher.ThreadPreprocessMessage += RecordHotkey;
            }
            else
            {
                ComponentDispatcher.ThreadPreprocessMessage -= RecordHotkey;
                RecordMultipleKeys.Content = "Record Multiple Keys";
                _isRecording = false;
                _multiRecording = false;
                (Application.Current.MainWindow.DataContext as AutoClickerViewModel).MultiKeys = _keys;
            }
        }
    }
}
