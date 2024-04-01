using BestAutoClicker.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BestAutoClicker.Helper
{
    internal class TabsManager
    {
        public static Controls ControlsTab { get; private set; }

        public static void InitializeTabs()
        {
            ControlsTab = new Controls();
        }
    }
}
