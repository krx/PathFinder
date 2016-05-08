using System.Collections.Generic;
using PathFinder.Core;

namespace PathFinder.Finders {
    internal class Dijkstra {
        public static List<Node> Search(Node start, Node end, Grid grid, HeuristicFunc heuristic, bool diagAllowed, bool crossDiagAllowed, History hist) {
            // Dijkstra's is simply A* where the H-Score is always 0
            return AStar.Search(start, end, grid, (n, e) => 0, diagAllowed, crossDiagAllowed, hist);
        }
    }
}
