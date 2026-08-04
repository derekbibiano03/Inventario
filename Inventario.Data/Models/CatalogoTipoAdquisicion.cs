using System;
using System.Collections.Generic;

namespace Inventario.Data.Models;

public partial class CatalogoTipoAdquisicion
{
    public string IdTipoAdquisicion { get; set; } = null!;

    public string? DescripcionAdquisicion { get; set; }

    public virtual ICollection<CatalogoHerramientasMateriale> CatalogoHerramientasMateriales { get; set; } = new List<CatalogoHerramientasMateriale>();
}
