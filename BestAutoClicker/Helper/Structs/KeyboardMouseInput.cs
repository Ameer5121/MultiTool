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
        public InputUnion inputUnion;

        public KeyboardMouseInput(int type = 0)
        {
            this.type = type;
            inputUnion = new InputUnion();
            inputUnion.mouseData.dwFlags = 0x8001;
        }
    }
}
