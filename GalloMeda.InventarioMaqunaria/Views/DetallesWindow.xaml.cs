using Inventario.Data.Models;
using Inventario.Desktop.ViewModels.UbicacionProyectosViewModel;
using System.Windows;


namespace Inventario.Desktop.Views
{
    public partial class DetallesWindow : Window
    {
        private readonly InventarioContext _context;
        public DetallesWindow(string idEconomico)
        {
            InitializeComponent();
            this.DataContext = new DetallesViewModel(idEconomico);
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
