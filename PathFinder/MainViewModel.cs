using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace PathFinder {
    class MainViewModel : INotifyPropertyChanged {
        public Grid Grid { get; }

        private AlgorithmType _algorithm;
        public AlgorithmType Algorithm { get { return _algorithm; } set { _algorithm = value; OnPropertyChanged("Algorithm"); } }

        private HeuristicType _heuristic;
        public HeuristicType Heuristic { get { return _heuristic; } set { _heuristic = value; OnPropertyChanged("Heuristic"); } }


        private int mouseXIdx;
        private int mouseYIdx;
        private NodeState dropType = NodeState.Empty;

        public MainViewModel() {
            Grid = new Grid {
                [0, 0] = { State = NodeState.Start },
                [0, 1] = { State = NodeState.End }
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;


        protected virtual void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void clearWalls() {

        }

        private void clearPath() {

        }

        private void setStart() {
            if (Util.isValid(mouseXIdx, mouseYIdx, Grid)) {
                Node n = Grid[mouseYIdx, mouseXIdx];
                if (n.State != NodeState.End && n.State != NodeState.Wall) {
                    foreach (Node node in Grid.Nodes) {
                        if (node.State == NodeState.Start) {
                            node.State = NodeState.Empty;
                            break;
                        }
                    }
                    n.State = NodeState.Start;
                }
            }
        }

        private void setEnd() {
            if (Util.isValid(mouseXIdx, mouseYIdx, Grid)) {
                Node n = Grid[mouseYIdx, mouseXIdx];
                if (n.State != NodeState.Start && n.State != NodeState.Wall) {
                    foreach (Node node in Grid.Nodes) {
                        if (node.State == NodeState.End) {
                            node.State = NodeState.Empty;
                            break;
                        }
                    }
                    n.State = NodeState.End;
                }
            }
        }

        private void paintState() {
            if (Util.isValid(mouseXIdx, mouseYIdx, Grid)) {
                Node n = Grid[mouseYIdx, mouseXIdx];
                if (n.State != NodeState.Start && n.State != NodeState.End) {
                    n.State = dropType;
                }
            }
        }

        public void OnMouseMoved(Point p) {
            mouseXIdx = (int) (p.X / Node.Nodesize);
            mouseYIdx = (int) (p.Y / Node.Nodesize);
            Console.WriteLine(new Point(mouseXIdx, mouseYIdx));

            //Dragging
            if (Mouse.LeftButton == MouseButtonState.Pressed) {
                switch (dropType) {
                    case NodeState.Start:
                        setStart();
                        break;
                    case NodeState.End:
                        setEnd();
                        break;
                    default:
                        paintState();
                        break;
                }
            }
        }

        public void OnLeftMouseDown() {
            if (Util.isValid(mouseXIdx, mouseYIdx, Grid)) {
                switch (Grid[mouseYIdx, mouseXIdx].State) {
                    case NodeState.Open:
                    case NodeState.Closed:
                    case NodeState.Empty:
                        dropType = NodeState.Wall;
                        break;
                    case NodeState.Wall:
                        dropType = NodeState.Empty;
                        break;
                    default:
                        dropType = Grid[mouseYIdx, mouseXIdx].State;
                        break;
                }
                paintState();
            }
        }

        public void OnLeftMouseUp() {
        }
    }

    public class RadioIsCheckedConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return parameter.Equals(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return (bool) value ? parameter : Binding.DoNothing;
        }
    }

    class NodeIdxToCoord : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return (int) value * (int) Node.Nodesize;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }


    public enum HeuristicType {
        Manhattan,
        Euclidean,
        Chebyshev
    }

    public enum AlgorithmType {
        AStar,
        BreadthFirst,
        DepthFirst,
        HillClimbing,
        BestFirst,
        Dijkstra,
        JumpPoint
    }
}
