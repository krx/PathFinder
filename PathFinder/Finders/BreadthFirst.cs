using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PathFinder.Finders {
    class BreadthFirst {
        public static List<Node> Search(Node start, Node end, Grid grid, HeuristicFunc heuristic, bool diagAllowed, bool crossDiagAllowed) {
            if (!end.IsWalkable) return null;
            List<Node> closed = new List<Node>();
            List<Node> open = new List<Node> { start };
            start.Parent = null;
            while (open.Count > 0) {
                Node current = open[0];
                open.RemoveAt(0);
                closed.Add(current);

                if (current.Equals(end)) return Util.Backtrace(current);
                if (current.State != NodeState.Start && current.State != NodeState.End) current.State = NodeState.Closed;

                foreach (Node neighbor in Util.GetNeighbors(current, grid, diagAllowed, crossDiagAllowed)) {
                    if (closed.Contains(neighbor) || open.Contains(neighbor)) continue;
                    open.Add(neighbor);
                    if (neighbor.State != NodeState.End) neighbor.State = NodeState.Open;
                    neighbor.Parent = current;
                }
            }
            return null;
        }
    }
}
