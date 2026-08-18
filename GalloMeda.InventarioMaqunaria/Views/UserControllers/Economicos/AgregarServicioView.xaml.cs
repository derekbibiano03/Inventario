using Inventario.Core.Services.Economicos;
using Inventario.Core.Services.Logs;
using Inventario.Desktop.ViewModels.EconomicosViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Inventario.Desktop.Views.UserControllers.Economicos
{
    /// <summary>
    /// Lógica de interacción para AgregarServicioView.xaml
    /// </summary>
    public partial class AgregarServicioView : UserControl
    {
        public AgregarServicioView()
        {
            InitializeComponent();
            var context = new Data.Models.InventarioContext();
            var logsService = new LogsService(context);
            var economicosService = new CatalogoEconomicosService(context, logsService);
            var historialServiciosService = new HistorialServicioService(context, logsService);
            var gestorArchivosService = new GestorArchivosService(context);

            this.DataContext = new HistorialServicioViewModel(economicosService, historialServiciosService, gestorArchivosService);
        }
    }
}
