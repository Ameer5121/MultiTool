using BestAutoClicker.Commands;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Forms;
using System.Windows.Media.Media3D;
using Point = System.Drawing.Point;
using MessageBox = System.Windows.MessageBox;
using System.DirectoryServices.ActiveDirectory;
using System.Threading;
using BestAutoClicker.Helper.Enums;
using System.Collections.ObjectModel;

namespace BestAutoClicker.ViewModels
{

    internal class AutoClickerViewModel : ViewModelBase
    {
        private bool _isRunning;
        private CancellationTokenSource _cancelClick;
        private Keys _defaultClicker = Keys.F1;
        private Point _cursorPosition;
        private const int lButton = 0x0006;
        private int _milliSeconds;
        private int _seconds;
        private int _minutes;
        private int _hours;

        private AutoClickerMode _currentMode;

        public int MilliSeconds
        {
            get => _milliSeconds;
            set
            {
                SetPropertyValue(ref _milliSeconds, value);
                UpdateTime();
            }
        }
        public int Seconds
        {
            get => _seconds;
            set
            {
                SetPropertyValue(ref _seconds, value);
                UpdateTime();
            }
        }
        public int Minutes
        {
            get => _minutes;
            set
            {
                SetPropertyValue(ref _minutes, value);
                UpdateTime();
            }
        }
        public int Hours
        {
            get => _hours;
            set
            {
                SetPropertyValue(ref _hours, value);
                UpdateTime();
            }
        }

        public AutoClickerMode CurrentMode
        {
            get => _currentMode;
            set => SetPropertyValue(ref _currentMode, value);
        }

        public RelayCommand ClearPointsCommand => new RelayCommand(ClearPoints);

        public bool IsRunning => _isRunning;
        public CancellationTokenSource ClickingProcess => _cancelClick;
        private TimeSpan _customTime;
        public ObservableCollection<Point> Points { get; } = new ObservableCollection<Point>();

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out Point getPoint);

        [DllImport("user32.dll")]
        private static extern bool SetCursorPos(int setX, int setY);

        [DllImport("User32.dll")]
        private static extern short GetKeyState(int vKey);

        [DllImport("User32.dll")]
        private static extern short mouse_event(int dwFlags, int xPos, int yPos, int dwData, int dwExtraInfo);

        public AutoClickerViewModel()
        {
            _cancelClick = new CancellationTokenSource();
            _milliSeconds = 100;
            CurrentMode = AutoClickerMode.AutoClicker;
            UpdateTime();
        }

        private void UpdateTime() => _customTime = new TimeSpan(0, _hours, _minutes, _seconds, _milliSeconds);
        public void Click()
        {
            _isRunning = true;
            while (_cancelClick.IsCancellationRequested == false && _currentMode == AutoClickerMode.AutoClicker)
            {
                mouse_event(lButton, 0, 0, 0, 0);
                Thread.Sleep(_customTime);
            }
            _isRunning = false;
            _cancelClick = new CancellationTokenSource();
        }   
        
        public void HoldClick()
        {
            _isRunning = true;
            while (_currentMode == AutoClickerMode.HoldClicker)
            {
                if (((ushort)GetKeyState(0x01) >> 15) == 1)
                {
                    Thread.Sleep(500);
                    while (((ushort)GetKeyState(0x01) >> 15) == 1)
                    {
                        mouse_event(0x002, 0, 0, 0, 0);
                        Thread.Sleep(_customTime);
                    }
                }
            }
            _isRunning = false;
        }

        public void MultipleClick()
        {
            _isRunning = true;
            while (_cancelClick.IsCancellationRequested == false && _currentMode == AutoClickerMode.MultiplePoints)
            {
                foreach (Point i in Points)
                {
                    var screen = Screen.PrimaryScreen.Bounds;
                    var abX = i.X * 65355 / screen.Width;
                    var abY = i.Y * 65355 / screen.Height;
                    mouse_event(0x8001, abX, abY, 0, 0);
                    mouse_event(0x8001, abX + 1, abY + 1, 0, 0);
                    mouse_event(lButton, 0, 0, 0, 0);
                    Thread.Sleep(_customTime);
                }
            }
            _isRunning = false;
            _cancelClick = new CancellationTokenSource();
        }

        private void ClearPoints()
        {
            Points.Clear();
        }
    }
}
