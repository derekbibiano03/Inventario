using Inventario.Core.Services.Catalogos;
using Inventario.Data;
using Inventario.Data.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

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
            CargarGrupos();
        
        }

        public void CargarGrupos()
        {

            Grupos.Clear();

            var datos = _gruposService.ObtenerGrupos();
            foreach (var dato in datos)
            {
                Grupos.Add(dato);
            }

        }

    }
}
