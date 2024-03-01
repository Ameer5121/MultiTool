using BestAutoClicker.Commands;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
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
        private static extern bool GetCursorPos(out Point point);

        public RelayCommand testing 
        {

            get
            {
                return new RelayCommand(ClickingFunction);
            }
        
        }
        private async Task ClickingFunction()
        {
            Point CursorPostion;
            GetCursorPos(out CursorPostion);
            MessageBox.Show(CursorPostion);
         
        }
        
    }
}
