using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace PathFinder {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow {
        private MainViewModel vm;
        private bool mouseDown = false;
        Timer resizeTimer = new Timer(100) { Enabled = false };

        public MainWindow() {
            InitializeComponent();
            vm = new MainViewModel();
            DataContext = vm;

            SizeChanged += OnResize;

            resizeTimer.Elapsed += ResizingDone;
        }

        private void MDownListener(object sender, RoutedEventArgs e) {
            Console.WriteLine("DOWN");
        }
        private void MUpListener(object sender, RoutedEventArgs e) {
            Console.WriteLine("UP");
        }

        void ResizingDone(object sender, ElapsedEventArgs e) {
            resizeTimer.Stop();

            Dispatcher.Invoke(() => {
                ItemsControl c = (ItemsControl) FindName("ItemsCanvas");
                vm.Grid.ResizeGrid((int) c.ActualWidth, (int) c.ActualHeight);
            });
        }

        private void OnResize(object sender, RoutedEventArgs e) {
            resizeTimer.Stop();
            resizeTimer.Start();
        }
    }
}
