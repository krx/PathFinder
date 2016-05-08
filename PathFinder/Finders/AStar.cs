using System.Collections.Generic;
using System.Linq;
using PathFinder.Core;

namespace PathFinder.Finders {
    internal class AStar {
        public static List<Node> Search(Node start, Node end, Grid grid, HeuristicFunc heuristic, bool diagAllowed, bool crossDiagAllowed, History hist) {
            // The default calculation for the G-Score is the same as the Euclidean heuristic
            return Search(start, end, grid, heuristic, diagAllowed, crossDiagAllowed, hist, Heuristic.Euclidean);
        }

        public static List<Node> Search(Node start, Node end, Grid grid, HeuristicFunc heuristic, bool diagAllowed, bool crossDiagAllowed, History hist, HeuristicFunc gFunc) {
            List<Node> closed = new List<Node>();
            List<Node> open = new List<Node> { start };
            start.Reset();
            while (open.Count > 0) {
                Node current = open.PopFirst();
                closed.Add(current);

                if (current.IsEnd) return Util.Backtrace(current);
                hist.Push(current, NodeState.Closed);

                foreach (Node neighbor in Util.GetNeighbors(current, grid, diagAllowed, crossDiagAllowed).Where(n => !closed.Contains(n))) {
                    double ng = current.GScore + gFunc(current, neighbor);
                    if (!open.Contains(neighbor) || ng < neighbor.GScore) {
                        neighbor.GScore = ng;
                        neighbor.HScore = heuristic(neighbor, end);
                        neighbor.Parent = current;
                        if (!open.Contains(neighbor)) {
                            open.Add(neighbor);
                            hist.Push(neighbor, NodeState.Open);
                        }
                    }
                }
                open.Sort();
            }
            return new List<Node>();
        }
    }
}
