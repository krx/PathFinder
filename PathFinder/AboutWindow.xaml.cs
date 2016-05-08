using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Navigation;

namespace PathFinder {
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow {

        private Assembly app = Assembly.GetExecutingAssembly();

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
            Process.Start(e.Uri.AbsoluteUri);
            e.Handled = true;
        }
    }
}
