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
using BestAutoClicker.Models;
using System.Diagnostics.Metrics;
using Application = System.Windows.Application;

namespace BestAutoClicker.ViewModels
{

    internal class AutoClickerViewModel : ViewModelBase
    {
        private bool _isRunning;
        private CancellationTokenSource _cancelClick;
        private IntPtr _mouseHandleHook;
        private bool _holding;

        public event Action ClearUIPoints;

        public int MilliSeconds { get; set; } = 100;
        public int Seconds { get; set; }
        public int Minutes { get; set; }
        public int Hours { get; set; }

        public AutoClickerMode CurrentMode { get; set; }
        public ClickingMode CurrentClickingMode { get; set; }
        public MouseMessage HoldClickMessage { get; set; } = MouseMessage.LeftButtonDown;

        public bool RLMPCIsChecked { get; set; }
        public bool UniversalDelay { get; set; } = true;

        public RelayCommand ClearPointsCommand => new RelayCommand(ClearPoints);
        public RelayCommand SetModeCommand => new RelayCommand(SetMode);
        public RelayCommand SetClickingModeCommand => new RelayCommand(SetClickingMode);

        public bool IsRunning => _isRunning;
        public CancellationTokenSource ClickingProcess => _cancelClick;
        public ObservableCollection<MPCModel> MPCModels { get; } = new ObservableCollection<MPCModel>();

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out Point getPoint);

        [DllImport("user32.dll")]
        public static extern bool SetCursorPos(int setX, int setY);

        [DllImport("User32.dll")]
        private static extern short GetKeyState(int vKey);

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint SendInput(uint nInputs, MouseInput[] pInputs, int cbSize);

        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);
        private LowLevelMouseProc _holdClickCallBack;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr UnhookWindowsHookEx(IntPtr handleToRemove);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        public AutoClickerViewModel()
        {
            _holdClickCallBack = HoldClick;
            _cancelClick = new CancellationTokenSource();
            CurrentMode = AutoClickerMode.AutoClicker;
            CurrentClickingMode = ClickingMode.LeftClickDown;
    }

        public void Click()
        {
            _isRunning = true;
            MouseInput[] mouseInput = new MouseInput[2];
            mouseInput[0].mouseData.dwFlags = (uint)CurrentClickingMode;
            mouseInput[1].mouseData.dwFlags = GetUpFlag(CurrentClickingMode);
            while (_cancelClick.IsCancellationRequested == false && CurrentMode == AutoClickerMode.AutoClicker)
            {
                SendInput(2, mouseInput, Marshal.SizeOf<MouseInput>());
                Thread.Sleep(new TimeSpan(0, Hours, Minutes, Seconds, MilliSeconds));
            }
            _isRunning = false;
            _cancelClick = new CancellationTokenSource();
        }

        private IntPtr HoldClick(int nCode, IntPtr wParam, IntPtr lParam)
        {
            MouseInputData hookStruct = Marshal.PtrToStructure<MouseInputData>(lParam);
            if ((MouseMessage)wParam == HoldClickMessage + 1 && (int)hookStruct.dwExtraInfo != 5) _holding = false;
            if (!IsRunning && (MouseMessage)wParam == HoldClickMessage)
            {
                Task.Run(() =>
                {
                    _isRunning = true;
                    _holding = true;
                    var keyState = CurrentClickingMode == ClickingMode.LeftClickDown ? (int)CurrentClickingMode / 2 : (int)CurrentClickingMode / 4;
                    MouseInput[] mouseInput = new MouseInput[2];
                    mouseInput[0].mouseData.dwFlags = (uint)CurrentClickingMode;
                    mouseInput[1].mouseData.dwFlags = GetUpFlag(CurrentClickingMode);
                    mouseInput[1].mouseData.dwExtraInfo = (IntPtr)5;
                    Thread.Sleep(500);
                    if ((ushort)GetKeyState(keyState) >> 15 == 1) _holding = true; // In case of a double click
                    while (_holding)
                    {
                        SendInput(2, mouseInput, Marshal.SizeOf<MouseInput>());
                        Thread.Sleep(new TimeSpan(0, Hours, Minutes, Seconds, MilliSeconds));
                    }
                    _isRunning = false;
                });
            }
            return CallNextHookEx(_mouseHandleHook, nCode, wParam, lParam);
        }
        public void MultipleClick()
        {
            _isRunning = true;
            MouseInput[] mouseInput = new MouseInput[4];
            mouseInput[0] = new MouseInput();
            mouseInput[1] = new MouseInput();
            var screen = Screen.PrimaryScreen.Bounds;
            while (CurrentMode == AutoClickerMode.MultiplePoints)
            {
                foreach (MPCModel i in MPCModels)
                {
                    var ClickingMode = RLMPCIsChecked == true ? i.ClickingMode : CurrentClickingMode;
                    var TimeSpan = UniversalDelay == true ? new TimeSpan(0, Hours, Minutes, Seconds, MilliSeconds) : new TimeSpan(0, i.Hours, i.Minutes, i.Seconds, i.Milliseconds);
                    var abX = i.Point.X * 65355 / screen.Width;
                    var abY = i.Point.Y * 65355 / screen.Height;
                    mouseInput[0].mouseData.dx = abX;
                    mouseInput[0].mouseData.dy = abY;
                    mouseInput[1].mouseData.dx = abX + 1;
                    mouseInput[1].mouseData.dy = abY + 1;
                    mouseInput[2].mouseData.dwFlags = (uint)ClickingMode;
                    mouseInput[3].mouseData.dwFlags = GetUpFlag(ClickingMode);
                    for (int x = i.Multiplicity; x > 0; --x)
                    {
                        if (_cancelClick.IsCancellationRequested) goto End;
                        SendInput(4, mouseInput, Marshal.SizeOf<MouseInput>());
                        Thread.Sleep(TimeSpan);
                    }
                }
            }

            End:
            _isRunning = false;
            _cancelClick = new CancellationTokenSource();
        }

        private uint GetUpFlag(ClickingMode clickingMode) => clickingMode == ClickingMode.LeftClickDown ? (uint)clickingMode + 2 : (uint)clickingMode + 8;

        private void SetMode(AutoClickerMode autoClickerMode)
        {
            if (CurrentMode == autoClickerMode) return;
            CurrentMode = autoClickerMode;
            if (autoClickerMode == AutoClickerMode.HoldClicker) _mouseHandleHook = SetWindowsHookEx(14, _holdClickCallBack, IntPtr.Zero, 0);
            else UnhookWindowsHookEx(_mouseHandleHook);
        }

        private void SetClickingMode(ClickingMode clickingMode)
        {
            CurrentClickingMode = clickingMode;
            if (clickingMode == ClickingMode.LeftClickDown) HoldClickMessage = MouseMessage.LeftButtonDown;
            else HoldClickMessage = MouseMessage.RightButtonDown;
        }

        private void ClearPoints()
        {
            MPCModels.Clear();
            ClearUIPoints?.Invoke();
        }
    }
}
