using Inventario.Core.Services;
using Inventario.Core.Services.Auth;
using Inventario.Data.Models;
using Inventario.Desktop.ViewModels.Auth;
using Inventario.Desktop.Views;
using InventarioMaquinaria.Services;
using System.Configuration;
using System.Data;
using System.Windows;

namespace GalloMeda.InventarioMaqunaria
{
    public partial class App : Application
    {
        public static ISessionService Session { get; private set; } = new SessionService();

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var context = new InventarioContext();
            var authService = new AutenticacionService(context);
            var loginVM = new LoginViewModel(authService);

            var loginWindow = new Auth();
            loginWindow.DataContext = loginVM;

            this.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            bool? result = loginWindow.ShowDialog();

            if (result == true && loginVM.IsAutenticado)
            {
                // CORRECCIÓN EXPLICITA: Forzamos la lectura directa de la sesión global actualizada
                string usuarioConfirmado = App.Session.Username;

                // Enviamos el string resuelto al constructor del MainWindow
                var mainWindow = new MainWindow(usuarioConfirmado);
                this.MainWindow = mainWindow;

                this.ShutdownMode = ShutdownMode.OnMainWindowClose;
                mainWindow.Show();
            }
            else
            {
                this.Shutdown();
            }
        }
    }
}