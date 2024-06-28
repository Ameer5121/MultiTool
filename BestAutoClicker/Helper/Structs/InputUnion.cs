using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BestAutoClicker.Helper.Structs
{
    [StructLayout(LayoutKind.Explicit)]
    internal struct InputUnion
    {
        [FieldOffset(0)]  public MouseInputData mouseData;
        [FieldOffset(0)]  public KeyboardInputData keyboardData;
    }
}
