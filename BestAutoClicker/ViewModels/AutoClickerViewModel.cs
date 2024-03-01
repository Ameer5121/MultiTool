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

        public RelayCommand testing => new RelayCommand(ClickingFunction);
        public RelayCommand setcursorpos => new RelayCommand(SetCursor);

        public AutoClickerViewModel()
        {
            Task.Run(ListenForKeys);

        }
        private async Task SetCursor()
        {
            SetCursorPos(_cursorPosition.X, _cursorPosition.Y);
        }
        private async Task ClickingFunction()
        {
            GetCursorPos(out _cursorPosition);
            MessageBox.Show(_cursorPosition.ToString());
        }
        private async Task Click()
        {
            if (true)
            {
                
            }
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
                        if (keyPressed == Keys.F1) MessageBox.Show(keyPressed.ToString());
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
