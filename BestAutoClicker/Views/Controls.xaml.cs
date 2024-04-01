using BestAutoClicker.Commands;
using BestAutoClicker.Helper.Enums;
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

        public static Dictionary<HotKeys, Keys> Bindings { get; private set; }

        private IntPtr _windowHandle;

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private static HotKeys _keyToChange;
        public Controls()
        {
            InitializeComponent();
            DataContext = this;
            _windowHandle = new WindowInteropHelper(Application.Current.MainWindow).Handle;
            Bindings = new Dictionary<HotKeys, Keys>() { { HotKeys.Click, Keys.F1 }, { HotKeys.MPCMenu, Keys.F5 } };
            foreach (var key in Bindings.Values) RegisterBinding(key);
        }

        private void RecordHotkey(ref MSG msg, ref bool handled)
        {
            if (msg.message == WM_KEYDOWN)
            {
                Keys key = (Keys)msg.wParam;
                RegisterBinding(key);
                Bindings[_keyToChange] = key;
                ComponentDispatcher.ThreadPreprocessMessage -= RecordHotkey;
            }
        }

        private void RegisterBinding(Keys key) => RegisterHotKey(_windowHandle, (int) key, 0, (int) key);

        private void Subscribe(object sender, RoutedEventArgs e)
        {
            ComponentDispatcher.ThreadPreprocessMessage += RecordHotkey;
            Keys tag = (Keys)(sender as Button).Tag;
            UnregisterHotKey(_windowHandle, (int)tag);
            _keyToChange = Bindings.First(x => x.Value == tag).Key;
        }
    }
}
