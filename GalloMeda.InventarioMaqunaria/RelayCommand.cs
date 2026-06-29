using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Inventario.Desktop
{
    // Clase genérica para gestionar comandos con parámetros en MVVM.
    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Predicate<T> _canExecute;

        public RelayCommand(Action<T> execute, Predicate<T> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        // Determina si el comando se puede ejecutar en el estado actual.
        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute((T)parameter);
        }

        // Ejecuta la acción principal pasando el parámetro convertido a su tipo real.
        public void Execute(object parameter)
        {
            _execute((T)parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
