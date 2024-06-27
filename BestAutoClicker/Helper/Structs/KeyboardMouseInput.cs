using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BestAutoClicker.Helper.Structs
{
    struct KeyboardMouseInput
    {
        public int type;
        public MouseInputData mouseData;
        public KeyboardInputData keyboardData;

        public KeyboardMouseInput(int type = 0)
        {
            this.type = type;
            mouseData = new MouseInputData();
            mouseData.dwFlags = 0x8001;
            keyboardData = new KeyboardInputData();
        }
    }
}
