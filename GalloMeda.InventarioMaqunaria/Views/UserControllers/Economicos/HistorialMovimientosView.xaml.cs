using Inventario.Core.Services.Economicos;
using Inventario.Core.Services.Logs;
using Inventario.Core.Services.UbicacionProyecto;
using Inventario.Desktop.ViewModels.EconomicosViewModel.Movimientos;
using System.Windows.Controls;

namespace Inventario.Desktop.Views.UserControllers.Economicos
{
    /// <summary>
    /// Lógica de interacción para HistorialMovimientosView.xaml
    /// </summary>
    public partial class HistorialMovimientosView : UserControl
    {
        public HistorialMovimientosView()
        {
            InitializeComponent();
            var context = new Data.Models.InventarioContext();
            var logsService = new LogsService(context);
            var ubicacionService = new UbicacionProyeectoService(context);
            var economicosService = new CatalogoEconomicosService(context, logsService);
            var realizarMovimientosService = new RealizarMovimientosService(context, logsService);
            this.DataContext = new RealizarMovimientoViewModel(ubicacionService, economicosService, realizarMovimientosService);
        }
    }
}
