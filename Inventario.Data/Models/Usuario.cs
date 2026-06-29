using System;
using System.Collections.Generic;

namespace Inventario.Data.Models;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    public string? NombreUsuario { get; set; }

    public string? Password { get; set; }

    public int? IdRol { get; set; }

    public virtual UsuariosRole? IdRolNavigation { get; set; }

    public virtual ICollection<UsuriosRole> UsuriosRoles { get; set; } = new List<UsuriosRole>();
}
