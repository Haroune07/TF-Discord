using System;
using System.Windows.Input;

namespace Frontend.Commands
{
    internal class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object? parameter)
        {
            return _canExecute();
        }

        public void Execute(object? parameter)
        {
            _execute();
        }

        public RelayCommand(Action execute, Func<bool> canExecute)
        {
            _canExecute = canExecute;
            _execute = execute;
        }
    }
}