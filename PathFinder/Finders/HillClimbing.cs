using System.Collections.Generic;
using System.Linq;
using PathFinder.Core;

namespace PathFinder.Finders {
    internal class HillClimbing {
        // Note that no open list is used in this algorithm, however any generated nodes are visually marked as open
        public static List<Node> Search(Node start, Node end, Grid grid, HeuristicFunc heuristic, bool diagAllowed, bool crossDiagAllowed, History hist) {
            List<Node> closed = new List<Node>();
            Node current = start;

            //Update start scores for reference
            start.Reset();
            start.HScore = heuristic(start, end);

            // Run until there are no more Nodes to explore
            while (current != null) {
                // If the end has been reached return the path
                if (current.IsEnd) return Util.Backtrace(current);

                // Close the curren't node if it hasn't been done already
                if (!closed.Contains(current)) {
                    closed.Add(current);
                    hist.Push(current, NodeState.Closed);
                }

                // Get all neighbors that haven't been closed
                List<Node> neighbors = Util.GetNeighbors(current, grid, diagAllowed, crossDiagAllowed).Except(closed).ToList();
                // Score each neighbor and mark them as open
                foreach (Node neighbor in neighbors) {
                    neighbor.GScore = 0;
                    neighbor.HScore = heuristic(neighbor, end);
                    neighbor.Parent = current;
                    hist.Push(neighbor, NodeState.Open);
                }
                // Sort the neigbors so the one with the best score is first
                neighbors.Sort();
                // If any neighbors are available, travel to the one with the lowest score. Otherwise backtrack to the parent
                current = neighbors.Count > 0 ? neighbors[0] : current.Parent;
            }
            // Return an empty path on failure
            return new List<Node>();
        }
    }
}
