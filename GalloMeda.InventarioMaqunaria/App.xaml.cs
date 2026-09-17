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
        public static IServiceProvider ServiceProvider { get; private set; } = null!;
        public static ISessionService Session { get; private set; } = new SessionService();

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Suscribimos los eventos de AutoUpdater para control y depuración
            AutoUpdater.ApplicationExitEvent += AutoUpdater_ApplicationExitEvent;
            AutoUpdater.CheckForUpdateEvent += AutoUpdater_CheckForUpdateEvent;
            AutoUpdater.HttpUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64)";

            // URL apuntando correctamente a la rama master
            string updateUrl = $"https://raw.githubusercontent.com/derekbibiano03/Inventario/master/update.xml?t={DateTime.UtcNow.Ticks}";
            AutoUpdater.Start(updateUrl);

            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                Exception ex = (Exception)args.ExceptionObject;
                MessageBox.Show($"Error no controlado en la aplicación:\n\n{ex.Message}\n\n{ex.InnerException?.Message}", "Error Crítico", MessageBoxButton.OK, MessageBoxImage.Error);
            };

            try
            {
                // Creamos el lector de configuración apuntando a la ruta del ejecutable.
                var builder = new ConfigurationBuilder()
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

                IConfiguration configuration = builder.Build();
                var serviceCollection = new ServiceCollection();
                var connectionString = configuration.GetConnectionString("InventarioConnection");

                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new InvalidOperationException("No se encontró la cadena de conexión 'InventarioConnection' en el archivo appsettings.json.");
                }

                serviceCollection.AddDbContext<InventarioContext>(options =>
                    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 30)))
                );

                serviceCollection.AddScoped<LogsService>();
                serviceCollection.AddScoped<AutenticacionService>();

                ServiceProvider = serviceCollection.BuildServiceProvider();

                using (var scope = ServiceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<InventarioContext>();
                    var logsService = new LogsService(context);
                    var authService = new AutenticacionService(context, logsService);
                    var loginVM = new LoginViewModel(authService, logsService);
                    var loginWindow = new Auth();
                    loginWindow.DataContext = loginVM;

                    this.ShutdownMode = ShutdownMode.OnExplicitShutdown;
                    bool? result = loginWindow.ShowDialog();

                    if (result == true && loginVM.IsAutenticado)
                    {
                        string usuarioConfirmado = App.Session.Username;
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
            catch (Exception ex)
            {
                MessageBox.Show($"Error al iniciar la aplicación:\n\n{ex.Message}\n\nDetalle: {ex.InnerException?.Message}", "Error de Inicialización", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Shutdown();
            }
        }

        // Método que intercepta el resultado de la revisión del archivo XML en el servidor
        private void AutoUpdater_CheckForUpdateEvent(UpdateInfoEventArgs args)
        {
            if (args.Error == null)
            {
                // La conexión fue exitosa y leyó el XML correctamente
                if (args.IsUpdateAvailable)
                {
                    // Si entra aquí, significa que detectó una versión mayor en el servidor con éxito
                    // (AutoUpdater lanzará su propia ventana automáticamente, esto es solo informativo)
                    System.Diagnostics.Debug.WriteLine($"Actualización encontrada: Versión {args.CurrentVersion}");
                }
                else
                {
                    MessageBox.Show("AutoUpdater se conectó con éxito, pero tu versión local es igual o superior a la del servidor.", "Verificación de Update", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                // Si hay un error (ej. URL mal escrita, sin internet, error 404, mal formato XML) lo verás aquí
                MessageBox.Show($"AutoUpdater no pudo leer el archivo XML:\n\n{args.Error.Message}", "Error de AutoUpdater", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void AutoUpdater_ApplicationExitEvent()
        {
            Application.Current.Shutdown();
        }
    }
}