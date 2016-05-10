using System.Collections.Generic;

namespace PathFinder.Core {

    /// <summary>
    /// Delegate function definition to be used for all search algorithm functions
    /// </summary>
    /// <param name="start">The starting Node in the Grid</param>
    /// <param name="end">The ending Node in the Grid</param>
    /// <param name="grid">The Grid containing everything</param>
    /// <param name="heuristic">Heuristic function to use for scoring Nodes</param>
    /// <param name="diagAllowed">Whether diagonal movement is allowed</param>
    /// <param name="crossDiagAllowed">Whether moving across a corner is allowed</param>
    /// <param name="hist">A blank history to store all state changes that took place</param>
    /// <returns>A path between the start and end Nodes. If no path is found, an empty list is returned</returns>
    internal delegate List<Node> AlgoFunc(Node start, Node end, Grid grid, HeuristicFunc heuristic, bool diagAllowed, bool crossDiagAllowed, History hist);

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
