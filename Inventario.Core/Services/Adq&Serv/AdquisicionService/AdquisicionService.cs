using Inventario.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inventario.Core.Services.Adq_Serv.AdquisicionService
{
    public class AdquisicionService
    {
        private readonly InventarioContext _context;

        public AdquisicionService(InventarioContext context)
        {
            _context = context;
        }

        public List<CatalogoUbicacionesProyecto> ObtenerProyectos()
        {

            var resultado = _context.CatalogoUbicacionesProyectos.Where(p => p.Siglas != null).ToList();
            return resultado;

        }

    }
}
