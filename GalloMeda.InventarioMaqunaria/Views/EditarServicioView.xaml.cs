using Inventario.Desktop.ViewModels.EconomicosViewModel;
using System.Windows;


namespace Inventario.Desktop.Views
{
    /// <summary>
    /// Lógica de interacción para EditarServicioView.xaml
    /// </summary>
    public partial class EditarServicioView : Window
    {
        public EditarServicioView(EditarServicioViewModel viewModel)
        {
            InitializeComponent();

            // Asigna el DataContext inyectado a la ventana
            DataContext = viewModel;

            // Suscripción al evento para cerrar la ventana cuando el guardado sea exitoso
            viewModel.RequestClose += (success) =>
            {
                this.DialogResult = success;
                this.Close();
            };
        }
    }
}
