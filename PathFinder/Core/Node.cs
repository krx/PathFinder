using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace PathFinder.Core {

    /// <summary>
    /// Represents the state/type of a node
    /// </summary>
    internal enum NodeState {
        Empty, Wall, Open, Closed, Start, End
    }

    /// <summary>
    /// An element/cell of the grid
    /// </summary>
    internal class Node : NotifyPropertyChangedBase, ICloneable, IComparable {

        // Global visual size of a node
        public static readonly double Nodesize = 30;

        // Map of states to colors
        private static Dictionary<NodeState, Color> cmap = new Dictionary<NodeState, Color>() {
            {NodeState.Empty, Colors.White },
            {NodeState.Wall, Colors.Gray },
            {NodeState.Open, Colors.Orange },
            {NodeState.Closed, Colors.Cyan },
            {NodeState.Start, Color.FromRgb(0, 255, 0) },
            {NodeState.End, Colors.Red }
        };

        // A command triggered when the node is visually loaded to update its color
        public ICommand UpdateColorCommand { get; }

        // The x/y indices of this node, and converters to screen coordinates
        // Index in the grid
        public int X { get; }
        public int Y { get; }

        // Top left corner coordinates
        public int CoordX => (int) (X * Nodesize);
        public int CoordY => (int) (Y * Nodesize);

        // Center coordinates
        public int CenterX => (int) (CoordX + Nodesize / 2.0);
        public int CenterY => (int) (CoordY + Nodesize / 2.0);

        // Scoring values of this node
        public double GScore { get; set; }
        public double HScore { get; set; }
        public double FScore => GScore + HScore;

        // The parent of this node used for bracktracing
        public Node Parent { get; set; }

        // Property to check if this node can be walked on
        public bool IsWalkable => State != NodeState.Wall;

        /// <summary>
        /// The current state of this node
        /// </summary>
        private NodeState _state;
        public NodeState State {
            get { return _state; }
            set {
                // Temporarily store the previous state
                NodeState prev = _state;
                _state = value;
                OnPropertyChanged("State");

                // Update this node's color to the new state
                UpdateColor(prev);
            }
        }


        /// <summary>
        /// Create a new Node
        /// </summary>
        /// <param name="x">X index of this node in the grid</param>
        /// <param name="y">Y index of this node in the grid</param>
        /// <param name="state">Initial state of the node</param>
        public Node(int x, int y, NodeState state = NodeState.Empty) {
            X = x;
            Y = y;
            State = state;
            UpdateColorCommand = new RelayCommand(o => UpdateColor(State, false));
        }

        /// <summary>
        /// Updates the color of this node's view, with optional animations
        /// </summary>
        /// <param name="old">Previous state of this node</param>
        /// <param name="animate">If animations should be used</param>
        private void UpdateColor(NodeState old, bool animate = true) {
            // Make sure this runs on the UI thread
            Application.Current.Dispatcher.Invoke(() => {
                // Attempt to find the ItemsControl that contains this node
                ItemsControl ctl = (ItemsControl) Application.Current.MainWindow.FindName("ItemsCanvas");
                if (ctl == null || ctl.Items.Count == 0) return;

                // Once the control is found, attempt to find the actual view that this node represents
                Rectangle rect = Util.FindDescendant<Rectangle>(ctl.ItemContainerGenerator.ContainerFromItem(this));
                if (rect == null) return;

                // Get the scale transform for the rectangle
                ScaleTransform scale = (ScaleTransform) rect.FindName("rectScale");

                // Only animate state other than start/end
                if (animate && old != NodeState.Start && old != NodeState.End && State != NodeState.Start && State != NodeState.End) {

                    // Scaling animation for walls
                    if (State == NodeState.Wall || old == NodeState.Wall) {
                        DoubleAnimation scaleAnim = new DoubleAnimation {
                            From = 1,
                            To = 1.2,
                            Duration = TimeSpan.FromMilliseconds(75),
                            AutoReverse = true
                        };
                        scale.BeginAnimation(ScaleTransform.ScaleXProperty, scaleAnim);
                        scale.BeginAnimation(ScaleTransform.ScaleYProperty, scaleAnim);
                    }

                    //Color animation
                    Storyboard sb = new Storyboard();
                    ColorAnimation anim = new ColorAnimation(cmap[old], cmap[State], TimeSpan.FromMilliseconds(75));
                    Storyboard.SetTarget(anim, rect);
                    Storyboard.SetTargetProperty(anim, new PropertyPath("Fill.Color"));
                    sb.Children.Add(anim);
                    sb.Begin();
                } else {
                    // Without animations, just set the color of the rectangle
                    rect.Fill = new SolidColorBrush(cmap[State]);
                }
            });
        }

        /// <summary>
        /// Zeros out all of this node's scores
        /// </summary>
        public void ClearScores() {
            GScore = HScore = 0;
        }

        /// <returns>A copy of this node</returns>
        public object Clone() {
            return new Node(X, Y, State);
        }

        /// <summary>
        /// Compares this node to another based on the F-Scores
        /// </summary>
        /// <param name="o">Other node being compared</param>
        /// <returns>A number to show if this node is less than, greater than, or the same as the other</returns>
        public int CompareTo(object o) {
            return FScore.CompareTo(((Node) o).FScore);
        }

        /// <summary>
        /// Checks if two nodes are the same on a grid
        /// </summary>
        /// <param name="other">Other node being compared</param>
        /// <returns>If the two nodes have the same grid indices</returns>
        public override bool Equals(object other) {
            Node n = other as Node;
            if (n != null) return X == n.X && Y == n.Y;
            return base.Equals(other);
        }
    }
}
