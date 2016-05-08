using System.Collections.Generic;
using PathFinder.Core;

namespace PathFinder.Finders {
    internal class BestFirst {
        public static List<Node> Search(Node start, Node end, Grid grid, HeuristicFunc heuristic, bool diagAllowed, bool crossDiagAllowed, History hist) {
            // Best first is simply A* where the G-Score is always 0
            return AStar.Search(start, end, grid, heuristic, diagAllowed, crossDiagAllowed, hist, (n, e) => 0);
        }
    }
}
