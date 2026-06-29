using System;
using System.Collections.Generic;

namespace Inventario.Data.Models;

public partial class ServiciosEconomico
{
    public int IdServicioEconomico { get; set; }

    public int? IdTipoServicio { get; set; }

    public string? IdEconomico { get; set; }

    public DateOnly? FechaServicio { get; set; }

    public int? Horometro { get; set; }

    public int? Kilometraje { get; set; }

    public virtual CatalogoEconomico? IdEconomicoNavigation { get; set; }

    public virtual CatalogoTiposServicio? IdTipoServicioNavigation { get; set; }
}
