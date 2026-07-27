using Inventario.Core.Services.Adq_Serv.AdquisicionService;
using Inventario.Core.Services.Logs;
using Inventario.Desktop.ViewModels.Adq_Serv.RequisicionesViewModel;
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
using static Inventario.Desktop.ViewModels.Adq_Serv.RequisicionesViewModel.AgregarRequisicion;

namespace Inventario.Desktop.Views.UserControllers.Adquisiciones_Servicios.Requisiciones
{
    /// <summary>
    /// Lógica de interacción para AgregarRequisicion.xaml
    /// </summary>
    public partial class AgregarRequisicionView : UserControl
    {
        public AgregarRequisicionView()
        {
            InitializeComponent();
            var context = new Data.Models.InventarioContext();
            var adquisicionesService = new AdquisicionService(context);
            this.DataContext = new AgregarRequisicion(adquisicionesService);

            List<OpcionComboBox> lista = new List<OpcionComboBox>
            {
                new OpcionComboBox { Texto = "CONSTRUCTORA GALLO MEDA", Valor = "CGM" },
                new OpcionComboBox { Texto = "OX TRANSPORTE Y LOGISTICA", Valor = "OXT" },
                new OpcionComboBox { Texto = "ENLACE FERROVIARIO", Valor = "ENF" }
            };
            cmbOpciones.ItemsSource = lista;

            List<OpcionComboBox> lista2 = new List<OpcionComboBox>
            {
                new OpcionComboBox { Texto = "ADQUISICIONES", Valor = "ADQ" },
                new OpcionComboBox { Texto = "SERVICIO", Valor = "SRV" },
                new OpcionComboBox { Texto = "PROVEEDORA", Valor = "PRA" }
            };
            cmbOpcionesTipo.ItemsSource = lista2;
        }
    }
}
