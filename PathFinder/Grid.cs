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

        public int Width { get; private set; }
        public int Height { get; private set; }

        public Node this[int row, int col] {
            get { return Nodes[row * Width + col]; }
            private set { Nodes[row * Width + col] = value; }
        }

        public Grid() {
            Nodes = new ObservableCollection<Node>();
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
            int ow = Width, oh = Height;
            InitializeGrid((width / (int) Node.Nodesize) + 2, (height / (int) Node.Nodesize) + 2);

            bool startCopied = false, endCopied = false;

            //Copy over the previous grid state
            for (int row = 0; row < Math.Min(Height, oh); ++row) {
                for (int col = 0; col < Math.Min(Width, ow); ++col) {
                    Node n = copy[row * ow + col];
                    if (n.State == NodeState.Start) startCopied = true;
                    if (n.State == NodeState.End) endCopied = true;
                    this[row, col] = n;
                }
            }

            //Check if the start/end nodes are missing, replace them if they are
            if (!startCopied) this[0, this[0, 0].State == NodeState.End ? 1 : 0].State = NodeState.Start;
            if (!endCopied) this[0, this[0, 1].State == NodeState.Start ? 0 : 1].State = NodeState.End;

        }

    }
}
