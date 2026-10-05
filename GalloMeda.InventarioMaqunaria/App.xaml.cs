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
using System.Security.Cryptography;
using System.Text;
using System.Windows;

namespace GalloMeda.InventarioMaqunaria
{
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; } = null!;
        public static ISessionService Session { get; private set; } = new SessionService();
        public static string ConnectionString { get; set; } = string.Empty;

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

                // 1. Verificar si existe el archivo de configuración para cifrar la cadena o asegurar AzureAd
                if (File.Exists(configPath))
                {
                    string jsonContent = File.ReadAllText(configPath);
                    var jsonObject = JObject.Parse(jsonContent);

                    var connSection = jsonObject["ConnectionStrings"];
                    string connString = connSection?["InventarioConnection"]?.ToString();

                    // Si la cadena está en texto plano, la ciframos usando Windows DPAPI
                    if (!string.IsNullOrEmpty(connString) && !connString.StartsWith("DPAPI_"))
                    {
                        byte[] plainBytes = Encoding.UTF8.GetBytes(connString);
                        byte[] encryptedBytes = ProtectedData.Protect(plainBytes, null, DataProtectionScope.CurrentUser);
                        string encryptedConn = Convert.ToBase64String(encryptedBytes);

                        connSection["InventarioConnection"] = "DPAPI_" + encryptedConn;
                        File.WriteAllText(configPath, jsonObject.ToString(Newtonsoft.Json.Formatting.Indented));
                    }

                    // Asegurarnos de que contenga el nodo AzureAd
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

                if (connectionString.StartsWith("DPAPI_"))
                {
                    byte[] encryptedBytes = Convert.FromBase64String(connectionString.Substring(6));
                    byte[] plainBytes = ProtectedData.Unprotect(encryptedBytes, null, DataProtectionScope.CurrentUser);
                    connectionString = Encoding.UTF8.GetString(plainBytes);
                }

                connectionString = connectionString.Trim().Trim('"', '\'').TrimStart('\uFEFF', '\u200B');

                // ➕ Guarda la cadena limpia en la variable global
                App.ConnectionString = connectionString;

                serviceCollection.AddDbContext<InventarioContext>(options =>
                    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
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