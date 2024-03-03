using BestAutoClicker.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
using MessageBox = System.Windows.MessageBox;

namespace BestAutoClicker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public const int WM_HOTKEY = 0x0312;

        private AutoClickerViewModel _autoClickerViewModel;
        private IntPtr _windowHandle;

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vlc);


        public MainWindow()
        {
            InitializeComponent();
            DataContext = new AutoClickerViewModel();

            _autoClickerViewModel = DataContext as AutoClickerViewModel;

            ComponentDispatcher.ThreadPreprocessMessage += HandleMessages;
            RegisterHotKeys();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            _windowHandle = new WindowInteropHelper(this).Handle;
        }

        private void HandleMessages(ref MSG msg, ref bool handled)
        {
            if(msg.message == WM_HOTKEY)
            {
                if ((int)msg.wParam == (int)Keys.F1)
                {
                    if (_autoClickerViewModel.IsRunning == false)
                    {
                        Task.Run(_autoClickerViewModel.Click);
                    }
                    else
                    {
                        _autoClickerViewModel.ClickingProcess.Cancel();
                    }
                }
                else if ((int)msg.wParam == (int)Keys.F2)
                {
                    MessageBox.Show("test2");
                }
            }
        }

        private void RegisterHotKeys()
        {
            RegisterHotKey(_windowHandle, (int)Keys.F1, 0, (int)Keys.F1);
            RegisterHotKey(_windowHandle, (int)Keys.F2, 0, (int)Keys.F2);
        }

    }
}
