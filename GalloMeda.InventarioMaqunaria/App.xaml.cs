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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Windows;

namespace GalloMeda.InventarioMaqunaria
{
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; } = null!;
        public static ISessionService Session { get; private set; } = new SessionService();

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            AutoUpdater.ApplicationExitEvent += AutoUpdater_ApplicationExitEvent;
            AutoUpdater.Start("https://raw.githubusercontent.com/derekbibiano03/Inventario/master/update.xml");

            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                Exception ex = (Exception)args.ExceptionObject;
                MessageBox.Show($"Error no controlado en la aplicación:\n\n{ex.Message}\n\n{ex.InnerException?.Message}", "Error Crítico", MessageBoxButton.OK, MessageBoxImage.Error);
            };

            try
            {
                string basePath = AppDomain.CurrentDomain.BaseDirectory;
                string configPath = Path.Combine(basePath, "appsettings.json");
                string templatePath = Path.Combine(basePath, "appsettings.template.json");

                // 1. Verificar si existe el archivo de configuración, si no, crearlo con ambas estructuras (BD y AzureAd)
                if (!File.Exists(configPath))
                {
                    if (File.Exists(templatePath))
                    {
                        File.Copy(templatePath, configPath);
                    }
                    else
                    {
                        // CREA EL ARCHIVO VACÍO (Seguro para GitHub / Primer arranque)
                        File.WriteAllText(configPath, "{\n  \"ConnectionStrings\": {\n    \"InventarioConnection\": \"\"\n  },\n  \"AzureAd\": {\n    \"TenantId\": \"\",\n    \"ClientId\": \"\",\n    \"ClientSecret\": \"\",\n    \"Remitente\": \"\"\n  }\n}");
                    }
                }
                else
                {
                    // 2. Si ya existe, asegurarnos de que contenga el nodo AzureAd (útil al actualizar la app)
                    string jsonContent = File.ReadAllText(configPath);
                    if (!jsonContent.Contains("AzureAd"))
                    {
                        var jsonObject = JObject.Parse(jsonContent);
                        if (jsonObject["AzureAd"] == null)
                        {
                            jsonObject["AzureAd"] = new JObject(
                                new JProperty("TenantId", ""),
                                new JProperty("ClientId", ""),
                                new JProperty("ClientSecret", ""),
                                new JProperty("Remitente", "")
                            );
                            File.WriteAllText(configPath, jsonObject.ToString(Newtonsoft.Json.Formatting.Indented));
                        }
                    }
                }

                var builder = new ConfigurationBuilder()
                    .SetBasePath(basePath)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

                IConfiguration configuration = builder.Build();

                var serviceCollection = new ServiceCollection();

                var connectionString = configuration.GetConnectionString("InventarioConnection");

                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new InvalidOperationException("No se encontró la cadena de conexión 'InventarioConnection' en el archivo appsettings.json.");
                }

                serviceCollection.AddDbContext<InventarioContext>(options =>
                    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 46)))
                );

                serviceCollection.AddScoped<LogsService>();
                serviceCollection.AddScoped<AutenticacionService>();
                serviceCollection.AddSingleton(configuration);
                serviceCollection.AddScoped<EmailService>();

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
                        int idRolUsuario = App.Session.IdRol;
                        string NombreCompleto = App.Session.NombreCompleto;
                        string FirmaPath = App.Session.FirmaPath;
                        var mainWindow = new MainWindow(usuarioConfirmado, idRolUsuario, NombreCompleto);
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

        private void AutoUpdater_ApplicationExitEvent()
        {
            Application.Current.Shutdown();
        }
    }
}