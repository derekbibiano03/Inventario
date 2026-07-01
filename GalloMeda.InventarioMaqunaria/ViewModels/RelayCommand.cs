using System;
using System.Windows.Input;

namespace Inventario.Desktop.ViewModels
{
    // Esta clase permite mapear las acciones de los botones en el XAML directamente a métodos del ViewModel.
    public class RelayCommand : ICommand
    {
        // Almacena la acción (método) que se ejecutará cuando el usuario haga clic.
        private readonly Action _execute;

        // Almacena la función que evalúa si el botón debe estar habilitado o deshabilitado.
        private readonly Func<bool> _canExecute;

        // Constructor que se ejecuta cuando solo nos interesa la acción y el botón siempre está habilitado.
        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            // Valida que la acción no sea nula para evitar excepciones en tiempo de ejecución.
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        // Evalúa si el comando se puede ejecutar en el estado actual de la aplicación.
        public bool CanExecute(object parameter)
        {
            // Si no se definió una regla, devuelve 'true' por defecto (siempre habilitado).
            return _canExecute == null || _canExecute();
        }

        // Ejecuta físicamente el método asociado al comando al presionar el botón.
        public void Execute(object parameter)
        {
            _execute();
        }

        // Evento que notifica a WPF que las condiciones de ejecución cambiaron y debe reevaluar si el botón se habilita o deshabilita.
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}