using System;
using System.Collections.Generic;

namespace Inventario.Data.Models;

public partial class CatalogoOperadore
{
    public int IdOperador { get; set; }

    public string NombreOperador { get; set; } = null!;

    public virtual ICollection<CatalogoEconomico> CatalogoEconomicos { get; set; } = new List<CatalogoEconomico>();
}
