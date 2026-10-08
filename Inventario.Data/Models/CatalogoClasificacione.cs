using System;
using System.Collections.Generic;

namespace Inventario.Data.Models;

public partial class CatalogoClasificacione
{
    public int IdClasificacion { get; set; }

    public string? Descripcion { get; set; }

    public virtual ICollection<CatalogoMateriale> CatalogoMateriales { get; set; } = new List<CatalogoMateriale>();

    public virtual ICollection<CatalogoSubclasificacione> CatalogoSubclasificaciones { get; set; } = new List<CatalogoSubclasificacione>();
}
