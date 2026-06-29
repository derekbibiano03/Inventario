using Inventario.Data;
using System;
using Inventario.Data.Models;
using System.Collections.Generic;
using System.Text;

namespace Inventario.Core.Services.Catalogos
{
    public class CatalogoMarcasService
    {

        private readonly InventarioContext _context;

        public CatalogoMarcasService(InventarioContext context)
        {
            _context = context;
        }

        public List<CatalogoMarca> ObtenerMarcas()
        {
        
            var resultado = _context.CatalogoMarcas.ToList();
            return resultado;

        }


    }
}
