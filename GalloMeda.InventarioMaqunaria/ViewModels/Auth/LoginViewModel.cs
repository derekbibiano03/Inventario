using GalloMeda.InventarioMaqunaria;
using Inventario.Core.Services.Auth;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls; // IMPORTANTE: Necesario para usar PasswordBox
using System.Windows.Input;

namespace Inventario.Desktop.ViewModels.Auth
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private readonly AutenticacionService _autenticacionService;

        private string _usuario = string.Empty;
        public string Usuario
        {
            get => _usuario;
            set { _usuario = value; OnPropertyChanged(); }
        }

        // Dejamos la propiedad interna pero ya no requiere OnPropertyChanged puesto que no se bindea directo al texto
        public string Contrasena { get; set; }

        public ICommand LoginCommand { get; }

        public bool IsAutenticado { get; private set; } = false;

        public LoginViewModel(AutenticacionService autenticacionService)
        {
            _autenticacionService = autenticacionService;
            // Modificado para aceptar el parámetro enviado desde la vista
            LoginCommand = new RelayCommand(EjecutarLogin); // Sin la lambda 'p => ...'
        }

        private void EjecutarLogin()
        {
            try
            {
                foreach (Window window in Application.Current.Windows)
                {
                    if (window.DataContext == this && window is Views.Auth authWindow)
                    {
                        // Accedemos directamente al PasswordBox por su x:Name gracias a la instancia de la ventana
                        this.Contrasena = authWindow.TxtContrasena.Password;
                        break;
                    }
                }
                if (string.IsNullOrWhiteSpace(this.Usuario))
                {
                    throw new Exception("El nombre de usuario es obligatorio.");
                }

                if (string.IsNullOrWhiteSpace(this.Contrasena))
                {
                    throw new Exception("La contraseña es obligatoria.");
                }

                if (_autenticacionService.ValidarCredenciales(this.Usuario.Trim(), this.Contrasena))
                {
                    this.IsAutenticado = true;

                    App.Session.Username = this.Usuario.Trim();
                    App.Session.IdRol = 1;

                    foreach (Window window in Application.Current.Windows)
                    {
                        if (window.DataContext == this)
                        {
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