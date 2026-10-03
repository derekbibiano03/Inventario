using System;
using System.Collections.Generic;

namespace Inventario.Data.Models;

public partial class Requisicione
{
    public string IdRequisicion { get; set; } = null!;

    public int? IdUbicacion { get; set; }

    public int? Consecutivo { get; set; }

    public DateOnly? FechaRequisicion { get; set; }

    public string? TipoRequisicion { get; set; }

    public string? Empresa { get; set; }

    public string? Estatus { get; set; }

    public string? ArchivoReq { get; set; }

    public int? IdSolicitante { get; set; }

    public int? IdAutorizante { get; set; }

    public int? IdAtencion { get; set; }

    public virtual Usuario? IdAtencionNavigation { get; set; }

    public virtual Usuario? IdAutorizanteNavigation { get; set; }

    public virtual Usuario? IdSolicitanteNavigation { get; set; }
}
