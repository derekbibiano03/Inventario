using System;
using System.Collections.Generic;
using System.Text;

namespace Inventario.Core.DTOs.Requisicion
{
    public class RequisicionListDto
    {
        public string IdRequisicion { get; set; }
        public DateOnly FechaRequisicion { get; set; }
        public string Empresa { get; set; }
        public string Estatus { get; set; }
        public string Solicitante { get; set; } // Nombre en texto
        public string Autorizante { get; set; }
        public string Atencion { get; set; }
        public string TipoReq { get; set; }
        public string Reviso { get; set; }// Nombre en texto
        public string ArchivoReq { get; set; }
        public string CorreoAtencion { get; set; }
    }
}
