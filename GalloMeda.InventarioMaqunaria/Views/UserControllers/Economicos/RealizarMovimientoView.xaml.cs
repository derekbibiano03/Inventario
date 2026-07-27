using Inventario.Core.Services.Economicos;
using Inventario.Core.Services.Logs;
using Inventario.Core.Services.UbicacionProyecto;
using Inventario.Desktop.ViewModels.EconomicosViewModel;
using Microsoft.EntityFrameworkCore;
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
