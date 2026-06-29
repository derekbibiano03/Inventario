using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Inventario.Core.Services.Auth;

namespace Inventario.Desktop.ViewModels.Auth
{
    // REGLA: Debe implementar INotifyPropertyChanged explícitamente para sincronizarse con la vista
    public class LoginViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private readonly AutenticacionService _autenticacionService;

        private string _usuario;
        public string Usuario
        {
            get => _usuario;
            set { _usuario = value; OnPropertyChanged(); }
        }

        private string _contrasena;
        public string Contrasena
        {
            get => _contrasena;
            set { _contrasena = value; OnPropertyChanged(); }
        }

        public ICommand LoginCommand { get; }

        // Indica si el inicio de sesión fue exitoso
        public bool IsAutenticado { get; private set; } = false;

        public LoginViewModel(AutenticacionService autenticacionService)
        {
            _autenticacionService = autenticacionService;
            LoginCommand = new RelayCommand(EjecutarLogin);
        }

        private void EjecutarLogin()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(this.Usuario))
                {
                    throw new Exception("El nombre de usuario es obligatorio.");
                }

                if (string.IsNullOrWhiteSpace(this.Contrasena))
                {
                    throw new Exception("La contraseña es obligatoria.");
                }

                // Validación con BCrypt contra PostgreSQL
                if (_autenticacionService.ValidarCredenciales(this.Usuario.Trim(), this.Contrasena))
                {
                    this.IsAutenticado = true;

                    // Buscamos la ventana actual para cerrarla pasando el resultado correcto
                    foreach (Window window in Application.Current.Windows)
                    {
                        if (window.DataContext == this)
                        {
                            // CORRECCIÓN CRÍTICA: Asignar DialogResult = true AUTOMÁTICAMENTE cierra la ventana 
                            // y le avisa a App.xaml.cs que el inicio de sesión fue totalmente exitoso.
                            window.DialogResult = true;
                            break;
                        }
                    }
                }
                else
                {
                    throw new Exception("Usuario o contraseña incorrectos. Intenta de nuevo.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error de Autenticación", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}