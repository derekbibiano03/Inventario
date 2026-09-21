using Inventario.Core.Services.Adq_Serv.AdquisicionService;
using Inventario.Core.Services.Logs;
using Inventario.Desktop.ViewModels.AdqServ; // CORRECTO
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
    public partial class AgregarRequisicionView : UserControl
    {
        public AgregarRequisicionView()
        {
            InitializeComponent();
            var context = new Data.Models.InventarioContext();
            var viewModel = new AgregarRequisicionViewModel(context);

            this.DataContext = viewModel;
        }
    }
}
