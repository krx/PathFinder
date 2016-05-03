using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace PathFinder {
    class MainViewModel : INotifyPropertyChanged {
        // Private variables
        private bool _diagonalsAllowed;
        private bool _cornerCutAllowed;
        private AlgoFunc _algorithm;
        private HeuristicFunc _heuristic;
        private int mouseXIdx;
        private int mouseYIdx;
        private NodeState dropType = NodeState.Empty;

        public Grid Grid { get; }

        // Binded properties
        public AlgoFunc Algo {
            get { return _algorithm; }
            set { _algorithm = value; OnPropertyChanged("Algo"); }
        }

        public HeuristicFunc HeuristicFunction {
            get { return _heuristic; }
            set { _heuristic = value; OnPropertyChanged("HeuristicFunction"); }
        }

        public bool DiagonalsAllowed {
            get { return _diagonalsAllowed; }
            set { _diagonalsAllowed = value; OnPropertyChanged("DiagonalsAllowed"); }
        }

        public bool CornerCutAllowed {
            get { return _cornerCutAllowed; }
            set { _cornerCutAllowed = value; OnPropertyChanged("CornerCutAllowed"); }
        }


        // Commands
        public ICommand ClearWallsCommand { get; }
        public ICommand ClearPathCommand { get; }
        public ICommand GenMazeCommand { get; }
        public ICommand StartCommand { get; }
        public ICommand PauseCommand { get; }
        public ICommand StopCommand { get; }

        public MainViewModel() {
            Grid = new Grid {
                [0, 0] = { State = NodeState.Start },
                [0, 1] = { State = NodeState.End }
            };

            Algo = Algorithm.AStar;
            HeuristicFunction = Heuristic.Manhattan;
            DiagonalsAllowed = true;
            CornerCutAllowed = true;

            ClearWallsCommand = new RelayCommand(o => ClearWalls());
            ClearPathCommand = new RelayCommand(o => ClearPath());
            StartCommand = new RelayCommand(o => StartSearch());
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void StartSearch() {
            ClearPath();
            Node start = Grid.Nodes.First(n => n.State == NodeState.Start);
            Node end = Grid.Nodes.First(n => n.State == NodeState.End);

            new Thread(() => Algo(start, end, Grid, HeuristicFunction, DiagonalsAllowed, CornerCutAllowed)).Start();
        }

        public void ClearWalls() {
            Grid.Nodes.ToList()
                .Where(n => n.State != NodeState.Start && n.State != NodeState.End).ToList()
                .ForEach(n => n.State = NodeState.Empty);
        }

        public void ClearPath() {
            Grid.Nodes.ToList()
                .Where(n => n.State == NodeState.Open || n.State == NodeState.Closed).ToList()
                .ForEach(n => n.State = NodeState.Empty);
        }

        private void SetStart() {
            if (Util.IsValid(mouseXIdx, mouseYIdx, Grid)) {
                ClearPath();
                Node n = Grid[mouseYIdx, mouseXIdx];
                if (n.State != NodeState.End && n.State != NodeState.Wall) {
                    Grid.Nodes.First(node => node.State == NodeState.Start).State = NodeState.Empty;
                    n.State = NodeState.Start;
                }
            }
        }

        private void SetEnd() {
            if (Util.IsValid(mouseXIdx, mouseYIdx, Grid)) {
                ClearPath();
                Node n = Grid[mouseYIdx, mouseXIdx];
                if (n.State != NodeState.Start && n.State != NodeState.Wall) {
                    Grid.Nodes.First(node => node.State == NodeState.End).State = NodeState.Empty;
                    n.State = NodeState.End;
                }
            }
        }

        private void PaintState() {
            if (Util.IsValid(mouseXIdx, mouseYIdx, Grid)) {
                Node n = Grid[mouseYIdx, mouseXIdx];
                if (n.State != NodeState.Start && n.State != NodeState.End) {
                    n.State = dropType;
                }
            }
        }

        public void OnMouseMoved(Point p) {
            mouseXIdx = (int) (p.X / Node.Nodesize);
            mouseYIdx = (int) (p.Y / Node.Nodesize);

            //Dragging
            if (Mouse.LeftButton == MouseButtonState.Pressed) {
                switch (dropType) {
                    case NodeState.Start:
                        SetStart();
                        break;
                    case NodeState.End:
                        SetEnd();
                        break;
                    default:
                        PaintState();
                        break;
                }
            }
        }

        public void OnLeftMouseDown() {
            if (Util.IsValid(mouseXIdx, mouseYIdx, Grid)) {
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
                PaintState();
            }
        }

        public void OnLeftMouseUp() {
        }
    }

    class RadioIsCheckedConverter : IValueConverter {
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

    class HeuristicsEnabledConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return new List<AlgoFunc> { Algorithm.AStar, Algorithm.BestFirst, Algorithm.HillClimbing, Algorithm.JumpPoint }.Contains(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
