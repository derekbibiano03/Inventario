using System.Windows.Controls;
using Inventario.Desktop.ViewModels.EconomicosViewModel;
using Inventario.Core.Services.Catalogos;
using Inventario.Core.Services.Personal;
using Inventario.Core.Services.UbicacionProyecto;
using Inventario.Core.Services.Economicos;
namespace Inventario.Desktop.Views.UserControllers.Economicos
{
    public partial class EconomicosAltaView : UserControl
    {
        public EconomicosAltaView()
        {
            InitializeComponent();
            var context = new Data.Models.InventarioContext();
            var marcasServicio = new CatalogoMarcasService(context);
            var tipoEquipoServicio = new CatalogoTiposEquipoService(context);
            var grupoServicio = new CatalogoGruposService(context);
            var combusServicio = new CatalogoCombustiblesService(context);
            var pyaServicio = new ProAdminService(context);
            var ubicacionService = new UbicacionProyeectoService(context);
            var operadoresService = new CatalogoOperadoresService(context);
            var responsableService = new CatalogoEncargadoMaquinariaService(context);
            var economicosService = new CatalogoEconomicosService(context);

            this.DataContext = new EconomicosAltaViewModel(marcasServicio, tipoEquipoServicio, grupoServicio, 
                                                           combusServicio, pyaServicio, ubicacionService, 
                                                           operadoresService, responsableService, economicosService);

        }
    }
}