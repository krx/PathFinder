using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PathFinder {
    class Grid {
        public ObservableCollection<Node> Nodes { get; }
        public ObservableCollection<PathSegment> Path { get; }

        public int Width { get; private set; }
        public int Height { get; private set; }

        public Node this[int row, int col] {
            get { return Nodes[row * Width + col]; }
            private set { Nodes[row * Width + col] = value; }
        }

        public Grid() {
            Nodes = new ObservableCollection<Node>();
            Path = new ObservableCollection<PathSegment>();
            InitializeGrid(5, 5);
        }

        public void InitializeGrid(int width, int height) {
            Width = width;
            Height = height;
            Nodes.Clear();
            for (int y = 0; y < Height; ++y) {
                for (int x = 0; x < Width; ++x) {
                    Nodes.Add(new Node(x, y));
                }
            }
        }

        public void ResizeGrid(int width, int height) {
            ObservableCollection<Node> copy = new ObservableCollection<Node>(Nodes);
            int oldWidth = Width, oldHeight = Height;
            bool startCopied = false, endCopied = false;
            InitializeGrid((width / (int) Node.Nodesize) + 2, (height / (int) Node.Nodesize) + 2);

            //Copy over the previous grid state
            for (int row = 0; row < Math.Min(Height, oldHeight); ++row) {
                for (int col = 0; col < Math.Min(Width, oldWidth); ++col) {
                    Node n = copy[row * oldWidth + col];
                    if (n.State == NodeState.Start) startCopied = true;
                    if (n.State == NodeState.End) endCopied = true;
                    this[row, col] = n;
                }
            }

            //Check if the start/end nodes are missing, replace them if they are
            if (!startCopied) this[0, this[0, 0].State == NodeState.End ? 1 : 0].State = NodeState.Start;
            if (!endCopied) this[0, this[0, 1].State == NodeState.Start ? 0 : 1].State = NodeState.End;
        }

        public void ClearWalls() {
            Nodes.ToList()
                .Where(n => n.State != NodeState.Start && n.State != NodeState.End).ToList()
                .ForEach(n => n.State = NodeState.Empty);
        }

        public void ClearPath() {
            if (Path.Count > 0) Path.Clear();
            Nodes.ToList()
                .Where(n => n.State == NodeState.Open || n.State == NodeState.Closed).ToList()
                .ForEach(n => n.State = NodeState.Empty);
        }

        public void GenPath(List<Node> trace) {
            Path.Clear();
            for (int i = 0; i < trace.Count - 1; ++i) {
                Path.Add(new PathSegment(trace[i], trace[i + 1]));
            }
        }
    }

    class PathSegment {
        public int X1 { get; }
        public int Y1 { get; }
        public int X2 { get; }
        public int Y2 { get; }

        public PathSegment(Node n1, Node n2) {
            X1 = n1.CenterX;
            Y1 = n1.CenterY;
            X2 = n2.CenterX;
            Y2 = n2.CenterY;
        }
    }
}
