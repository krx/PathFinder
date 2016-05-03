using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinder {
    class Util {
        public static bool isValid(int x, int y, Grid grid) {
            return 0 <= x && x < grid.Width && 0 <= y && y < grid.Height;
        }
    }
}
