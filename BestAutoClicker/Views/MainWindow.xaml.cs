using BestAutoClicker.Helper.Enums;
using BestAutoClicker.ViewModels;
using BestAutoClicker.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrayNotify;
using MessageBox = System.Windows.MessageBox;
using TextBox = System.Windows.Controls.TextBox;

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

        [DllImport("user32.dll")]
        private static extern bool SetCapture(IntPtr hWnd);

        private MPBG _background;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new AutoClickerViewModel();

            _autoClickerViewModel = DataContext as AutoClickerViewModel;

            ComponentDispatcher.ThreadPreprocessMessage += HandleMessages;

            _background = new MPBG();
            _background.WindowState = WindowState.Maximized;
            _background.MouseDown += AddPoints;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            _windowHandle = new WindowInteropHelper(this).Handle;
            RegisterHotKeys();
        }

        private void HandleMessages(ref MSG msg, ref bool handled)
        {
            if(msg.message == WM_HOTKEY)
            {
                if ((int)msg.wParam == (int)Keys.F1)
                {
                    if (_autoClickerViewModel.IsRunning) _autoClickerViewModel.ClickingProcess.Cancel();
                    else if (_autoClickerViewModel.CurrentMode == AutoClickerMode.AutoClicker) Task.Run(_autoClickerViewModel.Click);
                    else if (_autoClickerViewModel.CurrentMode == AutoClickerMode.MultiplePoints && _autoClickerViewModel.Points.Count != 0) Task.Run(_autoClickerViewModel.MultipleClick);
                }
                else if ((int)msg.wParam == (int)Keys.F5 && _autoClickerViewModel.CurrentMode == AutoClickerMode.MultiplePoints)
                {
                    if (_background.IsActive == false) OpenMPBackground();
                    else CloseBackground();
                }
            }
        }

        private void OpenMPBackground()
        {
            this.Hide();
            Activate();
            _background.Activate();
            _background.Show();
        }

        private void CloseBackground()
        {
            _background.Hide();
            this.Show();
            this.WindowState = WindowState.Minimized;
            Activate();
        }

        private void AddPoints(object sender, MouseButtonEventArgs info)
        {
            AutoClickerViewModel.GetCursorPos(out var pos);
            _autoClickerViewModel.Points.Add(pos);
            
        }

        private void RegisterHotKeys()
        {
            RegisterHotKey(_windowHandle, (int)Keys.F1, 0, (int)Keys.F1);
            RegisterHotKey(_windowHandle, (int)Keys.F5, 0, (int)Keys.F5);
        }

        private void CheckForNumbers(object sender, TextCompositionEventArgs e)
        {
            if (!int.TryParse(e.Text, out _)) e.Handled = true;
        }

        private void OnHoldClickChecked(object sender, RoutedEventArgs e) => Task.Run(_autoClickerViewModel.HoldClick);

        private void OnLeftClickPoint(object sender, MouseButtonEventArgs e)
        {

        }

        private void OnRightClickPoint(object sender, MouseButtonEventArgs e)
        {

        }

        protected override void OnClosed(EventArgs e)
        {
            _autoClickerViewModel.ClickingProcess.Cancel();
            base.OnClosed(e);
        }
    }
}
