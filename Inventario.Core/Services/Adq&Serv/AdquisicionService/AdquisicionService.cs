using Inventario.Core.DTOs.Requisicion;
using Inventario.Data.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Xceed.Document.NET;
using Xceed.Words.NET;

namespace Inventario.Core.Services.Adq_Serv.AdquisicionService
{
    public class AdquisicionService
    {
        private readonly InventarioContext _context;

        public AdquisicionService(InventarioContext context)
        {

            _context = context;

        }

        public bool NuevaRequisicion(string IdRequisicion, int IdSolicitante, int IdAutorizante, 
                                     int IdUbicacion, DateOnly FechaRequisicion, string TipoRequisicion, 
                                     string Empresa, string Estatus, string ArchivoReq)
        {
            var nuevaRequisicion = new Requisicione
            {
                IdRequisicion = IdRequisicion,
                IdSolicitante = IdSolicitante,
                IdAutorizante = IdAutorizante,
                IdUbicacion = IdUbicacion,
                FechaRequisicion = FechaRequisicion,
                TipoRequisicion = TipoRequisicion,
                Empresa = Empresa,
                Estatus = Estatus,
                ArchivoReq = ArchivoReq
            };

            _context.Requisiciones.Add(nuevaRequisicion);
            _context.SaveChanges();
            return true;
        }

    }
}
