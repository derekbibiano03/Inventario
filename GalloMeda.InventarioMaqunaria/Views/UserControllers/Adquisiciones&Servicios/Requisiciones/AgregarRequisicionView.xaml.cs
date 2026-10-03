using GalloMeda.InventarioMaqunaria; // Asegúrate de tener este using para acceder a App.ServiceProvider si gustas, o leer la configuración directo
using Inventario.Core.Services;
using Inventario.Core.Services.Adq_Serv.AdquisicionService;
using Inventario.Core.Services.Auth;
using Inventario.Core.Services.Logs;
using Inventario.Desktop.ViewModels.AdqServ;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Inventario.Desktop.Views.UserControllers.Adquisiciones_Servicios.Requisiciones
{
    public partial class AgregarRequisicionView : UserControl
    {
        public AgregarRequisicionView()
        {
            InitializeComponent();

            // Opción recomendada: Obtener el IConfiguration desde el contenedor global de la aplicación (App.ServiceProvider)
            // Si no usas App.ServiceProvider aquí, puedes construirlo de forma local como abajo:
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var context = new Data.Models.InventarioContext();
            var usuarios = new UsuariosService(context);
            var email = new EmailService(configuration); // <-- Aquí le pasamos la configuración requerida
            var adquisicionService = new AdquisicionService(context, email);
            var viewModel = new AgregarRequisicionViewModel(context, usuarios, adquisicionService);

            this.DataContext = viewModel;
        }
    }
}