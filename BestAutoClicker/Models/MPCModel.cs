using BestAutoClicker.Helper.Enums;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BestAutoClicker.Models
{
    public class MPCModel
    {
        public int Milliseconds { get; set; }
        public int Seconds { get; set; }
        public int Minutes { get; set; }
        public int Hours { get; set; }

        public Point Point { get; }
        public ClickingMode ClickingMode { get; set; } = ClickingMode.LeftClickDown;

        public int Multiplicity { get; set; } = 1;

        public MPCModel(Point point)
        {
            Point = point;
        }
    }
}
