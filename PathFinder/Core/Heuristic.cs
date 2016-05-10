using System;

namespace PathFinder.Core {

    /// <summary>
    /// Delegate function definition for all heurisitic functions
    /// </summary>
    /// <param name="n">The current node</param>
    /// <param name="end">The end node</param>
    /// <returns>A number that gives an estimate of the distance between the current and end Nodes</returns>
    internal delegate double HeuristicFunc(Node n, Node end);

    /// <summary>
    /// Contains static references to all heuristic functions to be used in bindings
    /// </summary>
    internal class Heuristic {
        /// <summary>
        /// Manhattan heuristic
        /// Returns the sum of the horiztontal and vertical differences between two nodes
        /// </summary>
        public static HeuristicFunc Manhattan = (n, end)
            => Math.Abs(end.X - n.X) + Math.Abs(end.Y - n.Y);

        /// <summary>
        /// Euclidean heuristic
        /// Returns the exact (euclidean) distance between two nodes
        /// </summary>
        public static HeuristicFunc Euclidean = (n, end) => {
            int dx = Math.Abs(end.X - n.X),
                dy = Math.Abs(end.Y - n.Y);
            return Math.Sqrt(dx * dx + dy * dy);
        };


        /// <summary>
        /// Chebyshev heuristic
        /// Returns the maximum of the horiztontal and vertical differences between two nodes
        /// </summary>
        public static HeuristicFunc Chebyshev = (n, end)
            => Math.Max(Math.Abs(end.X - n.X), Math.Abs(end.Y - n.Y));
    }
}
