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
using MaterialDesignColors.Recommended;
using System.Threading;
using Brushes = System.Windows.Media.Brushes;
using CheckBox = System.Windows.Controls.CheckBox;
using System.Windows.Media;
using BestAutoClicker.Models;
using BestAutoClicker.Helper.Events;
using static System.Net.Mime.MediaTypeNames;
using BestAutoClicker.Helper;

namespace BestAutoClicker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public const int WM_HOTKEY = 0x0312;

        private Controls _controlsInstance;

        private AutoClickerViewModel _autoClickerViewModel;

        [DllImport("user32.dll")]
        private static extern bool SetCapture(IntPtr hWnd);

        private MPBG _background;

        private List<Circle> _circles;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new AutoClickerViewModel();

            _autoClickerViewModel = DataContext as AutoClickerViewModel;

            ComponentDispatcher.ThreadPreprocessMessage += HandleMessages;

            _autoClickerViewModel.PointsCleared += ClearAllUICircles;
            _autoClickerViewModel.PointsLoaded += LoadCircles;
            _circles = new List<Circle>();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            TabsManager.InitializeTabs();
            base.OnSourceInitialized(e);
        }

        private void HandleMessages(ref MSG msg, ref bool handled)
        {
            if (msg.message == WM_HOTKEY)
            {
                if ((int)msg.wParam == (int)Controls.Bindings[HotKeys.Click])
                {
                    if (_autoClickerViewModel.IsRunning) _autoClickerViewModel.StopTimer();
                    else if (_autoClickerViewModel.CurrentMode == AutoClickerMode.AutoClicker)
                    {
                        _autoClickerViewModel.SetNormalClickInput();
                        _autoClickerViewModel.StartTimer(_autoClickerViewModel.Interval);
                    }
                    else if (_autoClickerViewModel.CurrentMode == AutoClickerMode.MultiplePoints && _autoClickerViewModel.MPCModels.Count != 0)
                    {
                        _autoClickerViewModel.SetMultipleClickInput();
                        _autoClickerViewModel.StartTimer(_autoClickerViewModel.GetMPCInterval());
                    }
                }
                else if ((int)msg.wParam == (int)Controls.Bindings[HotKeys.MPCMenu] && _autoClickerViewModel.CurrentMode == AutoClickerMode.MultiplePoints && !_autoClickerViewModel.Editing)
                {
                    if (_background is null || _background.IsActive == false) OpenMPBackground();
                    else CloseMPBackground();
                }
            }
        }

        private void OpenMPBackground()
        {
            this.Hide();
            Activate(); // So that background get shown on the screen
            _background = new MPBG();
            _background.WindowState = WindowState.Maximized;
            _background.MouseLeftButtonDown += AddPoints;
            if (_circles.Count > 0) foreach (var circle in _circles) _background.MPBackground.Children.Add(circle);
            _background.Show();
        }

        private void CloseMPBackground()
        {
            _background.MouseLeftButtonDown -= AddPoints;
            _background.Close();
            foreach (var circle in _circles) _background.MPBackground.Children.Remove(circle);
            this.Show();
        }
        private void AddPoints(object sender, MouseButtonEventArgs info)
        {
            AutoClickerViewModel.GetCursorPos(out var pos);
            _autoClickerViewModel.MPCModels.Add(new MPCModel(pos));
            var backgroundPosition = info.MouseDevice.GetPosition(_background);
            Circle circle = new Circle();
            _circles.Add(circle);
            circle.MouseDown += MDownUI;
            _background.MPBackground.Children.Add(circle);
            circle.RenderTransform = new TranslateTransform(backgroundPosition.X, backgroundPosition.Y);
        }

        private void CheckForNumbers(object sender, TextCompositionEventArgs e)
        {
            if (!int.TryParse(e.Text, out _)) e.Handled = true;
        }

        private void OnLeftClickPoint(object sender, MouseButtonEventArgs e)
        {
            if (!_autoClickerViewModel.Editing)
            {
                var textBlock = sender as TextBlock;
                var MPCModel = textBlock.DataContext as MPCModel;
                int indexPoint = _autoClickerViewModel.MPCModels.IndexOf(MPCModel!);
                var Border1 = _circles[indexPoint].Border1;
                var Border2 = _circles[indexPoint].Border2;
                var Border3 = _circles[indexPoint].Border3;
                HighlightCircle(Border1, Border2, Border3, (textBlock.Parent as Grid).Parent as Border);
            }
        }

        private void OnRightClickPoint(object sender, MouseButtonEventArgs e)
        {
            var LBItem = sender as TextBlock;
            var MPCModel = LBItem.DataContext as MPCModel;
            int indexPoint = _autoClickerViewModel.MPCModels.IndexOf(MPCModel);
            _autoClickerViewModel.MPCModels.Remove(MPCModel);
            _circles.RemoveAt(indexPoint);
        }

        private void ClearAllUICircles()
        {
            if (_circles.Count == 0) return;
            _background?.MPBackground.Children.Clear();
            _circles.Clear();
        }

        private void MDownUI(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) return;
            var circle = sender as Circle;
            int IndexOfCircle = _circles.IndexOf(circle);
            var Border1 = _circles[IndexOfCircle].Border1;
            var Border2 = _circles[IndexOfCircle].Border2;
            var Border3 = _circles[IndexOfCircle].Border3;

            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                var container = PointsListBox.ItemContainerGenerator.ContainerFromIndex(IndexOfCircle) as FrameworkElement;
                var itemTemplate = PointsListBox.ItemTemplate;
                var border = itemTemplate.FindName("MainBorder", container) as Border;
                HighlightCircle(Border1, Border2, Border3, border);
            }
            else if (e.RightButton == MouseButtonState.Pressed)
            {
                _background.MPBackground.Children.Remove(circle);
                _autoClickerViewModel.MPCModels.RemoveAt(IndexOfCircle);
                _circles.RemoveAt(IndexOfCircle);
            }
        }

        private void HighlightCircle(Border b1, Border b2, Border b3, Border itemBorder)
        {
            if (b1.Background != Brushes.LightGreen)
            {
                b1.Background = Brushes.LightGreen;
                b2.Background = Brushes.LightGreen;
                b3.Background = Brushes.LightGreen;
                itemBorder.BorderBrush = Brushes.LightGreen;
                itemBorder.BorderThickness = new Thickness(1);
            }
            else
            {
                b1.Background = Brushes.Red;
                b2.Background = Brushes.Orange;
                b3.Background = Brushes.Yellow;
                itemBorder.BorderThickness = new Thickness(0);
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            _autoClickerViewModel.ClickingProcess.Cancel();
            base.OnClosed(e);
        }

        private void OnCheckBoxChecked(object sender, RoutedEventArgs e)
        {
            var senderCheckBox = sender as CheckBox;
            var stackPanel = senderCheckBox.Parent as StackPanel;
            foreach (CheckBox checkBox in stackPanel.Children)
            {
                if (checkBox != senderCheckBox && checkBox.IsChecked == true)
                {
                    checkBox.Unchecked -= OnCheckBoxUnChecked;
                    checkBox.IsChecked = false;
                    checkBox.Unchecked += OnCheckBoxUnChecked;
                }
            }
        }

        private void OnCheckBoxUnChecked(object sender, RoutedEventArgs e)
        {
            var senderCheckBox = sender as CheckBox;
            senderCheckBox.Checked -= OnCheckBoxChecked;
            senderCheckBox.IsChecked = true;
            senderCheckBox.Checked += OnCheckBoxChecked;
        }

        private void LoadCircles(object sender, LoadPointsEventArgs e)
        {
            foreach (var mpcModel in e.Models)
            {
                Circle circle = new Circle();
                circle.RenderTransform = new TranslateTransform(mpcModel.Point.X, mpcModel.Point.Y);
                circle.MouseDown += MDownUI;
                _circles.Add(circle);
            }
        }
        private void ShutDown(object sender, RoutedEventArgs e) => Environment.Exit(Environment.ExitCode);
        private void Minimize(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

        private void OpenAutoClickerTab(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = null;
        }

        private void OpenControlsTab(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = TabsManager.ControlsTab;
        }

        private void EditPoints(object sender, RoutedEventArgs e)
        {
            if (!_autoClickerViewModel.Editing)
            {
                RemoveHighlightedBorder();
                PointsBorder.BorderBrush = new SolidColorBrush(Colors.Red);
                EnablePointEditing();
                _autoClickerViewModel.Editing = true;
            }
            else
            {
                PointsBorder.BorderBrush = this.Resources["WindowBorder"] as SolidColorBrush;
                DisablePointEditing();
                _autoClickerViewModel.Editing = false;
            }
        }

        private int _firstCircleToReplace = -1;
        private int _secondCircleToReplace = -1;
        private void ChoosePointToReplace(object sender, MouseButtonEventArgs e)
        {
            var border = (sender as Border);
            var index = _autoClickerViewModel.MPCModels.IndexOf(border.Tag as MPCModel);
            if (index == _firstCircleToReplace)
            {
                _firstCircleToReplace = -1;
                border.BorderBrush = Brushes.Transparent;
                border.BorderThickness = new Thickness(0);
            }
            else if (_firstCircleToReplace == -1)
            {
                _firstCircleToReplace = index;
                border.BorderBrush = Brushes.LightGreen;
                border.BorderThickness = new Thickness(1);
            }
            else
            {
                _secondCircleToReplace = index;
                RemoveHighlightedBorder();

                ReplaceCircles(_firstCircleToReplace, _secondCircleToReplace);
                _autoClickerViewModel.ReplacePoints(_firstCircleToReplace, _secondCircleToReplace);

                _firstCircleToReplace = -1;
                _secondCircleToReplace = -1;
            }
        }

        private void ReplaceCircles(int index1, int index2)
        {
            var tempCircle = _circles[index1];
            _circles[index1] = _circles[index2];
            _circles[index2] = tempCircle;
        }

        private IEnumerable<Border> GetAllBorders()
        {
            List<Border> result = new List<Border>();
            for (int x = 0; x < PointsListBox.Items.Count; x++)
            {
                var container = PointsListBox.ItemContainerGenerator.ContainerFromIndex(x) as FrameworkElement;
                var itemTemplate = PointsListBox.ItemTemplate;
                var border = itemTemplate.FindName("MainBorder", container) as Border;
                result.Add(border);
            }
            return result;
        }

        private void RemoveHighlightedBorder()
        {
            foreach (var border in GetAllBorders().Where(x => x.BorderThickness == new Thickness(1)))
            {
                border.BorderBrush = Brushes.Transparent;
                border.BorderThickness = new Thickness(0);
            }
        }

        private void EnablePointEditing()
        {
            foreach (var border in GetAllBorders()) border.MouseDown += ChoosePointToReplace;
        }
        private void DisablePointEditing()
        {
            foreach (var border in GetAllBorders()) border.MouseDown -= ChoosePointToReplace;
        }
    }
}
