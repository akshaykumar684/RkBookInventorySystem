using System;
using System.Windows.Input;

namespace BookInventorySystem
{
    class ApplicationCommand : ICommand
    {
        private readonly Action _executeMethod;
        private readonly Func<bool> _canExecuteMethod;

        public ApplicationCommand(Action executeMethod)
        {
            _executeMethod = executeMethod;
            _canExecuteMethod = () => true;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecuteMethod != null && _canExecuteMethod();
        }

        public void Execute(object parameter)
        {
            if (_executeMethod != null)
            {
                _executeMethod();
            }
        }

        public event EventHandler CanExecuteChanged;
    }

    class ApplicationCommand<T> : ICommand
    {
        private readonly Action<object> _executeMethod;
        private readonly Func<bool> _canExecuteMethod;

        public ApplicationCommand(Action<object> executeMethod)
        {
            _executeMethod = executeMethod;
            _canExecuteMethod = () => true;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecuteMethod != null && _canExecuteMethod();
        }

        public void Execute(object parameter)
        {
            if (_executeMethod != null)
            {
                _executeMethod(parameter);
            }
        }

        public event EventHandler CanExecuteChanged;
    }
}
