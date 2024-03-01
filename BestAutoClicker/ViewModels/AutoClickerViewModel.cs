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
        private Keys _defaultClicker = Keys.F6;
        private Point _cursorPosition;

        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out Point getPoint);

        [DllImport("user32.dll")]
        private static extern bool SetCursorPos(int setX, int setY);

        [DllImport("User32.dll")]
        private static extern short GetAsyncKeyState(int vKey);

        [DllImport("User32.dll")]
        private static extern short mouse_event(int dwFlags, int xPos, int yPos, int dwData, int dwExtraInfo);

        public AutoClickerViewModel()
        {
            _cancelClick = new CancellationTokenSource();
            Task.Run(ListenForKeys);

        }
        private void SetCursor()
        {
            SetCursorPos(_cursorPosition.X, _cursorPosition.Y);
        }
        private void GetCursor()
        {
            GetCursorPos(out _cursorPosition);
        }
        private async Task Click()
        {
            _isRunning = true;
            int lButton = 0x0006;
            while (_cancelClick.IsCancellationRequested == false)
            {
                mouse_event(lButton, 0, 0, 0, 0);
                await Task.Delay(1000);
            }
            _cancelClick = new CancellationTokenSource();
        }
        private async Task ListenForKeys()
        {
            ClearPreviousInputs();
            while (true)
            {
                for (int i = 0; i <= 255; i++)
                {
                    short keyResult = GetAsyncKeyState(i);
                    Keys keyPressed = (Keys)i;
                    if (keyResult != 0)
                    {
                        if (keyPressed == _defaultClicker)
                        {
                            if (!_isRunning)
                            {
                                Click();
                            }
                            else
                            {
                                _cancelClick.Cancel();
                                _isRunning = false;
                            }
                        }
                    }
                }
            }

            void ClearPreviousInputs()
            {
                for (int i = 0; i <= 255; i++) GetAsyncKeyState(i);
            }
        }
    }
}
