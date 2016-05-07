using System.ComponentModel;

namespace PathFinder {

    /// <summary>
    /// Simple template for any class that needs to have INotifyPropertyChanged
    /// </summary>
    class NotifyPropertyChangedBase : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
