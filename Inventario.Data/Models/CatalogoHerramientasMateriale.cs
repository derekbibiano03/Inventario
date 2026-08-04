using System;
using System.Collections.Generic;

namespace Inventario.Data.Models;

public partial class CatalogoHerramientasMateriale
{
    public string IdAdsquisicion { get; set; } = null!;

    public string? IdTipoAdquisicion { get; set; }

    public int? Cantidad { get; set; }

    public int? IdUbicacionAlmacen { get; set; }

    public decimal? Precio { get; set; }

    public int? IdProveedor { get; set; }

    public string? UnidadMedida { get; set; }

    public string? IdTipoMaterial { get; set; }

    public int? IdPersonaResguardo { get; set; }

    public virtual CatalogoProveedore? IdProveedorNavigation { get; set; }

    public virtual CatalogoTipoAdquisicion? IdTipoAdquisicionNavigation { get; set; }

    public virtual CatalogoTipoMaterial? IdTipoMaterialNavigation { get; set; }

    public virtual CatalogoUbicacionesProyecto? IdUbicacionAlmacenNavigation { get; set; }
}
