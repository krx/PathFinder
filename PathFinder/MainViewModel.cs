using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace PathFinder {
    class MainViewModel : INotifyPropertyChanged {
        public Grid Grid { get; }

        private AlgorithmType _algorithm;
        public AlgorithmType Algorithm { get { return _algorithm; } set { _algorithm = value; OnPropertyChanged("Algorithm"); } }

        private HeuristicType _heuristic;
        public HeuristicType Heuristic { get { return _heuristic; } set { _heuristic = value; OnPropertyChanged("Heuristic"); } }

        public int NodeSize = 10;

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
            return (int) value * (int) Node.Nodesize - (int) value;
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
