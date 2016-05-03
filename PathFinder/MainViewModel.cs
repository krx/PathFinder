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
using System.Windows.Threading;

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

        // Search playback
        enum PlaybackState {
            Play, Pause, Stop
        }
        private History hist;
        PlaybackState playState = PlaybackState.Stop;


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
            hist = new History(Grid);

            Algo = Algorithm.AStar;
            HeuristicFunction = Heuristic.Manhattan;
            DiagonalsAllowed = true;
            CornerCutAllowed = true;

            ClearWallsCommand = new RelayCommand(o => Grid.ClearWalls());
            ClearPathCommand = new RelayCommand(o => Grid.ClearPath());
            StartCommand = new RelayCommand(o => StartSearch());
            //            PauseCommand = new RelayCommand(o => playState = PlaybackState.Pause, o => playState == PlaybackState.Play);
            //            StopCommand = new RelayCommand(o => playState = PlaybackState.Stop, o => playState == PlaybackState.Pause);

            // Start playback thread
            //            new Thread(() => {
            //                while (true) {
            //                    switch (playState) {
            //                        case PlaybackState.Play:
            //                            if (!hist.Step()) playState = PlaybackState.Pause;
            //                            break;
            //                        case PlaybackState.Pause:
            //                            Thread.Yield();
            //                            break;
            //                        case PlaybackState.Stop:
            //                            hist.Reset();
            //                            break;
            //                        default:
            //                            throw new ArgumentOutOfRangeException();
            //                    }
            //                    Thread.Sleep(4);
            //                }
            //            }).Start();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void StartSearch() {
            //            if (playState == PlaybackState.Stop) {
            hist.Clear();
            Node start = Grid.Nodes.First(n => n.State == NodeState.Start);
            Node end = Grid.Nodes.First(n => n.State == NodeState.End);

            new Thread(() => {
                List<Node> path = Algo(start, end, Grid, HeuristicFunction, DiagonalsAllowed, CornerCutAllowed, hist);
                playState = PlaybackState.Play;
                while (hist.Step()) {
                    Thread.Sleep(4);
                }
                if (path != null) {
                    Application.Current.Dispatcher.Invoke(() => Grid.GenPath(path));
                }
            }).Start();
            //            }
        }


        private void SetStart() {
            if (Util.IsValid(mouseXIdx, mouseYIdx, Grid)) {
                //                Grid.ClearPath();
                Node n = Grid[mouseYIdx, mouseXIdx];
                if (n.State != NodeState.End && n.State != NodeState.Wall) {
                    Grid.Nodes.First(node => node.State == NodeState.Start).State = NodeState.Empty;
                    n.State = NodeState.Start;
                }
            }
        }

        private void SetEnd() {
            if (Util.IsValid(mouseXIdx, mouseYIdx, Grid)) {
                //                Grid.ClearPath();
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
                Grid.ClearPath();
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

    class HeuristicsEnabledConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return new List<AlgoFunc> { Algorithm.AStar, Algorithm.BestFirst, Algorithm.DepthFirstRandom, Algorithm.JumpPoint }.Contains(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
