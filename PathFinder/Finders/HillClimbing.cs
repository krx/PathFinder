using System.Collections.Generic;
using System.Linq;
using PathFinder.Core;

namespace PathFinder.Finders {
    internal class HillClimbing {
        public static List<Node> Search(Node start, Node end, Grid grid, HeuristicFunc heuristic, bool diagAllowed, bool crossDiagAllowed, History hist) {
            List<Node> closed = new List<Node>();
            Node current = start;

            //Update start scores for reference
            start.Reset();
            start.HScore = heuristic(start, end);

            while (current != null) {
                if (current.IsEnd) return Util.Backtrace(current);

                if (!closed.Contains(current)) {
                    closed.Add(current);
                    hist.Push(current, NodeState.Closed);
                }

                List<Node> neighbors = Util.GetNeighbors(current, grid, diagAllowed, crossDiagAllowed).Except(closed).ToList();
                foreach (Node neighbor in neighbors) {
                    neighbor.GScore = 0;
                    neighbor.HScore = heuristic(neighbor, end);
                    neighbor.Parent = current;
                    hist.Push(neighbor, NodeState.Open);
                }
                neighbors.Sort();
                current = neighbors.Count > 0 ? neighbors[0] : current.Parent;
            }
            return new List<Node>();
        }
    }
}
