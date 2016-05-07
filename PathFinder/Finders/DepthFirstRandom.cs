using System;
using System.Collections.Generic;
using System.Linq;
using PathFinder.Core;

namespace PathFinder.Finders {
    internal class DepthFirstRandom {
        public static List<Node> Search(Node start, Node end, Grid grid, HeuristicFunc heuristic, bool diagAllowed, bool crossDiagAllowed, History hist) {
            if (!end.IsWalkable) return null;
            List<Node> closed = new List<Node>();
            Node current = start;
            start.Parent = null;
            Random rnd = new Random();
            while (current != null) {
                if (current.Equals(end)) return Util.Backtrace(current);

                if (!closed.Contains(current)) {
                    closed.Add(current);
                    if (current.State != NodeState.Start && current.State != NodeState.End) hist.Push(current, NodeState.Closed);
                }

                List<Node> neighbors = Util.GetNeighbors(current, grid, diagAllowed, crossDiagAllowed).Where(n => !closed.Contains(n)).ToList();
                neighbors.ForEach(neighbor => {
                    neighbor.Parent = current;
                    if (neighbor.State != NodeState.End) hist.Push(neighbor, NodeState.Open);
                });
                current = neighbors.Count > 0 ? neighbors[rnd.Next(neighbors.Count)] : current.Parent;
            }
            return null;
        }
    }
}
