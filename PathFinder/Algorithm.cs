using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinder {

    delegate List<Node> AlgoFunc(Node start, Node end, Grid grid, HeuristicFunc heuristic, bool diagAllowed, bool crossDiagAllowed, History hist);

    class Algorithm {
        public static AlgoFunc AStar = (start, end, grid, heuristic, allowed, diagAllowed, hist) => new List<Node>();
        public static AlgoFunc BreadthFirst = Finders.BreadthFirst.Search;
        public static AlgoFunc DepthFirst = (start, end, grid, heuristic, allowed, diagAllowed, hist) => new List<Node>();
        public static AlgoFunc HillClimbing = (start, end, grid, heuristic, allowed, diagAllowed, hist) => new List<Node>();
        public static AlgoFunc BestFirst = Finders.BestFirst.Search;
        public static AlgoFunc Dijkstra = Finders.Dijkstra.Search;
        public static AlgoFunc JumpPoint = (start, end, grid, heuristic, allowed, diagAllowed, hist) => new List<Node>();
    }
}
