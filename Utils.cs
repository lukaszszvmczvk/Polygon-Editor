using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab1
{
    public static class Utils
    {
        public static int mod(int x, int m)
        {
            return (x % m + m) % m;
        }
    }
}
