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
using Point = System.Drawing.Point;
using BestAutoClicker.Views.Assets;
using MaterialDesignThemes.Wpf;
using System.Drawing;

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

            _autoClickerViewModel.ClearUIPoints += ClearAllUICircles;
            _background = new MPBG();
            _background.WindowState = WindowState.Maximized;
            _background.MouseLeftButtonDown += AddPoints;
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
                    else CloseMPBackground();
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

        private void CloseMPBackground()
        {
            _background.Hide();
            this.Show();
            Activate();
        }

        private void AddPoints(object sender, MouseButtonEventArgs info)
        {
            AutoClickerViewModel.GetCursorPos(out var pos);              
            _autoClickerViewModel.Points.Add(pos);
            var backgroundPosition = _background.PointToScreen(info.MouseDevice.GetPosition(_background));
            Circle circle = new Circle();
            circle.MouseRightButtonDown += RMouseDownUI;
            _background.MPBackground.Children.Add(circle);
            circle.RenderTransform = new TranslateTransform(backgroundPosition.X, backgroundPosition.Y);
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
            var LBItem = sender as ListBoxItem;
            Point point = (Point)LBItem.Content;
            OpenMPBackground();
            AutoClickerViewModel.SetCursorPos(point.X, point.Y);
        }

        private void OnRightClickPoint(object sender, MouseButtonEventArgs e)
        {
            var LBItem = sender as ListBoxItem;
            Point point = (Point)LBItem.Content;
            int indexPoint = _autoClickerViewModel.Points.IndexOf(point);
            _autoClickerViewModel.Points.Remove(point);
            _background.MPBackground.Children.RemoveAt(indexPoint);
        }

        private void ClearAllUICircles()
        {
            _background.MPBackground.Children.Clear();
        }

        private void RMouseDownUI(object sender, MouseButtonEventArgs e)
        {
            var circle = sender as Circle;
            int indexPoint = _background.MPBackground.Children.IndexOf(circle);
            _background.MPBackground.Children.Remove(circle);
            _autoClickerViewModel.Points.RemoveAt(indexPoint);
        }

        protected override void OnClosed(EventArgs e)
        {
            _autoClickerViewModel.ClickingProcess.Cancel();
            base.OnClosed(e);
        }
    }
}
