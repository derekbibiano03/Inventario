using Inventario.Data;
using Inventario.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inventario.Core.Services.Personal
{
    public class CatalogoOperadoresService
    {
        private readonly InventarioContext _context;

        public CatalogoOperadoresService(InventarioContext context)
        {
            _context = context;
        }

        public List<CatalogoOperadore> ObtenerOperadores()
        {
            var resultado = _context.CatalogoOperadores
                .Select(e => new CatalogoOperadore
                {
                    IdOperador = e.IdOperador,
                    NombreOperador = e.NombreOperador
                })
                .ToList();
            return resultado;
        }
    }
}
