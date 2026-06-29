using System;
using System.Collections.Generic;

namespace Inventario.Data.Models;

public partial class CatalogoMovimientosEconomico
{
    public int IdMovimiento { get; set; }

    public string? IdEconomico { get; set; }

    public int? IdUbicacionLlegada { get; set; }

    public int? IdUbicacionSalida { get; set; }

    public string? UbicacionPersonalizada { get; set; }

    public virtual CatalogoEconomico? IdEconomicoNavigation { get; set; }

    public virtual CatalogoUbicacionesProyecto? IdUbicacionLlegadaNavigation { get; set; }

    public virtual CatalogoUbicacionesProyecto? IdUbicacionSalidaNavigation { get; set; }
}
