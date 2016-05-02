using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PathFinder {
    class Grid {
        private ObservableCollection<Node> _nodes;
        public ObservableCollection<Node> Nodes => _nodes ?? (_nodes = new ObservableCollection<Node>());

        public int Width { get; private set; }
        public int Height { get; private set; }

        public Node this[int row, int col] => Nodes[row * Width + col];

        public Grid() {
            InitializeGrid(10, 10);
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

    }
}
