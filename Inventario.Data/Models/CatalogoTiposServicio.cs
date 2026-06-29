using System;
using System.Collections.Generic;

namespace Inventario.Data.Models;

public partial class CatalogoTiposServicio
{
    public int IdTipoServicio { get; set; }

    public string DescripcionServicio { get; set; } = null!;

    public virtual ICollection<ServiciosEconomico> ServiciosEconomicos { get; set; } = new List<ServiciosEconomico>();
}
