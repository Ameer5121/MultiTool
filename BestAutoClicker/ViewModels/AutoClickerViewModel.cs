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
using System.Windows.Media.Media3D;
using Point = System.Drawing.Point;

namespace BestAutoClicker.ViewModels
{
    
    internal class AutoClickerViewModel : ViewModelBase
    {

        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out Point getPoint);

        [DllImport("user32.dll")]
        private static extern bool SetCursorPos(int setX, int setY);

        Point CursorPostion;

        public RelayCommand testing => new RelayCommand(ClickingFunction);

        public RelayCommand setcursorpos => new RelayCommand(setCursor);

        private async Task setCursor()
        {

            SetCursorPos(CursorPostion.X, CursorPostion.Y);

        }

        private async Task ClickingFunction()
        {

            GetCursorPos(out CursorPostion);
            MessageBox.Show(CursorPostion.ToString());
         
        }
        
    }
}
