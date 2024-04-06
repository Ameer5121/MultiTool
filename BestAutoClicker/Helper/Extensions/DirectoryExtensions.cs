using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BestAutoClicker.Helper.Extensions
{
    internal static class DirectoryExtensions
    {
        public static void TryCreateInitialDirectory(this string DirectoryName)
        {
            if (!Directory.Exists(DirectoryName)) Directory.CreateDirectory(DirectoryName);
        }
    }
}
