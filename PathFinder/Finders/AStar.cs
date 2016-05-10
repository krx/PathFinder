using System.Collections.Generic;
using System.Linq;
using PathFinder.Core;

namespace PathFinder.Finders {
    internal class AStar {
        // This follows the AlgoFunc template and just calls the extended function
        public static List<Node> Search(Node start, Node end, Grid grid, HeuristicFunc heuristic, bool diagAllowed, bool crossDiagAllowed, History hist) {
            // The default calculation for the G-Score is the same as the Euclidean heuristic
            return Search(start, end, grid, heuristic, diagAllowed, crossDiagAllowed, hist, Heuristic.Euclidean);
        }

        /// <summary>
        /// Finds a path between two Nodes using the A* search algorithm
        /// </summary>
        /// <param name="start">The starting Node in the Grid</param>
        /// <param name="end">The ending Node in the Grid</param>
        /// <param name="grid">The Grid containing everything</param>
        /// <param name="heuristic">Heuristic function to use for scoring Nodes</param>
        /// <param name="diagAllowed">Whether diagonal movement is allowed</param>
        /// <param name="crossDiagAllowed">Whether moving across a corner is allowed</param>
        /// <param name="hist">A blank history to store all state changes that took place</param>
        /// <param name="gFunc">A function that follows the same template as heuristics used for calculating the G-Score</param>
        /// <returns>A path between the start and end Nodes. If no path is found, an empty list is returned</returns>
        public static List<Node> Search(Node start, Node end, Grid grid, HeuristicFunc heuristic, bool diagAllowed, bool crossDiagAllowed, History hist, HeuristicFunc gFunc) {
            List<Node> closed = new List<Node>();
            List<Node> open = new List<Node> { start };

            // Update start scores for reference
            start.Reset();
            start.HScore = heuristic(start, end);

            // Run until there are no more Nodes to explore
            while (open.Count > 0) {
                // Get the first open Node (best F-Score)
                Node current = open.PopFirst();

                // If this is the end, return the path
                if (current.IsEnd) return Util.Backtrace(current);

                // Close the open node
                closed.Add(current);
                hist.Push(current, NodeState.Closed);

                // Process every neighbor that isn't closed
                foreach (Node neighbor in Util.GetNeighbors(current, grid, diagAllowed, crossDiagAllowed).Except(closed)) {
                    // Calculate what the new cost to travel to this neighbor would be
                    double ng = current.GScore + gFunc(current, neighbor);

                    // If the node hasn't been opened yet, add it to the open list
                    // If it has been opened and this path is shorter, overwrite the existing scores and parent
                    if (!open.Contains(neighbor) || ng < neighbor.GScore) {
                        neighbor.GScore = ng;
                        neighbor.HScore = heuristic(neighbor, end);
                        neighbor.Parent = current;

                        // Open the node if hasn't been done already
                        if (!open.Contains(neighbor)) {
                            open.Add(neighbor);
                            hist.Push(neighbor, NodeState.Open);
                        }
                    }
                }

                // Sort the open list so the lowest F-Scores are first
                open.Sort();
            }
            // Return an empty path on failure
            return new List<Node>();
        }
    }
}
