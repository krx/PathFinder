using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PathFinder {
    class Grid : INotifyPropertyChanged {
        private List<Node> _nodes;
        private List<PathSegment> _path;

        public List<Node> Nodes {
            get { return _nodes; }
            set { _nodes = value; OnPropertyChanged("Nodes"); }
        }

        public List<PathSegment> Path {
            get { return _path; }
            set { _path = value; OnPropertyChanged("Path"); }
        }

        public int Width { get; private set; }
        public int Height { get; private set; }

        public Node StartNode => Nodes.First(n => n.State == NodeState.Start);
        public Node EndNode => Nodes.First(n => n.State == NodeState.End);
        public Node this[int row, int col] => Nodes[row * Width + col];

        public Grid() {
            Nodes = new List<Node>();
            Path = new List<PathSegment>();
            InitializeGrid(5, 5);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void InitializeGrid(int width, int height) {
            Width = width;
            Height = height;
            List<Node> newNodes = new List<Node>();
            for (int y = 0; y < Height; ++y) {
                for (int x = 0; x < Width; ++x) {
                    newNodes.Add(new Node(x, y));
                }
            }
            Nodes = newNodes;
        }

        public void ResizeGrid(int width, int height) {
            ObservableCollection<Node> copy = new ObservableCollection<Node>(Nodes);
            int oldWidth = Width, oldHeight = Height;
            bool startCopied = false, endCopied = false;
            InitializeGrid((width / (int) Node.Nodesize) + 1, (height / (int) Node.Nodesize) + 1);

            //Copy over the previous grid state
            for (int row = 0; row < Math.Min(Height, oldHeight); ++row) {
                for (int col = 0; col < Math.Min(Width, oldWidth); ++col) {
                    Node n = copy[row * oldWidth + col];
                    if (n.State == NodeState.Start) startCopied = true;
                    if (n.State == NodeState.End) endCopied = true;
                    this[row, col].State = n.State;
                }
            }

            //Check if the start/end nodes are missing, replace them if they are
            if (!startCopied) this[0, this[0, 0].State == NodeState.End ? 1 : 0].State = NodeState.Start;
            if (!endCopied) this[0, this[0, 1].State == NodeState.Start ? 0 : 1].State = NodeState.End;
        }

        public void SetStart(int x, int y) {
            if (Util.IsValid(x, y, this)) {
                Node n = this[y, x];
                if (n.State != NodeState.End && n.State != NodeState.Wall) {
                    StartNode.State = NodeState.Empty;
                    n.State = NodeState.Start;
                }
            }
        }

        public void SetEnd(int x, int y) {
            if (Util.IsValid(x, y, this)) {
                Node n = this[y, x];
                if (n.State != NodeState.Start && n.State != NodeState.Wall) {
                    EndNode.State = NodeState.Empty;
                    n.State = NodeState.End;
                }
            }
        }

        public void ClearWalls() {
            Nodes.ToList().
                Where(n => n.State != NodeState.Start && n.State != NodeState.End).ToList()
                .ForEach(n => n.State = NodeState.Empty);
        }

        public void ClearPath() {
            if (Path.Count > 0) Path = new List<PathSegment>();
            Nodes.ToList()
                .Where(n => n.State == NodeState.Open || n.State == NodeState.Closed).ToList()
                .ForEach(n => n.State = NodeState.Empty);
        }

        public void GenPath(List<Node> trace) {
            List<PathSegment> p = new List<PathSegment>();
            for (int i = 0; i < trace.Count - 1; ++i) {
                p.Add(new PathSegment(trace[i], trace[i + 1]));
            }
            Path = p;
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
