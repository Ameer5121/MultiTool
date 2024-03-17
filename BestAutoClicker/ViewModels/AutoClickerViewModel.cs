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
using BestAutoClicker.Helper.Structs;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrayNotify;

namespace BestAutoClicker.ViewModels
{

    internal class AutoClickerViewModel : ViewModelBase
    {
        private bool _isRunning;
        private CancellationTokenSource _cancelClick;
        private int _milliSeconds;
        private int _seconds;
        private int _minutes;
        private int _hours;

        public event Action ClearUIPoints;

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

        public AutoClickerMode CurrentMode { get; set; }


        public RelayCommand ClearPointsCommand => new RelayCommand(ClearPoints);
        public RelayCommand SetModeCommand => new RelayCommand(SetMode);

        public bool IsRunning => _isRunning;
        public CancellationTokenSource ClickingProcess => _cancelClick;
        private TimeSpan _customTime;
        public ObservableCollection<Point> Points { get; } = new ObservableCollection<Point>();

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out Point getPoint);

        [DllImport("user32.dll")]
        public static extern bool SetCursorPos(int setX, int setY);

        [DllImport("User32.dll")]
        private static extern short GetKeyState(int vKey);

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint SendInput(uint nInputs, MouseInput[] pInputs, int cbSize);

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
            MouseInput[] mouseInput = new MouseInput[2];
            mouseInput[0].mouseData.dwFlags = (uint)ClickingMode.LeftClickDown;
            mouseInput[1].mouseData.dwFlags = (uint)ClickingMode.LeftClickUp;
            while (_cancelClick.IsCancellationRequested == false && CurrentMode == AutoClickerMode.AutoClicker)
            {
                SendInput(2, mouseInput, Marshal.SizeOf<MouseInput>());
                Thread.Sleep(_customTime);
            }
            _isRunning = false;
            _cancelClick = new CancellationTokenSource();
        }

        public void HoldClick()
        {
            _isRunning = true;
            MouseInput[] mouseInput = new MouseInput[1];
            mouseInput[0].mouseData.dwFlags = (uint)ClickingMode.LeftClickDown;
            while (CurrentMode == AutoClickerMode.HoldClicker && _cancelClick.IsCancellationRequested == false)
            {
                if (((ushort)GetKeyState((int)HoldingMode.LeftClick) >> 15) == 1)
                {
                    Thread.Sleep(500);
                    while (((ushort)GetKeyState((int)HoldingMode.LeftClick) >> 15) == 1)
                    {
                        SendInput(1, mouseInput, Marshal.SizeOf<MouseInput>());
                        Thread.Sleep(_customTime);
                    }
                }
            }
            _isRunning = false;
        }

        public void MultipleClick()
        {
            _isRunning = true;
            MouseInput[] mouseInput = new MouseInput[4];
            mouseInput[0] = new MouseInput();
            mouseInput[1] = new MouseInput();
            mouseInput[2].mouseData.dwFlags = (uint)ClickingMode.LeftClickDown;
            mouseInput[3].mouseData.dwFlags = (uint)ClickingMode.LeftClickUp;
            while (_cancelClick.IsCancellationRequested == false && CurrentMode == AutoClickerMode.MultiplePoints)
            {
                foreach (Point i in Points)
                {
                    var screen = Screen.PrimaryScreen.Bounds;
                    var abX = i.X * 65355 / screen.Width;
                    var abY = i.Y * 65355 / screen.Height;
                    mouseInput[0].mouseData.dx = abX;
                    mouseInput[0].mouseData.dy = abY;
                    mouseInput[1].mouseData.dx = abX + 1;
                    mouseInput[1].mouseData.dy = abY + 1;
                    SendInput(4, mouseInput, Marshal.SizeOf<MouseInput>());
                    Thread.Sleep(_customTime);
                }
            }
            _isRunning = false;
            _cancelClick = new CancellationTokenSource();
        }

        private void SetMode(AutoClickerMode autoClickerMode)
        {
            if (CurrentMode == autoClickerMode) return;
            CurrentMode = autoClickerMode;
            if (autoClickerMode == AutoClickerMode.HoldClicker) Task.Run(HoldClick);
        }

        private void ClearPoints()
        {
            Points.Clear();
            ClearUIPoints?.Invoke();
        }
    }
}
