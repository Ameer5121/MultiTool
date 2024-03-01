using BestAutoClicker.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BestAutoClicker.ViewModels
{
    
    internal class AutoClickerViewModel : ViewModelBase
    {

        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out Point x);

        public RelayCommand testing 
        {

            get
            {
                return new RelayCommand(ClickingFunction);
            }
        
        }
        private async Task ClickingFunction()
        {

            Point CurrentCursor;

            GetCursorPos(out CurrentCursor);

            MessageBox.Show(CurrentCursor.X.ToString() + ", " + CurrentCursor.Y.ToString());

        }
        
    }
}
