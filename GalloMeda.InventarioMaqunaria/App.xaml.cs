using Inventario.Core.Services.Auth;
using Inventario.Data.Models;
using Inventario.Desktop.ViewModels.Auth;
using Inventario.Desktop.Views;
using System.Configuration;
using System.Data;
using System.Windows;

namespace GalloMeda.InventarioMaqunaria
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var context = new InventarioContext();
            var authService = new AutenticacionService(context);
            var loginVM = new LoginViewModel(authService);

            var loginWindow = new Auth();
            loginWindow.DataContext = loginVM;

            // CORRECCIÓN CRÍTICA: Le decimos a WPF que NO apague el programa de forma automática
            // cuando se cierre la ventana de Login. Nosotros controlaremos el apagado manualmente.
            this.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            // Se detiene aquí esperando el inicio de sesión
            bool? result = loginWindow.ShowDialog();

            if (result == true && loginVM.IsAutenticado)
            {
                // Instancia y muestra tu ventana principal
                var mainWindow = new MainWindow();
                this.MainWindow = mainWindow;

                // CORRECCIÓN CRÍTICA: Ahora que la ventana principal está lista, restauramos el comportamiento
                // por defecto para que, si el usuario cierra el MainWindow, ahora sí se apague todo el programa.
                this.ShutdownMode = ShutdownMode.OnMainWindowClose;

                mainWindow.Show();
            }
            else
            {
                // Si el usuario canceló el Login o cerró la ventana con la "X", apagamos explícitamente.
                this.Shutdown();
            }
        }
    }

}
