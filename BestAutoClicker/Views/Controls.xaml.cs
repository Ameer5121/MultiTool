using BestAutoClicker.Commands;
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
using Application = System.Windows.Application;
using Button = System.Windows.Controls.Button;
using MessageBox = System.Windows.MessageBox;

namespace BestAutoClicker.Views
{
    /// <summary>
    /// Interaction logic for Controls.xaml
    /// </summary>
    public partial class Controls : Page
    {
        int WM_KEYDOWN = 0x0100;

        public static Keys ClickingHotkey { get; private set; } = Keys.F1;
        public static Keys MPCMenu { get; private set; } = Keys.F5;

        private IntPtr _windowHandle;

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        public Controls()
        {
            InitializeComponent();
            DataContext = this;
            _windowHandle = new WindowInteropHelper(Application.Current.MainWindow).Handle;
            RegisterHotKeys(ClickingHotkey);
            RegisterHotKeys(MPCMenu);
        }

        private void RecordHotkey(ref MSG msg, ref bool handled)
        {
            if (msg.message == WM_KEYDOWN)
            {
                Keys key = (Keys)msg.wParam;
                RegisterHotKeys(key);
                ComponentDispatcher.ThreadPreprocessMessage -= RecordHotkey;
            }
        }

        private void RegisterHotKeys(Keys key) => RegisterHotKey(_windowHandle, (int) key, 0, (int) key);

        private void Subscribe(object sender, RoutedEventArgs e)
        {
            ComponentDispatcher.ThreadPreprocessMessage += RecordHotkey;
            UnregisterHotKey(_windowHandle, (int)(Keys)(sender as Button).Tag);
        }
    }
}
