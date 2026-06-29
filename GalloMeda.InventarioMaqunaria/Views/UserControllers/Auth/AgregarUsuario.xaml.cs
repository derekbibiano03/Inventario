using Inventario.Core.Services.Auth;
using Inventario.Data.Models;
using Inventario.Desktop.ViewModels.Auth;
using System.Windows.Controls;

namespace Inventario.Desktop.Views.UserControllers.Auth
{
    public partial class AgregarUsuario : UserControl
    {
        public AgregarUsuario()
        {
           InitializeComponent();
           var context = new InventarioContext();

           var authService = new AutenticacionService(context);

           this.DataContext = new AgregarUsuarioViewModel(authService);
        }
    }
}
