using System;
using System.Collections.Generic;

namespace Inventario.Data.Models;

public partial class Requisicione
{
    public string IdRequisicion { get; set; } = null!;

    public int? IdUbicacion { get; set; }

    public int? Consecutivo { get; set; }

    public string? RazonSocial { get; set; }

    public DateOnly? FechaRequisicion { get; set; }

    public string? TipoRequisicion { get; set; }
}
