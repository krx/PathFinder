using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinder.Finders {
    class Dijkstra {
        public static List<Node> Search(Node start, Node end, Grid grid, HeuristicFunc heuristic, bool diagAllowed, bool crossDiagAllowed, History hist) {
            if (!end.IsWalkable) return null;
            List<Node> closed = new List<Node>();
            List<Node> open = new List<Node> { start };
            start.Parent = null;
            start.ClearScores();
            while (open.Count > 0) {
                Node current = open[0];
                open.RemoveAt(0);
                closed.Add(current);

                if (current.Equals(end)) return Util.Backtrace(current);
                if (current.State != NodeState.Start && current.State != NodeState.End) hist.Push(current, NodeState.Closed);

                Util.GetNeighbors(current, grid, diagAllowed, crossDiagAllowed)
                    .Where(n => !closed.Contains(n)).ToList()
                    .ForEach(neighbor => {
                        double ng = current.GScore + Heuristic.Euclidean(current, neighbor);
                        if (!open.Contains(neighbor) || ng < neighbor.GScore) {
                            neighbor.GScore = ng;
                            neighbor.HScore = 0;
                            neighbor.Parent = current;
                            if (!open.Contains(neighbor)) {
                                open.Add(neighbor);
                                if (neighbor.State != NodeState.End) hist.Push(neighbor, NodeState.Open);
                            }
                            open.Sort();
                        }
                    });
            }
            return null;
        }
    }
}
