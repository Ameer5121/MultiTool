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
        public Point Point { get; }
        public ClickingMode ClickingMode { get; set; } = ClickingMode.LeftClickDown;

        public MPCModel(Point point)
        {
            Point = point; 
        }
    }
}
