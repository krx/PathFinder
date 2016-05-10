using System.Collections.Generic;
using System.Linq;
using PathFinder.Core;

namespace PathFinder.Finders {
    internal class DepthFirstRandom {
        // Note that no open list is used in this algorithm, however any generated nodes are visually marked as open
        public static List<Node> Search(Node start, Node end, Grid grid, HeuristicFunc heuristic, bool diagAllowed, bool crossDiagAllowed, History hist) {
            List<Node> closed = new List<Node>();
            Node current = start;
            start.Reset();

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
                // Update their parents to the current Node and mark them as opened
                foreach (Node neighbor in neighbors) {
                    neighbor.Parent = current;
                    hist.Push(neighbor, NodeState.Open);
                }
                // If there are any neighbors available, travel to a random one. Otherwise backtrack to the parent
                current = neighbors.Count > 0 ? Util.GetRandomElement(neighbors) : current.Parent;
            }
            // Return an empty path on failure
            return new List<Node>();
        }
    }
}
