using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace PathFinder {
    enum NodeState {
        Empty, Wall, Open, Closed, Start, End
    }

    class Node : INotifyPropertyChanged, ICloneable, IComparable {
        public static readonly double Nodesize = 30;

        private static Dictionary<NodeState, Color> cmap = new Dictionary<NodeState, Color>() {
            {NodeState.Empty, Colors.White },
            {NodeState.Wall, Colors.Gray },
            {NodeState.Open, Colors.Orange },
            {NodeState.Closed, Colors.Cyan },
            {NodeState.Start, Color.FromRgb(0, 255, 0) },
            {NodeState.End, Colors.Red }
        };

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
            set {
                NodeState prev = _state;
                _state = value;
                OnPropertyChanged("State");
                UpdateColor(prev);
            }
        }

        public Node(int x, int y, NodeState state = NodeState.Empty) {
            X = x;
            Y = y;
            State = state;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void UpdateColor(NodeState old) {
            Application.Current.Dispatcher.Invoke(() => {
                ItemsControl ctl = (ItemsControl) Application.Current.MainWindow.FindName("ItemsCanvas");
                if (ctl.Items.Count > 0) {
                    Rectangle rect = Util.FindDescendant<Rectangle>(ctl.ItemContainerGenerator.ContainerFromItem(this));
                    if (rect != null) {
                        ScaleTransform scale = (ScaleTransform) rect.FindName("rectScale");
                        if (old != NodeState.Start && old != NodeState.End && State != NodeState.Start && State != NodeState.End) {

                            // Scaling animation for walls
                            if (State == NodeState.Wall || old == NodeState.Wall) {
                                Console.WriteLine(scale?.ToString() ?? "NULL");
                                DoubleAnimation scaleAnim = new DoubleAnimation(1, 1.2, TimeSpan.FromMilliseconds(75));
                                scaleAnim.AutoReverse = true;
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
                            rect.Fill = new SolidColorBrush(cmap[State]);
                        }
                    }
                }
            });
        }

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
