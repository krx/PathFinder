using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinder {

    delegate List<Node> AlgoFunc(Node start, Node end, Grid grid, HeuristicFunc heuristic, bool diagAllowed, bool crossDiagAllowed);

    class Algorithm {
        public static AlgoFunc AStar = (start, end, grid, heuristic, allowed, diagAllowed) => new List<Node>();
        public static AlgoFunc BreadthFirst = (start, end, grid, heuristic, allowed, diagAllowed) => new List<Node>();
        public static AlgoFunc DepthFirst = (start, end, grid, heuristic, allowed, diagAllowed) => new List<Node>();
        public static AlgoFunc HillClimbing = (start, end, grid, heuristic, allowed, diagAllowed) => new List<Node>();
        public static AlgoFunc BestFirst = (start, end, grid, heuristic, allowed, diagAllowed) => new List<Node>();
        public static AlgoFunc Dijkstra = (start, end, grid, heuristic, allowed, diagAllowed) => new List<Node>();
        public static AlgoFunc JumpPoint = (start, end, grid, heuristic, allowed, diagAllowed) => new List<Node>();
    }
}
