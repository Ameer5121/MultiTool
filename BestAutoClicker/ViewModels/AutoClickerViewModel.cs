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

        public bool IsRunning => _isRunning;
        public CancellationTokenSource ClickingProcess => _cancelClick;
        private TimeSpan _customTime;

        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out Point getPoint);

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
            UpdateTime();
        }
        private void SetCursor()
        {
            SetCursorPos(_cursorPosition.X, _cursorPosition.Y);
        }
        private void GetCursor()
        {
            GetCursorPos(out _cursorPosition);
        }

        private void UpdateTime() => _customTime = new TimeSpan(0, _hours, _minutes, _seconds, _milliSeconds);
        public void Click()
        {
            _isRunning = true;
            while (_cancelClick.IsCancellationRequested == false)
            {
                mouse_event(lButton, 0, 0, 0, 0);
                Thread.Sleep(_customTime);
            }
            _isRunning = false;
            _cancelClick = new CancellationTokenSource();
        }        
    }
}
