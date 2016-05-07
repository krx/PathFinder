using System.Collections.Generic;
using System.Linq;
using PathFinder.Core;

namespace PathFinder.Finders {
    internal class BreadthFirst {
        public static List<Node> Search(Node start, Node end, Grid grid, HeuristicFunc heuristic, bool diagAllowed, bool crossDiagAllowed, History hist) {
            if (!end.IsWalkable) return null;
            List<Node> closed = new List<Node>();
            List<Node> open = new List<Node> { start };
            start.Parent = null;
            while (open.Count > 0) {
                Node current = open[0];
                open.RemoveAt(0);
                closed.Add(current);

                if (current.Equals(end)) return Util.Backtrace(current);
                if (current.State != NodeState.Start && current.State != NodeState.End) hist.Push(current, NodeState.Closed);

                Util.GetNeighbors(current, grid, diagAllowed, crossDiagAllowed)
                    .Where(n => !closed.Contains(n) && !open.Contains(n)).ToList()
                    .ForEach(neighbor => {
                        open.Add(neighbor);
                        if (neighbor.State != NodeState.End) hist.Push(neighbor, NodeState.Open);
                        neighbor.Parent = current;
                    });
            }
            return null;
        }
    }
}
