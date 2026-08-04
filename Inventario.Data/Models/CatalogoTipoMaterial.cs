using System;
using System.Collections.Generic;

namespace Inventario.Data.Models;

public partial class CatalogoTipoMaterial
{
    public string IdTipoMaterial { get; set; } = null!;

    public string? DescripcionMaterial { get; set; }

    public virtual ICollection<CatalogoHerramientasMateriale> CatalogoHerramientasMateriales { get; set; } = new List<CatalogoHerramientasMateriale>();
}
