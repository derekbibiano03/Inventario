using Inventario.Core.Services.Catalogos;
using Inventario.Data;
using Inventario.Data.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Text;

namespace Inventario.Desktop.ViewModels.CatalogosViewModel
{
    public class MarcasViewModel
    {
        private readonly CatalogoMarcasService _marcasService;
        public ObservableCollection<CatalogoMarca> Marcas { get; set; }

        public MarcasViewModel() 
        {
            var context = new InventarioContext();
            _marcasService = new CatalogoMarcasService(context);
            Marcas = new ObservableCollection<CatalogoMarca>();
            CargarMarcas();
        }

        public void CargarMarcas() 
        {
            Marcas.Clear();
            var datos = _marcasService.ObtenerMarcas();
            foreach (var dato in datos)
            { 
                Marcas.Add(dato);
            }
        }
    }
}
