using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace PathFinder.Core {

    /// <summary>
    /// A grid containing nodes and a path between them
    /// </summary>
    internal class Grid : NotifyPropertyChangedBase {

        /// <summary>
        /// Direct list of all nodes contained in this grid
        /// </summary>
        private List<Node> _nodes;
        public List<Node> Nodes {
            get { return _nodes; }
            set { _nodes = value; OnPropertyChanged("Nodes"); }
        }

        /// <summary>
        /// Collection of points that make up the path connecting this grid's nodes
        /// </summary>
        private PointCollection _path;
        public PointCollection Path {
            get { return _path; }
            set { _path = value; OnPropertyChanged("Path"); }
        }

        // Grid size
        public int Width { get; private set; }
        public int Height { get; private set; }

        // Node accessors
        public Node StartNode => Nodes.First(n => n.State == NodeState.Start);
        public Node EndNode => Nodes.First(n => n.State == NodeState.End);

        // Access any node by row/col (y/x) index
        public Node this[int row, int col] => Nodes[row * Width + col];

        /// <summary>
        /// Create a small (5x5) grid with start and end nodes by default
        /// </summary>
        public Grid() {
            Nodes = new List<Node>();
            Path = new PointCollection();
            InitializeGrid(5, 5);

        }

        /// <summary>
        /// Creates a new list of nodes matching grid dimensions
        /// </summary>
        /// <param name="width">Width of the grid in nodes</param>
        /// <param name="height">Height of the grid in nodes</param>
        public void InitializeGrid(int width, int height) {
            // Set dimensions
            Width = width;
            Height = height;

            //Generate the new list of nodes
            List<Node> newNodes = new List<Node>();
            for (int y = 0; y < Height; ++y) {
                for (int x = 0; x < Width; ++x) {
                    newNodes.Add(new Node(x, y));
                }
            }
            Nodes = newNodes;
        }

        /// <summary>
        /// Resize the grid to fit as many nodes as possible in the given pixel dimensions
        /// </summary>
        /// <param name="pxWidth"></param>
        /// <param name="pxHeight"></param>
        public void ResizeGrid(int pxWidth, int pxHeight) {
            // Create copies of old data before regenerating the grid
            List<Node> copy = new List<Node>(Nodes);
            int oldWidth = Width, oldHeight = Height;

            // Recreate the grid with the new sizes
            InitializeGrid((pxWidth / (int) Node.Nodesize), (pxHeight / (int) Node.Nodesize));

            //Copy over the previous grid state, keeping track of start/end nodes
            bool startCopied = false, endCopied = false;
            for (int row = 0; row < Math.Min(Height, oldHeight); ++row) {
                for (int col = 0; col < Math.Min(Width, oldWidth); ++col) {
                    Node n = copy[row * oldWidth + col];
                    if (n.State == NodeState.Start) startCopied = true;
                    if (n.State == NodeState.End) endCopied = true;
                    this[row, col].State = n.State;
                }
            }

            //Check if the start/end nodes are missing, replace them as one of the first two nodes if they are
            if (!startCopied) {
                this[0, this[0, 0].State == NodeState.End ? 1 : 0].State = NodeState.Start;
            }
            if (!endCopied) {
                this[0, this[0, 1].State == NodeState.Start ? 0 : 1].State = NodeState.End;
            }
        }

        /// <summary>
        /// Clears the current start node and sets another node as the new start
        /// </summary>
        /// <param name="x">X index of the new start</param>
        /// <param name="y">Y index of the new start</param>
        public void SetStart(int x, int y) {
            // Validity check
            if (!Util.IsValid(x, y, this)) return;

            // Check if the desired node can be set as the start node
            Node n = this[y, x];
            if (n.State == NodeState.End || n.State == NodeState.Wall) return;

            // Clear the old start and set the new one
            StartNode.State = NodeState.Empty;
            n.State = NodeState.Start;
        }

        /// <summary>
        /// Clears the current end node and sets another node as the new end
        /// </summary>
        /// <param name="x">X index of the new end</param>
        /// <param name="y">Y index of the new end</param>
        public void SetEnd(int x, int y) {
            // Validity check
            if (!Util.IsValid(x, y, this)) return;

            // Check if the desired node can be set as the end node
            Node n = this[y, x];
            if (n.State == NodeState.Start || n.State == NodeState.Wall) return;

            // Clear the old end and set the new one
            EndNode.State = NodeState.Empty;
            n.State = NodeState.End;
        }

        /// <summary>
        /// Clears the path and resets all nodes to the empty state
        /// </summary>
        public void ClearAll() {
            if (Path.Count > 0) Path = new PointCollection();
            foreach (Node n in Nodes.Where(n => !n.IsStart && !n.IsEnd)) {
                n.State = NodeState.Empty;
                n.Reset();
            }
        }

        /// <summary>
        /// Clears the path and clears all open and closed nodes
        /// </summary>
        public void ClearPath() {
            if (Path.Count > 0) Path = new PointCollection();
            foreach (Node n in Nodes.Where(n => n.IsOpen || n.IsClosed)) {
                n.State = NodeState.Empty;
                n.Reset();
            }
        }

        /// <summary>
        /// Generates a list of path points from the bracktrace of a search
        /// </summary>
        /// <param name="trace"></param>
        public void GenPath(List<Node> trace) {
            Path = new PointCollection(trace.Select(n => new Point(n.CenterX, n.CenterY)));
        }
    }
}
