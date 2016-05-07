using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace PathFinder.Core {

    /// <summary>
    /// Helpful utitiliy functions used throughout the program
    /// </summary>
    internal class Util {
        private static readonly Random rnd = new Random();

        /// <summary>
        /// Returns a random element of a list
        /// </summary>
        /// <typeparam name="T">Type of element stored in the list</typeparam>
        /// <param name="list">List of elements</param>
        /// <returns>Random element if the list had any, null otherwise</returns>
        public static T GetRandomElement<T>(IList<T> list) {
            if (list.Count == 0) throw new InvalidOperationException("Cannot get an element of an empty list");
            return list[rnd.Next(list.Count)];
        }

        /// <summary>
        /// Checks if a given coordinate is within the bounds of a grid
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <param name="grid">Grid to check the point in</param>
        /// <returns>If the coordinate is valid</returns>
        public static bool IsValid(int x, int y, Grid grid) {
            return 0 <= x && x < grid.Width && 0 <= y && y < grid.Height;
        }

        /// <summary>
        /// Checks if a coordinate is allowed to be walked on,
        /// meaning it is within the bounds of the grid and is not a wall
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <param name="grid">Grid to check the point in</param>
        /// <returns></returns>
        public static bool IsWalkableAt(int x, int y, Grid grid) {
            return IsValid(x, y, grid) && grid[y, x].IsWalkable;
        }

        /// <summary>
        /// Traces and collect a chain of parents until the start node is reached
        /// </summary>
        /// <param name="node">Node to begin tracing from</param>
        /// <returns>List of all nodes in the chain, starting form the Start node</returns>
        public static List<Node> Backtrace(Node node) {
            List<Node> path = new List<Node> { node };
            // Collect all the parents
            while (node.Parent != null) {
                path.Add(node.Parent);
                node = node.Parent;
            }
            // Reverse and return the list
            path.Reverse();
            return path;
        }

        /// <summary>
        /// Gathers all of the valid neighbors of a node according to the movement options
        /// </summary>
        /// <param name="node">Node to find the neighbors of</param>
        /// <param name="grid">Grid the node resides in</param>
        /// <param name="diagAllowed">Whether diagonal neighbors should be considered</param>
        /// <param name="crossDiagAllowed">If diagonals are considered, this allows crossing along a corner</param>
        /// <returns>The list of valid neighbors</returns>
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

        /// <summary>
        /// Helper to find the visual descendant of a UI object
        /// </summary>
        /// <typeparam name="T">The desired type of the descendant</typeparam>
        /// <param name="root">The root of the search</param>
        /// <returns>The descendant if it was found, otherwise null</returns>
        public static T FindDescendant<T>(DependencyObject root) where T : DependencyObject {
            if (root == null) {
                return null;
            }

            // Check if this object is the specified type
            if (root is T) {
                return (T) root;
            }

            // Check for children
            int childCount = VisualTreeHelper.GetChildrenCount(root);
            // If there are none the search failed
            if (childCount < 1) return null;

            // First check if one of the children is the desired object
            for (int i = 0; i < childCount; i++) {
                DependencyObject child = VisualTreeHelper.GetChild(root, i);
                if (child is T) {
                    return child as T;
                }
            }

            // Then check the childrens children
            for (int i = 0; i < childCount; i++) {
                DependencyObject child = FindDescendant<T>(VisualTreeHelper.GetChild(root, i));
                // If anything was returned, the child was found
                if (child != null) {
                    return (T) child;
                }
            }

            return null;
        }
    }
}
