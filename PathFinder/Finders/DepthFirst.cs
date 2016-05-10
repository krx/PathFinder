using System.Collections.Generic;
using System.Linq;
using PathFinder.Core;

namespace PathFinder.Finders {
    internal class DepthFirst {
        public static List<Node> Search(Node start, Node end, Grid grid, HeuristicFunc heuristic, bool diagAllowed, bool crossDiagAllowed, History hist) {
            List<Node> closed = new List<Node>();
            Node current = start;
            start.Reset();
            while (current != null) {
                if (current.IsEnd) return Util.Backtrace(current);

                if (!closed.Contains(current)) {
                    closed.Add(current);
                    hist.Push(current, NodeState.Closed);
                }

                List<Node> neighbors = Util.GetNeighbors(current, grid, diagAllowed, crossDiagAllowed).Except(closed).ToList();
                foreach (Node neighbor in neighbors) {
                    neighbor.Parent = current;
                    hist.Push(neighbor, NodeState.Open);
                }
                current = neighbors.Count > 0 ? neighbors[0] : current.Parent;
            }
            return new List<Node>();
        }
    }
}
