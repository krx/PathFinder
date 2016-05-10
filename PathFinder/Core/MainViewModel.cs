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
        // Backing variables
        private volatile int _animSleepTime;
        private AlgoFunc _algorithm;
        private HeuristicFunc _heuristic;
        private bool _diagonalsAllowed;
        private bool _cornerCutAllowed;
        private string _statPathLength;
        private string _statTime;
        private string _statNodesExplored;

        // Current node indices the mouse is hovering over
        private int mouseXIdx;
        private int mouseYIdx;

        // The state that will be painted as the mouse is dragged
        private NodeState dropType = NodeState.Empty;

        // Search results
        private List<Node> path;
        private double pathLength;
        private double algoTime;

        // Search playback
        private History hist;
        private volatile PlaybackState playState = PlaybackState.Modified;
        private Thread playThread;

        /// <summary>
        /// Grid containing all nodes
        /// </summary>
        public Grid Grid { get; }

        /// <summary>
        /// Number of milliseconds between every step in the playback animation
        /// </summary>
        public int AnimSleepTime {
            get { return _animSleepTime; }
            set { _animSleepTime = value; OnPropertyChanged("AnimSleepTime"); }
        }

        /// <summary>
        /// The algoritm that will be called to find the path
        /// </summary>
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
        public bool DiagonalsAllowed {
            get { return _diagonalsAllowed; }
            set { _diagonalsAllowed = value; OnPropertyChanged("DiagonalsAllowed"); }
        }

        /// <summary>
        /// Wether the diagonals in the path can cross the corner of a wall
        /// </summary>
        public bool CornerCutAllowed {
            get { return _cornerCutAllowed; }
            set { _cornerCutAllowed = value; OnPropertyChanged("CornerCutAllowed"); }
        }

        /// <summary>
        /// Result string for the path length
        /// </summary>
        public string StatPathLength {
            get { return _statPathLength; }
            set { _statPathLength = value; OnPropertyChanged("StatPathLength"); }
        }

        /// <summary>
        /// Result string for the search time
        /// </summary>
        public string StatTime {
            get { return _statTime; }
            set { _statTime = value; OnPropertyChanged("StatTime"); }
        }

        /// <summary>
        /// Result string for the number of nodes explored in the search
        /// </summary>
        public string StatNodesExplored {
            get { return _statNodesExplored; }
            set { _statNodesExplored = value; OnPropertyChanged("StatNodesExplored"); }
        }

        // Button Commands
        public ICommand PlayCommand { get; }
        public ICommand PauseCommand { get; }
        public ICommand StopCommand { get; }
        public ICommand StepBackwardCommand { get; }
        public ICommand StepForwardCommand { get; }
        public ICommand ClearAllCommand { get; }
        public ICommand ClearPathCommand { get; }
        public ICommand GenMazeCommand { get; }

        /// <summary>
        /// Create and initializes this ViewModel
        /// </summary>
        public MainViewModel() {
            Grid = new Grid();
            hist = new History(Grid);

            // Initial options
            Algo = Algorithm.AStar;
            HeuristicFunction = Heuristic.Manhattan;
            DiagonalsAllowed = true;
            CornerCutAllowed = true;
            AnimSleepTime = 4;

            // Playback commands
            PlayCommand = new RelayCommand(
                o => PlaySearch(),
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

            StepForwardCommand = new RelayCommand(
                o => StepForward(),
                o => playState != PlaybackState.GeneratingMaze);

            StepBackwardCommand = new RelayCommand(
                o => StepBackward(),
                o => playState != PlaybackState.GeneratingMaze);

            // Grid control commands
            ClearAllCommand = new RelayCommand(
                o => {
                    playState = PlaybackState.Modified;
                    hist.Clear(false);
                    Grid.ClearAll();
                },
                o => CanModify());

            ClearPathCommand = new RelayCommand(
                o => {
                    playState = PlaybackState.Modified;
                    hist.Clear();
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
        /// Updates the string properties for all result stats
        /// </summary>
        public void UpdateStats() {
            // Update stat strings
            StatPathLength = $"{pathLength:f2}";
            StatTime = $"{algoTime:f4}ms";
            StatNodesExplored = $"{Grid.Nodes.Count(n => n.IsOpen || n.IsClosed) + 2}"; // +2 for start and end
        }

        /// <summary>
        /// Recalculates the path if it has not already been found
        /// </summary>
        public void CalculatePath() {
            // Don't recalculate if the animation is currently paused
            if (playState == PlaybackState.Paused) return;

            // Clear old path
            hist.Clear();

            // Time how long the search takes
            Stopwatch sw = new Stopwatch();
            sw.Start();
            path = Algo(Grid.StartNode, Grid.EndNode, Grid, HeuristicFunction, DiagonalsAllowed, CornerCutAllowed, hist);
            sw.Stop();

            // Record search time
            algoTime = sw.Elapsed.TotalMilliseconds;

            // Calculate path length
            pathLength = 0;
            for (int i = 0; i < path.Count - 1; ++i) {
                pathLength += Heuristic.Euclidean(path[i], path[i + 1]);
            }
        }

        /// <summary>
        /// Plays the search animation for a path between the start and end nodes
        /// </summary>
        public void PlaySearch() {
            // Recalculate if needed
            CalculatePath();

            // Update the playback state and kill any old threads
            playState = PlaybackState.Playing;
            playThread?.Abort();

            // Start the search playback thread
            playThread = new Thread(() => {
                // Keep stepping through history until the end is reached
                while (hist.Step()) {
                    Thread.Sleep(AnimSleepTime);

                    // If at any point the playback state changes, exit the thread
                    if (playState != PlaybackState.Playing) {
                        return;
                    }
                }

                // Update statistics
                UpdateStats();

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
        /// Take one step forward in the animation
        /// If the end is reached it will loop again (recalculating the path)
        /// </summary>
        public void StepForward() {
            // Stop the animation if it is currently running
            if (playState == PlaybackState.Playing) {
                playState = PlaybackState.Paused;
            }

            // Recalculate if needed
            CalculatePath();

            // If the end of the animation has been reached, draw the path and update display
            if (hist.AtEnd) {
                // Update display
                UpdateStats();
                Grid.GenPath(path);

                // Update playback state
                playState = PlaybackState.SearchComplete;

                // Force commands to update
                CommandManager.InvalidateRequerySuggested();
            } else {
                // Step once
                hist.Step();
                playState = PlaybackState.Paused;
            }
        }

        /// <summary>
        /// Take one step backwards in the animation, stops when the beginning is reached
        /// </summary>
        public void StepBackward() {
            if (hist.AtEnd) {
                // Clear the visual path
                Grid.GenPath(new List<Node>());
            }

            // Step back once
            hist.StepBack();
            playState = PlaybackState.Paused;

            // When the beginning is hit, return the grid to a usable state
            if (hist.AtBeginning) {
                playState = PlaybackState.Modified;
            }
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
                    Thread.Sleep(AnimSleepTime);
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

            // Dragging
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
