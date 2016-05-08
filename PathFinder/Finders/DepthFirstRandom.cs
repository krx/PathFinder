using System.Collections.Generic;
using System.Linq;
using PathFinder.Core;

namespace PathFinder.Finders {
    internal class DepthFirstRandom {
        public static List<Node> Search(Node start, Node end, Grid grid, HeuristicFunc heuristic, bool diagAllowed, bool crossDiagAllowed, History hist) {
            List<Node> closed = new List<Node>();
            Node current = start;
            start.Parent = null;
            while (current != null) {
                if (current.IsEnd) return Util.Backtrace(current);

                closed.Add(current);
                hist.Push(current, NodeState.Closed);

                List<Node> neighbors = Util.GetNeighbors(current, grid, diagAllowed, crossDiagAllowed).Where(n => !closed.Contains(n)).ToList();
                neighbors.ForEach(neighbor => {
                    neighbor.Parent = current;
                    hist.Push(neighbor, NodeState.Open);
                });
                current = neighbors.Count > 0 ? Util.GetRandomElement(neighbors) : current.Parent;
            }
            return null;
        }
    }
}
