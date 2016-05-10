using System.Collections.Generic;
using System.Linq;
using PathFinder.Core;

namespace PathFinder.Finders {
    internal class BreadthFirst {
        public static List<Node> Search(Node start, Node end, Grid grid, HeuristicFunc heuristic, bool diagAllowed, bool crossDiagAllowed, History hist) {
            List<Node> closed = new List<Node>();
            List<Node> open = new List<Node> { start };
            start.Reset();
            while (open.Count > 0) {
                Node current = open.PopFirst();
                closed.Add(current);

                if (current.IsEnd) return Util.Backtrace(current);
                hist.Push(current, NodeState.Closed);

                foreach (Node neighbor in Util.GetNeighbors(current, grid, diagAllowed, crossDiagAllowed).Except(closed).Except(open)) {
                    open.Add(neighbor);
                    hist.Push(neighbor, NodeState.Open);
                    neighbor.Parent = current;
                }
            }
            return new List<Node>();
        }
    }
}
