using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PathFinder.Core;

namespace PathFinder {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow {
        // Store the main DataContext
        private MainViewModel vm;

        // Timer to delay resize updates
        private Timer resizeTimer = new Timer(200) { Enabled = false };

        public MainWindow() {
            InitializeComponent();

            // Set the DataContext
            vm = new MainViewModel();
            DataContext = vm;

            // Set event listeners
            SizeChanged += OnResize;
            MouseMove += (sender, args) => vm.OnMouseMoved(Mouse.GetPosition(this));
            MouseLeftButtonDown += (sender, args) => vm.OnLeftMouseDown();
            resizeTimer.Elapsed += ResizingDone;
        }

        private void ResizingDone(object sender, ElapsedEventArgs e) {
            // Quit if the grid can't be modified
            if (!vm.CanModify()) return;

            // Stop the timer
            resizeTimer.Stop();

            // On the UI thread, update the grid size to fill up the available space
            Dispatcher.Invoke(() => {
                ItemsControl c = (ItemsControl) FindName("ItemsCanvas");
                vm.Grid.ResizeGrid((int) c.ActualWidth, (int) c.ActualHeight);
            });
        }

        private void OnResize(object sender, RoutedEventArgs e) {
            // Reset the timer whenever the window event is fired
            resizeTimer.Stop();
            resizeTimer.Start();
        }
    }
}
