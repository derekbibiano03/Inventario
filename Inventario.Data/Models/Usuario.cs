using System;
using System.Collections.Generic;

namespace Inventario.Data.Models;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    public string? NombreUsuario { get; set; }

    public string? Password { get; set; }

    public int? IdRol { get; set; }

    public virtual ICollection<CatalogoMovimientosEconomico> CatalogoMovimientosEconomicos { get; set; } = new List<CatalogoMovimientosEconomico>();

    public virtual ICollection<HistorialLog> HistorialLogs { get; set; } = new List<HistorialLog>();

    public virtual UsuariosRole? IdRolNavigation { get; set; }
}
