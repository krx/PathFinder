using System;
using System.Windows.Input;

namespace PathFinder {

    /// <summary>
    /// Convenience class for setting up commands
    /// Simplified version of the MVVM Light class
    /// </summary>
    internal class RelayCommand : ICommand {
        private readonly Action<object> execute;
        private readonly Func<object, bool> canExecute;

        public event EventHandler CanExecuteChanged {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null) {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameter) {
            return canExecute == null || canExecute(parameter);
        }

        public void Execute(object parameter) {
            execute(parameter);
        }
    }
}
