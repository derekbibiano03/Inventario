using Inventario.Core.Services.Economicos;
using Inventario.Core.Services.Logs;
using Inventario.Desktop.ViewModels.EconomicosViewModel.Servicios;
using System.Windows.Controls;

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
