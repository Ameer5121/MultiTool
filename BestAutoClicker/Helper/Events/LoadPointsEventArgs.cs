using BestAutoClicker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BestAutoClicker.Helper.Events
{
    internal class LoadPointsEventArgs : EventArgs
    {
        public IEnumerable<MPCModel> Models { get; }

        public LoadPointsEventArgs(IEnumerable<MPCModel> models)
        {
            Models = models;    
        }
    }
}
