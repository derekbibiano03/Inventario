using AutoUpdaterDotNET;
using Inventario.Core.Services;
using Inventario.Core.Services.Auth;
using Inventario.Core.Services.Logs;
using Inventario.Data.Models;
using Inventario.Desktop.ViewModels.Auth;
using Inventario.Desktop.Views;
using InventarioMaquinaria.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Windows;

namespace GalloMeda.InventarioMaqunaria
{
    // Define la clase principal de la aplicación WPF heredando de Application.
    public partial class App : Application
    {
        // Propiedad estática para almacenar el proveedor de servicios de inyección de dependencias.
        public static IServiceProvider ServiceProvider { get; private set; } = null!;

        // Propiedad estática para mantener la sesión del usuario activo en la aplicación.
        public static ISessionService Session { get; private set; } = new SessionService();

        // Sobrescribimos el método OnStartup que es el punto de entrada real de la aplicación WPF.
        protected override void OnStartup(StartupEventArgs e)
        {
            // Ejecutamos la lógica base de inicialización del marco WPF.
            base.OnStartup(e);
            AutoUpdater.Start("https://raw.githubusercontent.com/derekbibiano03/Inventario/main/update.xml");

            // Capturamos cualquier excepción no controlada en el hilo principal para mostrar un mensaje claro.
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                // Extraemos el objeto de excepción arrojado por el sistema.
                Exception ex = (Exception)args.ExceptionObject;
                // Desplegamos un mensaje con la causa exacta del fallo.
                MessageBox.Show($"Error no controlado en la aplicación:\n\n{ex.Message}\n\n{ex.InnerException?.Message}", "Error Crítico", MessageBoxButton.OK, MessageBoxImage.Error);
            };

            try
            {
                // Creamos el lector de configuración apuntando a la ruta del ejecutable.
                var builder = new ConfigurationBuilder()
                    // Establecemos el directorio base donde se ejecuta el programa.
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                    // Indicamos que lea el archivo appsettings.json de forma obligatoria.
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

                // Construimos la configuración para acceder a las claves e incrustaciones.
                IConfiguration configuration = builder.Build();

                // Inicializamos la colección de servicios del contenedor IoC.
                var serviceCollection = new ServiceCollection();

                // Extraemos la cadena de conexión especificada en el archivo appsettings.json.
                var connectionString = configuration.GetConnectionString("InventarioConnection");

                // Verificamos que la cadena de conexión exista y no esté vacía.
                if (string.IsNullOrEmpty(connectionString))
                {
                    // Disparamos un error informativo en caso de que la clave no esté presente.
                    throw new InvalidOperationException("No se encontró la cadena de conexión 'InventarioConnection' en el archivo appsettings.json.");
                }

                // Registramos el contexto de datos de EF Core usando una versión fija de MariaDB/MySQL sin autodetección de red.
                serviceCollection.AddDbContext<InventarioContext>(options =>
                    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 30)))
                );

                // Registramos las clases concretas directamente en el contenedor de dependencias sin interfaz.
                serviceCollection.AddScoped<LogsService>();
                serviceCollection.AddScoped<AutenticacionService>();

                // Compilamos la fábrica del proveedor de servicios.
                ServiceProvider = serviceCollection.BuildServiceProvider();

                // Creamos un alcance de ejecución para resolver las instancias necesarias en el Login.
                using (var scope = ServiceProvider.CreateScope())
                {
                    // Resolvemos el DbContext configurado desde el contenedor de dependencias.
                    var context = scope.ServiceProvider.GetRequiredService<InventarioContext>();

                    // Creamos el servicio de logs inyectándole el contexto válido.
                    var logsService = new LogsService(context);

                    // Creamos el servicio de autenticación inyectándole el contexto y el servicio de logs.
                    var authService = new AutenticacionService(context, logsService);

                    // Creamos el ViewModel asociándolo a sus dependencias inicializadas.
                    var loginVM = new LoginViewModel(authService, logsService);

                    // Instanciamos la vista del Login.
                    var loginWindow = new Auth();

                    // Asignamos el DataContext a la vista.
                    loginWindow.DataContext = loginVM;

                    // Configuramos el modo de cierre para impedir la salida prematura al ocultar la ventana.
                    this.ShutdownMode = ShutdownMode.OnExplicitShutdown;

                    // Mostramos la ventana de Login en modo modal.
                    bool? result = loginWindow.ShowDialog();

                    // Validamos si la autenticación fue exitosa.
                    if (result == true && loginVM.IsAutenticado)
                    {
                        // Leemos la propiedad del usuario autenticado en la sesión global.
                        string usuarioConfirmado = App.Session.Username;

                        // Instanciamos la ventana principal enviando el usuario.
                        var mainWindow = new MainWindow(usuarioConfirmado);

                        // Asignamos la ventana principal a la propiedad global de WPF.
                        this.MainWindow = mainWindow;

                        // Establecemos que al cerrar la ventana principal finalice toda la aplicación.
                        this.ShutdownMode = ShutdownMode.OnMainWindowClose;

                        // Desplegamos la ventana principal.
                        mainWindow.Show();
                    }
                    else
                    {
                        // Finalizamos el proceso si el login falla o es cancelado.
                        this.Shutdown();
                    }
                }
            }
            catch (Exception ex)
            {
                // Desplegamos la ventana con la excepción capturada durante la inicialización.
                MessageBox.Show($"Error al iniciar la aplicación:\n\n{ex.Message}\n\nDetalle: {ex.InnerException?.Message}", "Error de Inicialización", MessageBoxButton.OK, MessageBoxImage.Error);
                // Cerramos la aplicación tras notificar el error.
                this.Shutdown();
            }
        }
    }
}