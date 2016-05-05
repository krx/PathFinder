using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinder {
    class MazeGenerator {
        public static void Generate(Grid grid, History hist) {
            Random rnd = new Random();
            grid.Nodes
                .Where(n => n.State != NodeState.Start && n.State != NodeState.End).ToList()
                .ForEach(n => n.State = NodeState.Wall);
            Node current = RandomCell(grid, new List<Node>());
            Stack<Node> traced = new Stack<Node>();
            List<Node> visited = new List<Node> { current };
            while (NumUnvisited(grid, visited) > 0) {
                List<Node> neighbors = GetNeighbors(current.X, current.Y, grid, visited);
                if (neighbors.Count > 0) {
                    Node neighbor = neighbors[rnd.Next(neighbors.Count)];
                    traced.Push(current);
                    int midX = (current.X + neighbor.X) / 2,
                        midY = (current.Y + neighbor.Y) / 2;
                    if (grid[midY, midX].State == NodeState.Wall) {
                        hist.Push(grid[midY, midX], NodeState.Empty);
                    }
                    current = neighbor;
                    visited.Add(current);
                } else if (traced.Count > 0) {
                    current = traced.Pop();
                } else {
                    visited.Add(current);
                    current = RandomCell(grid, visited);
                }
                if (current.State == NodeState.Wall) {
                    hist.Push(current, NodeState.Empty);
                }
            }
        }

        private static int NumUnvisited(Grid grid, List<Node> visited) {
            return grid.Nodes
                .Where(n => n.X % 2 == 0 && n.Y % 2 == 0 && !visited.Contains(n)).ToList()
                .Count;
        }

        private static Node RandomCell(Grid grid, List<Node> visited) {
            Node n;
            Random rnd = new Random();
            do {
                n = grid.Nodes[rnd.Next(grid.Nodes.Count)];
            } while (n.X % 2 != 0 || n.Y % 2 != 0 || visited.Contains(n));
            return n;
        }

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
