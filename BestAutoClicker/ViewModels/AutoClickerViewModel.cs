﻿using BestAutoClicker.Commands;
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
using System.Windows.Media.Media3D;
using Point = System.Drawing.Point;

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

            while (true)
            {

                for (int i = 0; i <= 172; i++)
                {

                    short keyResult = GetAsyncKeyState(i);
                    if (keyResult != 0)
                    {

                        Key keyPressed = (Key)i;

                        if (keyPressed == Key.S) MessageBox.Show(keyPressed.ToString());

                    }
                    
                }

            }

        }

        
    }
}
