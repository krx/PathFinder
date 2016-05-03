using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinder {
    class Util {
        public static bool IsValid(int x, int y, Grid grid) {
            return 0 <= x && x < grid.Width && 0 <= y && y < grid.Height;
        }

        public static bool IsWalkableAt(int x, int y, Grid grid) {
            return IsValid(x, y, grid) && grid[y, x].IsWalkable;
        }

        public static List<Node> backtrace(Node node) {
            List<Node> path = new List<Node> { node };
            while (node.Parent != null) {
                path.Add(node.Parent);
                node = node.Parent;
            }
            path.Reverse();
            return path;
        }

    }
}
