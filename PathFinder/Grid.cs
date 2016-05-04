using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace PathFinder {
    class Grid : INotifyPropertyChanged {
        private List<Node> _nodes;
        private PointCollection _path;



        public List<Node> Nodes {
            get { return _nodes; }
            set { _nodes = value; OnPropertyChanged("Nodes"); }
        }

        public PointCollection Path {
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
            Path = new PointCollection();
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
            List<Node> copy = new List<Node>(Nodes);
            int oldWidth = Width, oldHeight = Height;
            bool startCopied = false, endCopied = false;
            InitializeGrid((width / (int) Node.Nodesize), (height / (int) Node.Nodesize));

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

        public void ClearAll() {
            if (Path.Count > 0) Path = new PointCollection();
            Nodes.ToList().
                Where(n => n.State != NodeState.Start && n.State != NodeState.End).ToList()
                .ForEach(n => n.State = NodeState.Empty);
        }

        public void ClearPath() {
            if (Path.Count > 0) Path = new PointCollection();
            Nodes.ToList()
                .Where(n => n.State == NodeState.Open || n.State == NodeState.Closed).ToList()
                .ForEach(n => n.State = NodeState.Empty);
        }

        public void GenPath(List<Node> trace) {
            Path = new PointCollection(trace.Select(n => new Point(n.CenterX, n.CenterY)));
        }
    }
}
