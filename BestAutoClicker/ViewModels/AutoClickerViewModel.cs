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

namespace BestAutoClicker.ViewModels
{
    
    internal class AutoClickerViewModel : ViewModelBase
    {

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
        private void Click()
        {
            int lButton = 0x0002;
            mouse_event(lButton, _cursorPosition.X, _cursorPosition.Y, 0, 0);
        }
        private void ListenForKeys()
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
                        if (keyPressed == Keys.S)
                        {
                            Click();
                        }
                        if (keyPressed == Keys.F2)
                        {
                            GetCursor();
                        }
                        if (keyPressed == Keys.F3)
                        {
                            SetCursor();
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
