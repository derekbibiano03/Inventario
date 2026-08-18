using Inventario.Desktop.ViewModels;
using System.Windows;

namespace Inventario.Desktop.Views
{
    /// <summary>
    /// Lógica de interacción para EditarEconomicoWindow.xaml
    /// </summary>
    public partial class EditarEconomicoWindow : Window
    {
        // CORRECCIÓN: El constructor de la ventana ahora recibe correctamente un entero (int) en lugar de una cadena (string).
        public EditarEconomicoWindow(string idEconomico)
        {
            InitializeComponent();

            var viewModel = new EditarEconomicoViewModel();

            viewModel.CargarDatosEconomico(idEconomico);

            this.DataContext = viewModel;

            if (viewModel.CloseAction == null)
            {
                viewModel.CloseAction = new System.Action<bool>((bool resultado) =>
                {
                    this.DialogResult = resultado;

                    this.Close();
                });
            }
        }
    }
}