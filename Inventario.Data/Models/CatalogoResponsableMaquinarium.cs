using System;
using System.Collections.Generic;

namespace Inventario.Data.Models;

public partial class CatalogoResponsableMaquinarium
{
    public int IdResponsable { get; set; }

    public string NombreResponsable { get; set; } = null!;

    public virtual ICollection<CatalogoEconomico> CatalogoEconomicos { get; set; } = new List<CatalogoEconomico>();
}
