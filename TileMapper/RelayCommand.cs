using System;
using System.Windows.Input;

namespace TileMapper
{
    public class RelayCommand : ICommand
    {
        readonly Action _execute;
        readonly Predicate<object> _canExecute;

        public RelayCommand(Action execute)
            : this(execute, null)
        {

        }
        public RelayCommand(Action execute, Predicate<object> canExecute)
        {
            if (execute == null) throw new ArgumentNullException("execute");
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        public void Execute(object parameter)
        {
            _execute();
        }
    }
}