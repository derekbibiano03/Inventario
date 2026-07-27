using System;
using System.Collections.Generic;
using System.Net;

namespace Inventario.Data.Models;

public partial class HistorialLog
{
    public int IdLog { get; set; }

    public string? DescripcionLog { get; set; }

    public string? TipoLog { get; set; }

    public int? IdUsuario { get; set; }

    public DateTime? FechaLog { get; set; }

    public IPAddress? IpAddress { get; set; }

    public virtual Usuario? IdUsuarioNavigation { get; set; }
}
