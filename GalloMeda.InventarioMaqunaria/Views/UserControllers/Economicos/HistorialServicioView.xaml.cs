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
    /// Lógica de interacción para HistorialServicioView.xaml
    /// </summary>
    public partial class HistorialServicioView : UserControl
    {
        public HistorialServicioView()
        {
            InitializeComponent();
            var context = new Data.Models.InventarioContext();
            var logsService = new LogsService(context);
            var economicosService = new CatalogoEconomicosService(context, logsService);
            var gestorarchivo = new GestorArchivosService(context);
            var historialServicio = new HistorialServicioService(context, logsService);
            this.DataContext = new HistorialServicioViewModel(economicosService, historialServicio, gestorarchivo);
        }
    }
}
