using BestAutoClicker.Commands;
using BestAutoClicker.Helper.Enums;
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
        private AutoClickerViewModel _autoClickerViewModel;

        public KeyClicker()
        {
            InitializeComponent();
            _autoClickerViewModel = (Application.Current.MainWindow.DataContext as AutoClickerViewModel);
            DataContext = _autoClickerViewModel;
        }

        private void RecordKey(object sender, RoutedEventArgs e)
        {
            if (!_isRecording)
            {
                ComponentDispatcher.ThreadPreprocessMessage += RecordHotkey;
                RecordKeyButton.Content = "...";
                _isRecording = true;
                if (_autoClickerViewModel.CurrentKMode == KeyClickerMode.Multi) _autoClickerViewModel.MultiKeys.Clear();
            }
            else 
            {
                ComponentDispatcher.ThreadPreprocessMessage -= RecordHotkey;
                RecordKeyButton.Content = "Record Key(s)";
                _isRecording = false;
            }
        }

        private void RecordHotkey(ref MSG msg, ref bool handled)
        {
            if (msg.message == WM_KEYDOWN)
            {
                Keys key = (Keys)msg.wParam;
                if (_autoClickerViewModel.CurrentKMode == KeyClickerMode.Single)
                {
                    _autoClickerViewModel.KeyToPress = key;
                    ComponentDispatcher.ThreadPreprocessMessage -= RecordHotkey;
                    _isRecording = false;
                    RecordKeyButton.Content = "Record Key(s)";
                }
                else _autoClickerViewModel.MultiKeys.Add(key);
            }
        }
    }
}
