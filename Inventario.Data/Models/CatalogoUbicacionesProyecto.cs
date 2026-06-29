using System;
using System.Collections.Generic;

namespace Inventario.Data.Models;

public partial class CatalogoUbicacionesProyecto
{
    public int IdUbicacion { get; set; }

    public string NombreProyecto { get; set; } = null!;

    public string Ubicacion { get; set; } = null!;

    public virtual ICollection<CatalogoEconomico> CatalogoEconomicos { get; set; } = new List<CatalogoEconomico>();

    public virtual ICollection<CatalogoMovimientosEconomico> CatalogoMovimientosEconomicoIdUbicacionLlegadaNavigations { get; set; } = new List<CatalogoMovimientosEconomico>();

    public virtual ICollection<CatalogoMovimientosEconomico> CatalogoMovimientosEconomicoIdUbicacionSalidaNavigations { get; set; } = new List<CatalogoMovimientosEconomico>();

    public virtual ICollection<CatalogoTramo> CatalogoTramos { get; set; } = new List<CatalogoTramo>();
}
