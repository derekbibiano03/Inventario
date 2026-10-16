using System;
using System.Collections.Generic;

namespace Inventario.Desktop.Models;

public partial class CatalogoTiposCombustible
{
    public int IdCombustible { get; set; }

    public string? DescripcionCombustible { get; set; }

    public virtual ICollection<CatalogoEconomico> CatalogoEconomicos { get; set; } = new List<CatalogoEconomico>();
}
