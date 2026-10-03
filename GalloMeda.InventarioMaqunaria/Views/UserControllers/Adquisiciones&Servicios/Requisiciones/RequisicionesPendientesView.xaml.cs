using Inventario.Desktop.ViewModels.AdqServ;
using Inventario.Desktop.ViewModels.CatalogosViewModel;
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

namespace Inventario.Desktop.Views.UserControllers.Adquisiciones_Servicios.Requisiciones
{
    /// <summary>
    /// Lógica de interacción para RequisicionesPendientesView.xaml
    /// </summary>
    public partial class RequisicionesPendientesView : UserControl
    {
        public RequisicionesPendientesView()
        {
            InitializeComponent();
            this.DataContext = new ListaRequisicionesViewModel();
        }
        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Lógica opcional del evento o déjalo vacío si solo quieres que compile
        }
    }
}
