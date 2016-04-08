using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace PathFinder {
	class Node : INotifyPropertyChanged {
		private int _x;
		public int X
		{
			get { return _x; }
			set
			{
				_x = value;
				OnPropertyChanged( "X" );
			}
		}


		private int _y;
		public int Y
		{
			get { return _y; }
			set
			{
				_y = value;
				OnPropertyChanged("Y");
			}
		}

		private SolidColorBrush _col;

		public SolidColorBrush Color
		{
			get { return _col; }
			set
			{
				_col = value;
				OnPropertyChanged("Color");
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
