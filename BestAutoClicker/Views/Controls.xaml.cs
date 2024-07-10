using BestAutoClicker.Commands;
using BestAutoClicker.Helper.Enums;
using BestAutoClicker.Helper.Extensions;
using BestAutoClicker.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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
        private int WM_KEYDOWN = 0x0100;

        private Button _button;

        public static bool HotkeyRecording;

        private string _controlsDirectory;

        private string _bindingsFile;

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
            Bindings = new Dictionary<HotKeys, Keys>() { { HotKeys.Click, Keys.F1 }, { HotKeys.MPCMenu, Keys.F5 }, { HotKeys.Macro, Keys.F2 } };
            _controlsDirectory = $@"{Directory.GetCurrentDirectory()}\Data\Controls";
            _controlsDirectory.TryCreateInitialDirectory();
            _bindingsFile = $@"{_controlsDirectory}\Bindings.dt";
            InitializeControlsFile();
        }
        private void RecordHotkey(ref MSG msg, ref bool handled)
        {
            if (msg.message == WM_KEYDOWN)
            {
                Keys key = (Keys)msg.wParam;
                RegisterBinding(key);
                Bindings[_keyToChange] = key;
                _button.Tag = key;
                _button.Foreground = Brushes.White;
                _button.Content = $"Record ({key})";
                ComponentDispatcher.ThreadPreprocessMessage -= RecordHotkey;
                HotkeyRecording = false;
                SaveControlsFile();
            }
        }

        private void RegisterBinding(Keys key) => RegisterHotKey(_windowHandle, (int) key, 0, (int) key);

        private void Subscribe(object sender, RoutedEventArgs e)
        {
            if (!HotkeyRecording)
            {
                HotkeyRecording = true;
                ComponentDispatcher.ThreadPreprocessMessage += RecordHotkey;
                Keys tag = (Keys)(sender as Button).Tag;
                _button = sender as Button;
                UnregisterHotKey(_windowHandle, (int)tag);
                _keyToChange = Bindings.First(x => x.Value == tag).Key;
                _button.Content = "Select a keybind";
                _button.Foreground = Brushes.LightGreen;
            }
        }

        private void InitializeControlsFile()
        {
            var Directory = $@"{_controlsDirectory}\Bindings.dt";
            if (!File.Exists(Directory))
            {
                File.Create(Directory).Dispose();
                SaveControlsFile();
                foreach (var value in Bindings.Values) RegisterBinding(value);
            }
            else
            {
                using (StreamReader sr = new StreamReader(Directory))
                {
                    string jsonData = sr.ReadToEnd();
                    var loadedBindigs = JsonConvert.DeserializeObject<Dictionary<HotKeys, Keys>>(jsonData);
                    Bindings = loadedBindigs;
                    int counter = 0;
                    foreach (var value in loadedBindigs.Values)
                    {
                        RegisterBinding(value);
                        Button button = ButtonColumn.Children[counter] as Button;
                        button.Content = $"Record ({value})";
                        counter++;
                    }
                }
            }
        }

        private void SaveControlsFile()
        {
            using (StreamWriter sw = new StreamWriter(_bindingsFile, false))
            {
                var bindings = JsonConvert.SerializeObject(Bindings, Formatting.Indented);
                sw.WriteLine(bindings);
                
            }
        } 
    }
}
