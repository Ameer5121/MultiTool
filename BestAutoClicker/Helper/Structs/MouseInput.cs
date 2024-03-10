using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BestAutoClicker.Helper.Structs
{
    struct MouseInput
    {
        public int type;
        public MouseInputData mouseData;

        public MouseInput()
        {
            type = 0;
            mouseData = new MouseInputData();
            mouseData.dwFlags = 0x8001;
        }
    }
}
