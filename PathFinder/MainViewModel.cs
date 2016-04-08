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

namespace PathFinder
{
	class MainViewModel : INotifyPropertyChanged {
		private ObservableCollection<Node> _nodes;
		private AlgorithmType _algorithm;
		private HeuristicType _heuristic;

		public ObservableCollection<Node> Nodes { get { return _nodes ?? (_nodes = new ObservableCollection<Node>()); } }

		public AlgorithmType Algorithm { get { return _algorithm; } set { _algorithm = value; OnPropertyChanged("Algorithm"); } }

		public HeuristicType Heuristic { get { return _heuristic; } set { _heuristic = value; OnPropertyChanged("Heuristic"); } }

		public MainViewModel() {

		}

		public event PropertyChangedEventHandler PropertyChanged;


		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
		}


	}

	public class RadioIsCheckedConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return parameter.Equals(value);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (bool)value ? parameter : Binding.DoNothing;
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
