using Inventario.Core.Services.Personal;
using Inventario.Data;
using Inventario.Data.Models;
using Inventario.Desktop.ViewModels.EconomicosViewModel;
using System.Collections.ObjectModel;

namespace Inventario.Desktop.ViewModels.Personal
{
    public class EncargadosViewModel
    {
        private readonly CatalogoEncargadoMaquinariaService _encargadoService;
        public ObservableCollection<CatalogoResponsableMaquinarium> Encargados { get; set; }

        public EncargadosViewModel()
        {
            var context = new InventarioContext();
            _encargadoService = new CatalogoEncargadoMaquinariaService(context);
            Encargados = new ObservableCollection<CatalogoResponsableMaquinarium>();
            CargarResponsables();
        }

        public void CargarResponsables()
        { 
            Encargados.Clear();
            var datosEncar = _encargadoService.ObtenerResponsables();
            foreach (var dato in datosEncar)
            {
                Encargados.Add(dato);
            }
        }
    }
}
