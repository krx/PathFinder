using System.Collections.Generic;
using System.Linq;

namespace PathFinder.Core {

    /// <summary>
    /// A randomized maze generator for the grid used in this program
    /// 
    /// This isn't any "official" algorithm that I know of,
    /// but it uses depth-first and backtracing like typical maze generators
    /// </summary>
    internal class MazeGenerator {

        /// <summary>
        /// Gemerate a random maze on the grid, and create a history of the generation
        /// Since the grid this is being made for uses whole nodes as the walls, this essentially
        /// has to operate on every other node in both axes
        /// </summary>
        /// <param name="grid">Grid to generate the maze on</param>
        /// <param name="hist">History to record the state changes to</param>
        public static void Generate(Grid grid, History hist) {
            // Fill the grid with walls
            foreach (Node n in grid.Nodes.Where(n => !n.IsStart && !n.IsEnd)) {
                n.State = NodeState.Wall;
            }

            // Start with a random node
            Node current = RandomNode(grid, new List<Node>());

            // This stack will serve as a way to trace back through the depth-first generation
            Stack<Node> traced = new Stack<Node>();

            // Keep a list of all visited ndoes
            List<Node> visited = new List<Node> { current };

            // Continue until every node has been visted
            while (GetUnvisitedNodes(grid, visited).Count > 0) {
                // Get all the neighbors of the current node
                List<Node> neighbors = GetNeighbors(current.X, current.Y, grid, visited);
                if (neighbors.Count > 0) {
                    // If there are unvisited neighors, choose one of them at random as the next node
                    Node neighbor = Util.GetRandomElement(neighbors);
                    traced.Push(current);

                    // Clear the wall between the current node and the neighbor
                    int midX = (current.X + neighbor.X) / 2,
                        midY = (current.Y + neighbor.Y) / 2;
                    if (grid[midY, midX].IsWall) {
                        hist.Push(grid[midY, midX], NodeState.Empty);
                    }

                    // Move to the neighbor and mark the current node as visited
                    current = neighbor;
                    visited.Add(current);
                } else if (traced.Count > 0) {
                    // If there were no neighbors, attempt to trace backwards until a new neigbor is found
                    current = traced.Pop();
                } else {
                    // If the bracktrace is empty, mark the current node as visited
                    // and jump to a random node in the grid to start again
                    visited.Add(current);
                    current = RandomNode(grid, visited);
                }

                // Clear the current node if it's a wall
                if (current.IsWall) {
                    hist.Push(current, NodeState.Empty);
                }
            }
        }


        /// <summary>
        /// Gathers all unvisited nodes in the grid
        /// Only nodes with even indices will be considered in this count
        /// </summary>
        /// <param name="grid">Grid containing all nodes</param>
        /// <param name="visited">List of visited nodes</param>
        /// <returns>List of unvisited nodes</returns>
        private static List<Node> GetUnvisitedNodes(Grid grid, List<Node> visited) {
            return grid.Nodes.Where(n => n.X % 2 == 0 && n.Y % 2 == 0 && !visited.Contains(n)).ToList();
        }

        /// <summary>
        /// Selects a random unvisited node in the grid
        /// Only nodes with even indices will be considered
        /// </summary>
        /// <param name="grid">Grid containing all nodes</param>
        /// <param name="visited">List of visited nodes</param>
        /// <returns></returns>
        private static Node RandomNode(Grid grid, List<Node> visited) {
            return Util.GetRandomElement(GetUnvisitedNodes(grid, visited));
        }

        /// <summary>
        /// Gathers unvisited neighbors of a node
        /// Neigbors in this case are two nodes away in the horizontial and vertical directions
        /// </summary>
        /// <param name="x">X index of the center node</param>
        /// <param name="y">Y index of the center node</param>
        /// <param name="grid">Grid containing all nodes</param>
        /// <param name="visited">List of visited nodes</param>
        /// <returns></returns>
        private static List<Node> GetNeighbors(int x, int y, Grid grid, List<Node> visited) {
            List<Node> neighbors = new List<Node>();
            if (Util.IsValid(x, y + 2, grid) && !visited.Contains(grid[y + 2, x])) neighbors.Add(grid[y + 2, x]);
            if (Util.IsValid(x, y - 2, grid) && !visited.Contains(grid[y - 2, x])) neighbors.Add(grid[y - 2, x]);
            if (Util.IsValid(x + 2, y, grid) && !visited.Contains(grid[y, x + 2])) neighbors.Add(grid[y, x + 2]);
            if (Util.IsValid(x - 2, y, grid) && !visited.Contains(grid[y, x - 2])) neighbors.Add(grid[y, x - 2]);
            return neighbors;
        }
    }
}
