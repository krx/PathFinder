using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace PathFinder.Core {

    /// <summary>
    /// The different states used for the playback state machine
    /// </summary>
    internal enum PlaybackState {
        Playing, Paused, SearchComplete, Modified, GeneratingMaze
    }

    /// <summary>
    /// Main display controls for the pathfinder
    /// </summary>
    internal class MainViewModel : NotifyPropertyChangedBase {

        // Current node indices the moust is hovering over
        private int mouseXIdx;
        private int mouseYIdx;

        // The state that will be painted as the mouse is dragged
        private NodeState dropType = NodeState.Empty;


        // Search playback
        private History hist;
        private volatile PlaybackState playState = PlaybackState.Modified;
        private Thread playThread;

        // Search results
        private List<Node> path;
        private double algoTime;

        // Grid containing all nodes
        public Grid Grid { get; }

        /// <summary>
        /// The algoritm that will be called to find the path
        /// </summary>
        private AlgoFunc _algorithm;
        public AlgoFunc Algo {
            get { return _algorithm; }
            set {
                _algorithm = value;
                OnPropertyChanged("Algo");
                playState = PlaybackState.Modified;
            }
        }

        /// <summary>
        /// The heuristic function that will be used by the selected search algorithm
        /// </summary>
        private HeuristicFunc _heuristic;
        public HeuristicFunc HeuristicFunction {
            get { return _heuristic; }
            set {
                _heuristic = value;
                OnPropertyChanged("HeuristicFunction");
                playState = PlaybackState.Modified;
            }
        }

        /// <summary>
        /// Whether the path is allowed to use diagonals at all
        /// </summary>
        private bool _diagonalsAllowed;
        public bool DiagonalsAllowed {
            get { return _diagonalsAllowed; }
            set { _diagonalsAllowed = value; OnPropertyChanged("DiagonalsAllowed"); }
        }


        /// <summary>
        /// Wether the diagonals in the path can cross the corner of a wall
        /// </summary>
        private bool _cornerCutAllowed;
        public bool CornerCutAllowed {
            get { return _cornerCutAllowed; }
            set { _cornerCutAllowed = value; OnPropertyChanged("CornerCutAllowed"); }
        }

        /// <summary>
        /// Result string for the path length
        /// </summary>
        private string _statPathLength;
        public string StatPathLength {
            get { return _statPathLength; }
            set { _statPathLength = value; OnPropertyChanged("StatPathLength"); }
        }

        /// <summary>
        /// Result string for the search time
        /// </summary>
        private string _statTime;
        public string StatTime {
            get { return _statTime; }
            set { _statTime = value; OnPropertyChanged("StatTime"); }
        }

        /// <summary>
        /// Result string for the number of nodes explored in the search
        /// </summary>
        private string _statNodesExplored;
        public string StatNodesExplored {
            get { return _statNodesExplored; }
            set { _statNodesExplored = value; OnPropertyChanged("StatNodesExplored"); }
        }

        // Button Commands
        public ICommand ClearAllCommand { get; }
        public ICommand ClearPathCommand { get; }
        public ICommand GenMazeCommand { get; }
        public ICommand StartCommand { get; }
        public ICommand PauseCommand { get; }
        public ICommand StopCommand { get; }

        public MainViewModel() {
            Grid = new Grid();
            hist = new History(Grid);

            // Initial options
            Algo = Algorithm.AStar;
            HeuristicFunction = Heuristic.Manhattan;
            DiagonalsAllowed = true;
            CornerCutAllowed = true;

            // Playback commands
            StartCommand = new RelayCommand(
                o => StartSearch(),
                o => playState != PlaybackState.GeneratingMaze);

            PauseCommand = new RelayCommand(
                o => playState = PlaybackState.Paused,
                o => playState == PlaybackState.Playing);

            StopCommand = new RelayCommand(
                o => {
                    playState = PlaybackState.Modified;
                    hist.Clear();
                },
                o => playState == PlaybackState.Paused);

            // Grid control commands
            ClearAllCommand = new RelayCommand(
                o => {
                    playState = PlaybackState.Modified;
                    Grid.ClearAll();
                },
                o => CanModify());

            ClearPathCommand = new RelayCommand(
                o => {
                    playState = PlaybackState.Modified;
                    Grid.ClearPath();
                },
                o => CanModify());

            GenMazeCommand = new RelayCommand(
                o => GenMaze(),
                o => CanModify());

        }

        /// <returns>If the grid is allowed to be modified right now</returns>
        public bool CanModify() {
            return (playState == PlaybackState.Modified || playState == PlaybackState.SearchComplete) && playState != PlaybackState.GeneratingMaze;
        }

        /// <summary>
        /// Starts the search for a path between the start and end nodes
        /// </summary>
        public void StartSearch() {
            // Only search again if the player isn't paused
            if (playState != PlaybackState.Paused) {
                hist.Clear(); //Clear old path

                // Time how long the search takes
                Stopwatch sw = new Stopwatch();
                sw.Start();
                path = Algo(Grid.StartNode, Grid.EndNode, Grid, HeuristicFunction, DiagonalsAllowed, CornerCutAllowed, hist);
                sw.Stop();

                // Record search time
                algoTime = sw.Elapsed.TotalMilliseconds;
            }

            // Update the playback state and kill any old threads
            playState = PlaybackState.Playing;
            playThread?.Abort();

            // Start the search playback thread
            playThread = new Thread(() => {
                // Keep stepping through history until the end is reached
                while (hist.Step()) {
                    Thread.Sleep(4);

                    // If at any point the playback state changes, exit the thread
                    if (playState != PlaybackState.Playing) {
                        return;
                    }
                }

                // Update statistics
                double len = 0;
                for (int i = 0; i < path.Count - 1; ++i) {
                    len += Heuristic.Euclidean(path[i], path[i + 1]);
                }

                // Update stat strings
                StatPathLength = $"{len:f2}";
                StatTime = $"{algoTime:f4}ms";
                StatNodesExplored = $"{Grid.Nodes.Where(n => n.State == NodeState.Open || n.State == NodeState.Closed).ToList().Count + 2}";

                playState = PlaybackState.SearchComplete;
                Application.Current.Dispatcher.Invoke(() => {
                    // Generate the final path on the grid
                    Grid.GenPath(path);

                    // Force commands to update
                    CommandManager.InvalidateRequerySuggested();
                });
            });
            playThread.Start();
        }

        /// <summary>
        /// Generates a maze on the grid
        /// </summary>
        public void GenMaze() {
            // Prepare by clearing history and path
            hist.Clear(false);
            Grid.Path = new PointCollection();
            DiagonalsAllowed = false;
            playState = PlaybackState.GeneratingMaze;

            // Start the generation thread
            new Thread(() => {
                MazeGenerator.Generate(Grid, hist);

                // Step through history to animate creation
                while (hist.Step()) {
                    Thread.Sleep(4);
                }
                playState = PlaybackState.Modified;

                // Force commands to update
                Application.Current.Dispatcher.Invoke(CommandManager.InvalidateRequerySuggested);
            }).Start();
        }

        /// <summary>
        /// Paint the dropType onto the grid where the mouse is
        /// </summary>
        private void PaintState() {
            // Validity check
            if (!Util.IsValid(mouseXIdx, mouseYIdx, Grid)) return;

            // Set the type if the node isn't the start or end
            Node n = Grid[mouseYIdx, mouseXIdx];
            if (n.State != NodeState.Start && n.State != NodeState.End) {
                n.State = dropType;
            }
        }

        /// <summary>
        /// Process mouse motion event and update grid if needed
        /// </summary>
        /// <param name="p">The current position of the mouse</param>
        public void OnMouseMoved(Point p) {
            // Quit if the grid can't be modified now
            if (!CanModify()) return;

            // Update the node indices the mouse is current on
            mouseXIdx = (int) (p.X / Node.Nodesize);
            mouseYIdx = (int) (p.Y / Node.Nodesize);

            //Dragging
            if (Mouse.LeftButton == MouseButtonState.Pressed) {
                switch (dropType) {
                    // Special start/end cases
                    case NodeState.Start:
                        Grid.SetStart(mouseXIdx, mouseYIdx);
                        break;
                    case NodeState.End:
                        Grid.SetEnd(mouseXIdx, mouseYIdx);
                        break;
                    // Paint the state where the mouse is
                    default:
                        PaintState();
                        break;
                }
            }
        }

        /// <summary>
        /// Process initial setting on mouse left click
        /// </summary>
        public void OnLeftMouseDown() {
            // Quit if the grid can't be modified or the mouse is off the grid
            if (!CanModify() || !Util.IsValid(mouseXIdx, mouseYIdx, Grid)) return;

            // Update the playback state
            playState = PlaybackState.Modified;

            // Set the dropType depending on what was clicked on
            switch (Grid[mouseYIdx, mouseXIdx].State) {
                // Place a wall over open nodes
                case NodeState.Open:
                case NodeState.Closed:
                case NodeState.Empty:
                    dropType = NodeState.Wall;
                    break;
                // Clear walls
                case NodeState.Wall:
                    dropType = NodeState.Empty;
                    break;
                // Other states don't change anything
                default:
                    dropType = Grid[mouseYIdx, mouseXIdx].State;
                    break;
            }

            // Update the state of the current node immediately
            PaintState();
        }
    }

    /// <summary>
    /// Converter for enabling radio buttons depending on an enum variable's value
    /// </summary>
    internal class RadioIsCheckedConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return parameter.Equals(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return (bool) value ? parameter : Binding.DoNothing;
        }
    }

    /// <summary>
    /// Converter for disabling the heuristics groupbox depending on the selected algorithm
    /// </summary>
    internal class HeuristicsEnabledConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            AlgoFunc val = (AlgoFunc) value;
            return val == Algorithm.AStar || val == Algorithm.BestFirst ||
                   val == Algorithm.HillClimbing || val == Algorithm.JumpPoint;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
