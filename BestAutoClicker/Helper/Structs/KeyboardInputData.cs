using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BestAutoClicker.Helper.Structs
{
    internal struct KeyboardInputData
    {
        public int wVk;
        public int wScan;
        public uint dwFlags;
        public uint time;
        public IntPtr dwExtraInfo;
    }
}
