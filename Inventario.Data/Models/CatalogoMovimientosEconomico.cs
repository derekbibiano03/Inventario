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

    public DateTime? FechaMovimiento { get; set; }

    public string? NombreArchivo { get; set; }

    public string? Archivo { get; set; }

    public string? NombreArchivo2 { get; set; }

    public string? Archivo2 { get; set; }

    public int? IdUsuario { get; set; }

    public virtual CatalogoEconomico? IdEconomicoNavigation { get; set; }

    public virtual CatalogoUbicacionesProyecto? IdUbicacionLlegadaNavigation { get; set; }

    public virtual CatalogoUbicacionesProyecto? IdUbicacionSalidaNavigation { get; set; }

    public virtual Usuario? IdUsuarioNavigation { get; set; }
}
