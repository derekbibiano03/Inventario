using Inventario.Core.Services.Economicos;
using Inventario.Core.Services.Logs;
using Inventario.Core.Services.UbicacionProyecto;
using Inventario.Desktop.ViewModels.EconomicosViewModel.Movimientos;
using System.Windows.Controls;

namespace Inventario.Desktop.Views.UserControllers.Economicos
{
    /// <summary>
    /// Lógica de interacción para RealizarMovimientoView.xaml
    /// </summary>
    public partial class RealizarMovimientoView : UserControl
    {
        public RealizarMovimientoView()
        {
            InitializeComponent();
            var context = new Data.Models.InventarioContext();
            var logsService = new LogsService(context);
            var ubicacionService = new UbicacionProyeectoService(context);
            var economicosService = new CatalogoEconomicosService (context, logsService);
            var realizarMovimientosService = new RealizarMovimientosService(context, logsService);
            this.DataContext = new RealizarMovimientoViewModel(ubicacionService, economicosService, realizarMovimientosService); 
        }
    }
}
