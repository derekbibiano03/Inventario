using Inventario.Core.Services;
using Inventario.Core.Services.Auth;
using Inventario.Core.Services.Logs; // <--- IMPORTANTE: Agrega este using para usar LogsService
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
            // 1. Instanciamos el DbContext único que usará toda la app durante el arranque.
            var context = new InventarioContext();

            // 2. Instanciamos el servicio de autenticación pasándole el contexto.
            var authService = new AutenticacionService(context);

            // 3. Instanciamos el servicio de logs pasándole también el contexto de la base de datos.
            var logsService = new LogsService(context); // <--- LÍNEA NUEVA

            // 4. Instanciamos el ViewModel pasándole AMBOS servicios requeridos por su constructor.
            var loginVM = new LoginViewModel(authService, logsService); // <--- CORREGIDO: Ahora recibe logsService

            // 5. Creamos la ventana de Login y asignamos su DataContext.
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