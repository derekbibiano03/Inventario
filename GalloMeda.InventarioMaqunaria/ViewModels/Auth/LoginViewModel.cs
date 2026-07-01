using GalloMeda.InventarioMaqunaria;
using Inventario.Core.Services.Auth;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace Inventario.Desktop.ViewModels.Auth
{
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

                if (_autenticacionService.ValidarCredenciales(this.Usuario.Trim(), this.Contrasena))
                {
                    this.IsAutenticado = true;

                    // CORRECCIÓN DIRECTA: Escribimos directo en la sesión global sin usar métodos estáticos intermedios
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