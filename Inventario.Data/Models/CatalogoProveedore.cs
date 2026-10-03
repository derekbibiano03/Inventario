using System;
using System.Collections.Generic;

namespace Inventario.Data.Models;

public partial class CatalogoProveedore
{
    public int IdProveedor { get; set; }

    public string? RazonSocial { get; set; }

    public string? NumeroTelefonico { get; set; }

    public string? CorreoElectronico { get; set; }

    public int? IdUbicacion { get; set; }

    public virtual CatalogoUbicacionesProyecto? IdUbicacionNavigation { get; set; }
}
