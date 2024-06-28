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
using Newtonsoft.Json;
using System.IO;
using BestAutoClicker.Helper.Events;
using BestAutoClicker.Views;
using BestAutoClicker.Helper.Extensions;
using System.Diagnostics;
using System.Windows.Shapes;
using Rectangle = System.Drawing.Rectangle;

namespace BestAutoClicker.ViewModels
{

    internal class AutoClickerViewModel : ViewModelBase
    {
        private CancellationTokenSource _cancelClick;
        private IntPtr _mouseHandleHook;
        private bool _holding;
        private string _pointsDirectory;

        public event Action PointsCleared;
        public event EventHandler<LoadPointsEventArgs> PointsLoaded;

        public bool RLMPCIsChecked { get; set; }
        public bool UniversalDelay { get; set; } = true;
        public bool Editing { get; set; }

        public int MilliSeconds { get; set; } = 100;
        public int Seconds { get; set; }
        public int Minutes { get; set; }
        public int Hours { get; set; }
        public int Interval => (int)new TimeSpan(0, Hours, Minutes, Seconds, MilliSeconds).TotalMilliseconds;

        private int _currentMPCModelIndex;

        public int TimerIdentifier { get; private set; }

        public AutoClickerMode CurrentMode { get; set; }
        public ClickingMode CurrentClickingMode { get; set; }
        public MouseMessage HoldClickMessage { get; set; } = MouseMessage.LeftButtonDown;

        public KeyboardMouseInput[] MouseInput { get; set; }

        public RelayCommand ClearPointsCommand => new RelayCommand(ClearPoints);
        public RelayCommand SetModeCommand => new RelayCommand(SetMode);
        public RelayCommand SetClickingModeCommand => new RelayCommand(SetClickingMode);

        public RelayCommand SavePointsCommand => new RelayCommand(SavePoints, CanSavePoints);
        public RelayCommand LoadPointsCommand => new RelayCommand(LoadPoints);

        public bool IsRunning { get; private set; }
        public CancellationTokenSource ClickingProcess => _cancelClick;
        public ObservableCollection<MPCModel> MPCModels { get; } = new ObservableCollection<MPCModel>();

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out Point getPoint);

        [DllImport("User32.dll")]
        private static extern short GetKeyState(int vKey);

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint SendInput(int nInputs, KeyboardMouseInput[] pInputs, int cbSize);

        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);
        private LowLevelMouseProc _holdClickCallBack;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr UnhookWindowsHookEx(IntPtr handleToRemove);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        private Action _timerHandler;

        [DllImport("winmm.dll")]
        private static extern int timeSetEvent(int delay, int resolution,
                                         Action handler, IntPtr user, int eventType);
        [DllImport("winmm.dll")]
        private static extern int timeKillEvent(int uTimerID);

        private Rectangle _screenBounds;

        public AutoClickerViewModel()
        {
            _holdClickCallBack = HoldClick;
            _cancelClick = new CancellationTokenSource();
            CurrentMode = AutoClickerMode.AutoClicker;
            CurrentClickingMode = ClickingMode.LeftClickDown;
            _pointsDirectory = $@"{Directory.GetCurrentDirectory()}\Data\Points";
            _pointsDirectory.TryCreateInitialDirectory();

            _screenBounds = Screen.PrimaryScreen.Bounds;

        }

        private void Click() => SendInput(MouseInput.Length, MouseInput, Marshal.SizeOf<KeyboardMouseInput>());
        public void MultiplePointClick()
        {

            if (_currentMPCModelIndex == MPCModels.Count) _currentMPCModelIndex = 0;
            MPCModel mpcModel = MPCModels.ElementAt(_currentMPCModelIndex++);
            var clickingMode = RLMPCIsChecked == true ? mpcModel.ClickingMode : CurrentClickingMode;
            var interval = UniversalDelay == true ? Interval : (int)new TimeSpan(0, mpcModel.Hours, mpcModel.Minutes, mpcModel.Seconds, mpcModel.Milliseconds).TotalMilliseconds;
            var abX = mpcModel.Point.X * 65355 / _screenBounds.Width;
            var abY = mpcModel.Point.Y * 65355 / _screenBounds.Height;
            for (int x = 0; x < MouseInput.Length - 2; x++)
            {
                MouseInput[x].inputUnion.mouseData.dx = abX + x;
                MouseInput[x].inputUnion.mouseData.dy = abY + x;
            }
            MouseInput[MouseInput.Length - 2].inputUnion.mouseData.dwFlags = (uint)clickingMode;
            MouseInput[MouseInput.Length - 1].inputUnion.mouseData.dwFlags = GetUpFlag(clickingMode);
            Click();
            if (!UniversalDelay && IsRunning)
            {
                timeKillEvent(TimerIdentifier);
                StartTimer(interval);
            }
        }

        public int GetMPCInterval() => UniversalDelay == true ? Interval : (int)new TimeSpan(0, MPCModels[0].Hours, MPCModels[0].Minutes, MPCModels[0].Seconds, MPCModels[0].Milliseconds).TotalMilliseconds;
        public void StartTimer(int totalMilliSeconds)
        {
            if (!IsRunning) IsRunning = true;
            TimerIdentifier = timeSetEvent(totalMilliSeconds == 0 ? 1 : totalMilliSeconds, 0, _timerHandler, IntPtr.Zero, 1);
        }
        public void StopTimer()
        {
            timeKillEvent(TimerIdentifier);
            if (CurrentMode == AutoClickerMode.MultiplePoints) _currentMPCModelIndex = 0;
            TimerIdentifier = 0;
            IsRunning = false;
        }
        public void SetNormalClickInput()
        {
            _timerHandler = Click;
            MouseInput = new KeyboardMouseInput[2];
            MouseInput[0].inputUnion.mouseData.dwFlags = (uint)CurrentClickingMode;
            MouseInput[1].inputUnion.mouseData.dwFlags = GetUpFlag(CurrentClickingMode);
        }

        private IntPtr HoldClick(int nCode, IntPtr wParam, IntPtr lParam)
        {
            MouseInputData hookStruct = Marshal.PtrToStructure<MouseInputData>(lParam);
            if ((MouseMessage)wParam == HoldClickMessage + 1 && (int)hookStruct.dwExtraInfo != 5)
            {
                _holding = false;
                if (TimerIdentifier != 0) StopTimer();
            }
            else if (!IsRunning && (MouseMessage)wParam == HoldClickMessage && (int)hookStruct.dwExtraInfo != 5 && !Controls.HotkeyRecording)
            {
                Task.Run(() =>
                {
                    _holding = true;
                    IsRunning = true;
                    _timerHandler = Click;
                    var keyState = CurrentClickingMode == ClickingMode.LeftClickDown ? (int)CurrentClickingMode / 2 : (int)CurrentClickingMode / 4;
                    MouseInput = new KeyboardMouseInput[2];
                    MouseInput[0].inputUnion.mouseData.dwFlags = (uint)CurrentClickingMode;
                    MouseInput[1].inputUnion.mouseData.dwFlags = GetUpFlag(CurrentClickingMode);
                    MouseInput[0].inputUnion.mouseData.dwExtraInfo = (IntPtr)5;
                    MouseInput[1].inputUnion.mouseData.dwExtraInfo = (IntPtr)5;
                    Thread.Sleep(500);
                    if ((ushort)GetKeyState(keyState) >> 15 == 1) _holding = true; // In case of a double click
                    if (_holding) StartTimer(Interval);
                    else IsRunning = false;
                });
            }
            return CallNextHookEx(_mouseHandleHook, nCode, wParam, lParam);
        }

        public void SetMultipleClickInput()
        {
            _timerHandler = MultiplePointClick;
            MouseInput = new KeyboardMouseInput[300];
            for (int x = 0; x < MouseInput.Length - 2; x++)
                MouseInput[x] = new KeyboardMouseInput();
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

        public void ReplacePoints(int index1, int index2)
        {
            var tempModel = MPCModels[index1];
            MPCModels[index1] = MPCModels[index2];
            MPCModels[index2] = tempModel;
        }
        private void ClearPoints()
        {
            MPCModels.Clear();
            PointsCleared?.Invoke();
        }

        private void SavePoints()
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Points Data (dt File) | *.dt";
                saveFileDialog.InitialDirectory = _pointsDirectory;
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    var MPCList = JsonConvert.SerializeObject(MPCModels, Formatting.Indented);
                    using (StreamWriter sw = new StreamWriter(saveFileDialog.OpenFile())) sw.Write(MPCList);
                }
            }
        }

        private bool CanSavePoints() => MPCModels.Count > 0;
        private void LoadPoints()
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Points Data (dt File) | *.dt";
                openFileDialog.InitialDirectory = _pointsDirectory;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    if (MPCModels.Count > 0) ClearPoints();
                    using (StreamReader streamReader = new StreamReader(openFileDialog.OpenFile()))
                    {
                        var jsonData = streamReader.ReadToEnd();
                        var mpcModelsToReturn = JsonConvert.DeserializeObject<IEnumerable<MPCModel>>(jsonData);
                        foreach (var mpcModel in mpcModelsToReturn) MPCModels.Add(mpcModel);
                    }
                }
            }
            PointsLoaded?.Invoke(this, new LoadPointsEventArgs(MPCModels));
        }
    }
}
