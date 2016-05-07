using System.Collections.Generic;
using System.Linq;
using PathFinder.Core;

namespace PathFinder.Finders {
    internal class HillClimbing {
        public static List<Node> Search(Node start, Node end, Grid grid, HeuristicFunc heuristic, bool diagAllowed, bool crossDiagAllowed, History hist) {
            if (!end.IsWalkable) return null;
            List<Node> closed = new List<Node>();
            Node current = start;
            start.Parent = null;
            start.GScore = 0;
            start.HScore = heuristic(start, end);
            while (current != null) {
                if (current.Equals(end)) return Util.Backtrace(current);

                if (!closed.Contains(current)) {
                    closed.Add(current);
                    if (current.State != NodeState.Start && current.State != NodeState.End) hist.Push(current, NodeState.Closed);
                }

                List<Node> neighbors = Util.GetNeighbors(current, grid, diagAllowed, crossDiagAllowed).Where(n => !closed.Contains(n)).ToList();
                neighbors.ForEach(neighbor => {
                    neighbor.GScore = 0;
                    neighbor.HScore = heuristic(neighbor, end);
                    neighbor.Parent = current;
                    if (neighbor.State != NodeState.End) hist.Push(neighbor, NodeState.Open);
                });
                neighbors.Sort();
                current = neighbors.Count > 0 ? neighbors[0] : current.Parent;
            }
            return null;
        }
    }
}
