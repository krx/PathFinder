using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace PathFinder {
    enum NodeState {
        Empty, Wall, Open, Closed, Start, End
    }

    class Node : INotifyPropertyChanged, ICloneable, IComparable {
        public static readonly double Nodesize = 30;

        public int X { get; }
        public int Y { get; }
        public int CoordX => (int) (X * Nodesize);
        public int CoordY => (int) (Y * Nodesize);
        public int CenterX => (int) (CoordX + Nodesize / 2.0);
        public int CenterY => (int) (CoordY + Nodesize / 2.0);

        public double GScore { get; set; }
        public double HScore { get; set; }
        public double FScore => GScore + HScore;
        public Node Parent { get; set; }
        public bool IsWalkable => State != NodeState.Wall;

        private NodeState _state;
        public NodeState State {
            get { return _state; }
            set { _state = value; OnPropertyChanged("State"); }
        }

        public Node(int x, int y, NodeState state = NodeState.Empty) {
            X = x;
            Y = y;
            State = state;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void ClearScores() {
            GScore = HScore = 0;
        }

        public object Clone() {
            return new Node(X, Y, State);
        }

        public int CompareTo(object o) {
            return FScore.CompareTo(((Node) o).FScore);
        }

        public override bool Equals(object other) {
            Node n = other as Node;
            if (n != null) return X == n.X && Y == n.Y;
            return base.Equals(other);
        }
    }
}
