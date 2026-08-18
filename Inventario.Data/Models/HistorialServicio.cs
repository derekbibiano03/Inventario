using System;
using System.Collections.Generic;

namespace Inventario.Data.Models;

public partial class HistorialServicio
{
    public int IdServicio { get; set; }

    public string NoEconomico { get; set; } = null!;

    public DateOnly FechaMantenimiento { get; set; }

    public string TipoMantenimiento { get; set; } = null!;

    public string? Anotaciones { get; set; }

    public string? Horaskilometrosreales { get; set; }

    public virtual CatalogoEconomico NoEconomicoNavigation { get; set; } = null!;

    public virtual ICollection<ServicioArchivo> ServicioArchivos { get; set; } = new List<ServicioArchivo>();
}
