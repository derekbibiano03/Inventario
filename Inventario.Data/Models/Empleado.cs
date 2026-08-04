using System;
using System.Collections.Generic;

namespace Inventario.Data.Models;

public partial class Empleado
{
    public int NoEmpleado { get; set; }

    public string? NombreEmpleado { get; set; }

    public int? IdRolEmpleado { get; set; }

    public DateOnly? Ds3 { get; set; }

    public virtual ICollection<CatalogoEconomico> CatalogoEconomicoIdOperadorNavigations { get; set; } = new List<CatalogoEconomico>();

    public virtual ICollection<CatalogoEconomico> CatalogoEconomicoIdResponsableNavigations { get; set; } = new List<CatalogoEconomico>();

    public virtual RolEmpleado? IdRolEmpleadoNavigation { get; set; }
}
