using System;
using System.Collections.Generic;

namespace Inventario.Desktop.Models;

public partial class CatalogoTiposEquipo
{
    public string IdTipoEquipo { get; set; } = null!;

    public string DescripcionTipoEquipo { get; set; } = null!;

    public virtual ICollection<CatalogoEconomico> CatalogoEconomicos { get; set; } = new List<CatalogoEconomico>();
}
