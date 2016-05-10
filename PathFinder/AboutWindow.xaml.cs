using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Navigation;

namespace PathFinder {
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow {
        // Load this program's assembly informaiton
        private Assembly app = Assembly.GetExecutingAssembly();

        // Load information strings as properties
        public string Version => app.GetName().Version.ToString();
        public string Copyright => ((AssemblyCopyrightAttribute) app.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false)[0]).Copyright;

        public AboutWindow() {
            InitializeComponent();
            DataContext = this;
        }

        public AboutWindow(Window parent) : this() {
            Owner = parent;
        }

        private void Close_Click(object sender, RoutedEventArgs e) {
            Close();
        }

        private void Hyperlink_OnRequestNavigate(object sender, RequestNavigateEventArgs e) {
            // Handle a URL click by opening it in a browser
            Process.Start(e.Uri.AbsoluteUri);
            e.Handled = true;
        }
    }
}
