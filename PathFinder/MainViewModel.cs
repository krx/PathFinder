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
        private List<Node> path;
        private Thread playThread;

        // Search playback
        enum PlaybackState {
            Playing, Restart, Paused, SearchComplete, Modified, GeneratingMaze
        }
        private History hist;
        volatile PlaybackState playState = PlaybackState.Modified;


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
        public ICommand ClearAllCommand { get; }
        public ICommand ClearPathCommand { get; }
        public ICommand GenMazeCommand { get; }
        public ICommand StartCommand { get; }
        public ICommand PauseCommand { get; }
        public ICommand StopCommand { get; }

        public MainViewModel() {
            Grid = new Grid();
            hist = new History(Grid);

            Algo = Algorithm.AStar;
            HeuristicFunction = Heuristic.Manhattan;
            DiagonalsAllowed = true;
            CornerCutAllowed = true;

            StartCommand = new RelayCommand(o => StartSearch(), o => playState != PlaybackState.GeneratingMaze);
            PauseCommand = new RelayCommand(o => playState = PlaybackState.Paused, o => playState == PlaybackState.Playing);
            StopCommand = new RelayCommand(o => {
                playState = PlaybackState.Modified;
                hist.Clear();
            }, o => playState == PlaybackState.Paused);



            ClearAllCommand = new RelayCommand(o => {
                playState = PlaybackState.Modified;
                Grid.ClearAll();
            }, o => CanModify() && playState != PlaybackState.GeneratingMaze);
            ClearPathCommand = new RelayCommand(o => {
                playState = PlaybackState.Modified;
                Grid.ClearPath();
            }, o => CanModify() && playState != PlaybackState.GeneratingMaze);
            GenMazeCommand = new RelayCommand(o => GenMaze(), o => CanModify() && playState != PlaybackState.GeneratingMaze);

        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool CanModify() {
            return playState == PlaybackState.Modified || playState == PlaybackState.SearchComplete;
        }

        public void StartSearch() {
            // Don't search again if paused
            bool searchPath = playState != PlaybackState.Paused;

            // Don't search again if path was already found
            if (playState == PlaybackState.Restart || playState == PlaybackState.SearchComplete) {
                hist.Reset(); // Back to start
                searchPath = false;
            }
            if (searchPath) {
                hist.Clear();
                path = Algo(Grid.StartNode, Grid.EndNode, Grid, HeuristicFunction, DiagonalsAllowed, CornerCutAllowed, hist);
            }

            playState = PlaybackState.Playing;
            playThread?.Abort();
            playThread = new Thread(() => {
                while (hist.Step()) {
                    Thread.Sleep(4);
                    if (playState != PlaybackState.Playing) return;
                }
                playState = PlaybackState.SearchComplete;
                if (path != null) Application.Current.Dispatcher.Invoke(() => {
                    Grid.GenPath(path);
                    CommandManager.InvalidateRequerySuggested();

                });
            });
            playThread.Start();
        }

        public void GenMaze() {
            hist.Clear(false);
            Grid.Path = new PointCollection();
            DiagonalsAllowed = false;
            playState = PlaybackState.GeneratingMaze;
            new Thread(() => {
                MazeGenerator.Generate(Grid, hist);
                while (hist.Step()) {
                    Thread.Sleep(4);
                }
                playState = PlaybackState.Modified;
                Application.Current.Dispatcher.Invoke(CommandManager.InvalidateRequerySuggested);
            }).Start();
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
            if (CanModify() && playState != PlaybackState.GeneratingMaze) {
                mouseXIdx = (int) (p.X / Node.Nodesize);
                mouseYIdx = (int) (p.Y / Node.Nodesize);

                //Dragging
                if (Mouse.LeftButton == MouseButtonState.Pressed) {
                    switch (dropType) {
                        case NodeState.Start:
                            Grid.SetStart(mouseXIdx, mouseYIdx);
                            break;
                        case NodeState.End:
                            Grid.SetEnd(mouseXIdx, mouseYIdx);
                            break;
                        default:
                            PaintState();
                            break;
                    }
                }
            }
        }

        public void OnLeftMouseDown() {
            if (CanModify() && playState != PlaybackState.GeneratingMaze && Util.IsValid(mouseXIdx, mouseYIdx, Grid)) {
                playState = PlaybackState.Modified;
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

    class HeuristicsEnabledConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return new List<AlgoFunc> { Algorithm.AStar, Algorithm.BestFirst, Algorithm.HillClimbing, Algorithm.JumpPoint }.Contains(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
