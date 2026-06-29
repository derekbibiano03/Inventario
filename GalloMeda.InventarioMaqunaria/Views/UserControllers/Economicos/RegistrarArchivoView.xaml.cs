using Inventario.Desktop.ViewModels.EconomicosViewModel;
using Inventario.Desktop.ViewModels.EconomicosViewModel.EconomicosViewModel;
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
    /// Lógica de interacción para RegistrarArchivoView.xaml
    /// </summary>
    public partial class RegistrarArchivoView : UserControl
    {
        public RegistrarArchivoView()
        {
            InitializeComponent();
            this.DataContext = new RegistrarArchivoViewModel();
        }
    }
}
