using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PathFinder
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow {
		public MainWindow()
		{
			InitializeComponent();
			DataContext = new MainViewModel();
		}

		private void Button_Click(object sender, RoutedEventArgs e) {
			var vm = (MainViewModel) DataContext;
			MessageBox.Show(vm.Algorithm.ToString());
			MessageBox.Show(vm.Heuristic.ToString());
		}
	}
}
