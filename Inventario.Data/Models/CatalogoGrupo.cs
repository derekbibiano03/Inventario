using System;
using System.Collections.Generic;

namespace Inventario.Data.Models;

public partial class CatalogoGrupo
{
    public string IdGrupo { get; set; } = null!;

    public string DescripcionGrupo { get; set; } = null!;

    public virtual ICollection<CatalogoEconomico> CatalogoEconomicos { get; set; } = new List<CatalogoEconomico>();
}
