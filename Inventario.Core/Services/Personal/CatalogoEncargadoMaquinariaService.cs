using Inventario.Data;
using Inventario.Data.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Inventario.Desktop.ViewModels.EconomicosViewModel
{
    public class CatalogoEncargadoMaquinariaService
    {
        private readonly InventarioContext _context;

        public CatalogoEncargadoMaquinariaService(InventarioContext context)
        {
            _context = context;
        }

        public List<CatalogoResponsableMaquinarium> ObtenerResponsables()
        {
            var resultado = _context.CatalogoResponsableMaquinaria
                .Select(e => new CatalogoResponsableMaquinarium
                {
                    IdResponsable = e.IdResponsable,
                    NombreResponsable = e.NombreResponsable
                })
                .ToList();
            return resultado;
        }

    }
}
