using Inventario.Core.Services.Catalogos;
using Inventario.Data;
using Inventario.Data.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks; 

namespace Inventario.Desktop.ViewModels.CatalogosViewModel
{
    public class GruposViewModel
    {

        private readonly CatalogoGruposService _gruposService;

        public ObservableCollection<CatalogoGrupo> Grupos { get; set; }

        public GruposViewModel()
        {
            var contexto = new InventarioContext();
            _gruposService = new CatalogoGruposService(contexto);
            Grupos = new ObservableCollection<CatalogoGrupo>();
            _ = CargarGruposAsync();
        }

        public async Task CargarGruposAsync()
        {
            Grupos.Clear();

            var datos = await Task.Run(() => _gruposService.ObtenerGrupos());

            foreach (var dato in datos)
            {
                if (dato.DescripcionGrupo != null)
                {
                    dato.IdGrupo = dato.IdGrupo.ToUpper();
                    dato.DescripcionGrupo = dato.DescripcionGrupo.ToUpper();
                }
                Grupos.Add(dato);
            }
        }
    }
}