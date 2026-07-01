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
            // Inicializa todos los elementos gráficos definidos en el documento XAML.
            InitializeComponent();

            // Crea una nueva instancia del ViewModel que manejará el flujo de datos para esta edición.
            var viewModel = new EditarEconomicoViewModel();

            // Invoca la carga pasándole el valor numérico; este buscará la entidad y rellenará la propiedad EconomicoEdicion.
            viewModel.CargarDatosEconomico(idEconomico);

            // Vincula el objeto del ViewModel directamente al contexto de datos de esta interfaz de usuario de WPF.
            this.DataContext = viewModel;

            // Evalúa si la acción de cierre no ha sido configurada previamente.
            if (viewModel.CloseAction == null)
            {
                // Inicializa un delegado anónimo pasándole una bandera booleana para controlar el éxito del guardado.
                viewModel.CloseAction = new System.Action<bool>((bool resultado) =>
                {
                    // Asigna el estado resultante a la ventana para avisar a quien la invocó si hubo cambios guardados.
                    this.DialogResult = resultado;

                    // Fuerza la destrucción y el cierre visual de la ventana emergente.
                    this.Close();
                });
            }
        }
    }
}