using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace PathFinder {
    class Util {
        public static bool IsValid(int x, int y, Grid grid) {
            return 0 <= x && x < grid.Width && 0 <= y && y < grid.Height;
        }

        public static bool IsWalkableAt(int x, int y, Grid grid) {
            return IsValid(x, y, grid) && grid[y, x].IsWalkable;
        }

        public static List<Node> Backtrace(Node node) {
            List<Node> path = new List<Node> { node };
            while (node.Parent != null) {
                path.Add(node.Parent);
                node = node.Parent;
            }
            path.Reverse();
            return path;
        }

        public static List<Node> GetNeighbors(Node node, Grid grid, bool diagAllowed, bool crossDiagAllowed) {
            List<Node> neighbors = new List<Node>();
            int x = node.X, y = node.Y;
            // Add adjacent nodes
            bool u = false, r = false, d = false, l = false;
            // Up
            if (IsWalkableAt(x, y - 1, grid)) {
                neighbors.Add(grid[y - 1, x]);
                u = true;
            }
            // Right
            if (IsWalkableAt(x + 1, y, grid)) {
                neighbors.Add(grid[y, x + 1]);
                r = true;
            }
            // Down
            if (IsWalkableAt(x, y + 1, grid)) {
                neighbors.Add(grid[y + 1, x]);
                d = true;
            }
            // Left
            if (IsWalkableAt(x - 1, y, grid)) {
                neighbors.Add(grid[y, x - 1]);
                l = true;
            }

            // If diagonals are allowed, check to see if we can walk to them
            if (diagAllowed) {
                bool ur, dr, dl, ul;
                if (crossDiagAllowed) {
                    // If corner crossing is allowed, at least one of the adjacent nodes have to be walkable
                    ur = u || r;
                    dr = d || r;
                    dl = d || l;
                    ul = u || l;
                } else {
                    // otherwise both have to be walkable
                    ur = u && r;
                    dr = d && r;
                    dl = d && l;
                    ul = u && l;
                }


                // Up-Left
                if (ul && IsWalkableAt(x - 1, y - 1, grid)) {
                    neighbors.Add(grid[y - 1, x - 1]);
                }
                // Up-Right
                if (ur && IsWalkableAt(x + 1, y - 1, grid)) {
                    neighbors.Add(grid[y - 1, x + 1]);
                }
                // Down-Right
                if (dr && IsWalkableAt(x + 1, y + 1, grid)) {
                    neighbors.Add(grid[y + 1, x + 1]);
                }
                // Down-Left
                if (dl && IsWalkableAt(x - 1, y + 1, grid)) {
                    neighbors.Add(grid[y + 1, x - 1]);
                }
            }

            return neighbors;
        }

        public static T FindDescendant<T>(DependencyObject obj) where T : DependencyObject {
            if (obj == null) return null;

            // Check if this object is the specified type
            if (obj is T)
                return obj as T;

            // Check for children
            int childrenCount = VisualTreeHelper.GetChildrenCount(obj);
            if (childrenCount < 1)
                return null;

            // First check all the children
            for (int i = 0; i < childrenCount; i++) {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child is T)
                    return child as T;
            }

            // Then check the childrens children
            for (int i = 0; i < childrenCount; i++) {
                DependencyObject child = FindDescendant<T>(VisualTreeHelper.GetChild(obj, i));
                if (child != null && child is T)
                    return child as T;
            }

            return null;
        }
    }
}
