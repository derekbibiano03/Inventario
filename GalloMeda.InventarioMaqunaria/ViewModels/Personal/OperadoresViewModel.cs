using Inventario.Core.Services.Personal;
using Inventario.Data;
using Inventario.Data.Models;
using System.Collections.ObjectModel;

namespace Inventario.Desktop.ViewModels.Personal
{
    public class OperadoresViewModel
    {
        private readonly CatalogoOperadoresService _catalogoOperadoresService;
        public ObservableCollection<CatalogoOperadore> Operadores { get; set; }

        public OperadoresViewModel() 
        {
            var context = new InventarioContext();
            _catalogoOperadoresService = new CatalogoOperadoresService(context);
            Operadores = new ObservableCollection<CatalogoOperadore>();
            CargarOperadores();
        }

        public void CargarOperadores() 
        {
            Operadores.Clear();
            var datosOper = _catalogoOperadoresService.ObtenerOperadores();
            foreach (var dato in datosOper)
            {
                Operadores.Add(dato);
            }
        }
    }
}
