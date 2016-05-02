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

    class Node : INotifyPropertyChanged {
        public static readonly double Nodesize = 20;

        private Dictionary<NodeState, Color> cmap = new Dictionary<NodeState, Color>() {
            {NodeState.Empty, Colors.White },
            {NodeState.Wall, Colors.Gray },
            {NodeState.Open, Colors.LightBlue },
            {NodeState.Closed, Colors.LightGreen },
            {NodeState.Start, Color.FromRgb(0, 255, 0) },
            {NodeState.End, Colors.Red }
        };

        private int _x;
        public int X {
            get { return _x; }
            set {
                _x = value;
                OnPropertyChanged("X");
            }
        }


        private int _y;
        public int Y {
            get { return _y; }
            set {
                _y = value;
                OnPropertyChanged("Y");
            }
        }

        private Color _col;
        public Color Color {
            get { return _col; }
            set {
                _col = value;
                OnPropertyChanged("Color");
            }
        }

        private NodeState _state;
        public NodeState State {
            get { return _state; }
            set {
                _state = value;
                OnPropertyChanged("State");
                Color = cmap[_state];
            }
        }

        public Node(int x, int y) {
            X = x;
            Y = y;
            State = NodeState.Empty;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }


}
