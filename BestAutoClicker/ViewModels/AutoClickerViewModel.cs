using BestAutoClicker.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Media3D;

namespace BestAutoClicker.ViewModels
{
    
    internal class AutoClickerViewModel : ViewModelBase
    {

        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out int x, out int y);

        public RelayCommand testing 
        {

            get
            {
                return new RelayCommand(ClickingFunction);
            }
        
        }
        private async Task ClickingFunction()
        {
            int cursorXPos = 0;
            int cursorYPos = 0;
            GetCursorPos(out cursorXPos, out cursorYPos);
            MessageBox.Show($"the x is {cursorXPos} and the Y IS {cursorYPos}");
         
        }
        
    }
}
