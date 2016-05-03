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

    class Node : INotifyPropertyChanged, ICloneable {
        public static readonly double Nodesize = 30;

        public int X { get; }
        public int Y { get; }
        public int GScore { get; set; }
        public int HScore { get; set; }
        public int FScore => GScore + HScore;

        private NodeState _state;
        public NodeState State {
            get { return _state; }
            set {
                _state = value;
                OnPropertyChanged("State");
            }
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

        public object Clone() {
            return new Node(X, Y, State);
        }
    }


}
