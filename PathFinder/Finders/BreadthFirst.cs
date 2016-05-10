using System.Collections.Generic;
using System.Linq;
using PathFinder.Core;

namespace PathFinder.Finders {
    internal class BreadthFirst {
        public static List<Node> Search(Node start, Node end, Grid grid, HeuristicFunc heuristic, bool diagAllowed, bool crossDiagAllowed, History hist) {
            List<Node> closed = new List<Node>();
            List<Node> open = new List<Node> { start };
            start.Reset();

            // Run until there are no more Nodes to explore
            while (open.Count > 0) {
                // Get the first open Node
                Node current = open.PopFirst();

                // If this is the end, return the path
                if (current.IsEnd) return Util.Backtrace(current);

                // Close the open node
                closed.Add(current);
                hist.Push(current, NodeState.Closed);

                // Get all neighbors that haven't already been visited and add them to the open list
                foreach (Node neighbor in Util.GetNeighbors(current, grid, diagAllowed, crossDiagAllowed).Except(closed).Except(open)) {
                    open.Add(neighbor);
                    hist.Push(neighbor, NodeState.Open);
                    neighbor.Parent = current;
                }
            }
            // Return an empty path on failure
            return new List<Node>();
        }
    }
}
