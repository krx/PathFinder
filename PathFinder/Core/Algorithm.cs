using System.Collections.Generic;

namespace PathFinder.Core {

    // Delegate function definition to be used for all search algorithm functions
    delegate List<Node> AlgoFunc(Node start, Node end, Grid grid, HeuristicFunc heuristic, bool diagAllowed, bool crossDiagAllowed, History hist);

    /// <summary>
    /// Contains static references to all search algorithms to be used in bindings
    /// </summary>
    internal class Algorithm {
        public static AlgoFunc AStar = Finders.AStar.Search;
        public static AlgoFunc BreadthFirst = Finders.BreadthFirst.Search;
        public static AlgoFunc DepthFirst = Finders.DepthFirst.Search;
        public static AlgoFunc DepthFirstRandom = Finders.DepthFirstRandom.Search;
        public static AlgoFunc HillClimbing = Finders.HillClimbing.Search;
        public static AlgoFunc BestFirst = Finders.BestFirst.Search;
        public static AlgoFunc Dijkstra = Finders.Dijkstra.Search;
        public static AlgoFunc JumpPoint = (start, end, grid, heuristic, allowed, diagAllowed, hist) => new List<Node>();
    }
}
