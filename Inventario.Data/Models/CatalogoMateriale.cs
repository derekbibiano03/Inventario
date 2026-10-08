using System;
using System.Collections.Generic;

namespace Inventario.Data.Models;

public partial class CatalogoMateriale
{
    public int IdMaterial { get; set; }

    public string? DescripcionCorta { get; set; }

    public string? DescripcionLarga { get; set; }

    public string? UnidadMedida { get; set; }

    public string? NoParte { get; set; }

    public int? Marca { get; set; }

    public int? IdClasificacion { get; set; }

    public int? IdSubclasificacion { get; set; }

    public virtual CatalogoClasificacione? IdClasificacionNavigation { get; set; }

    public virtual CatalogoSubclasificacione? IdSubclasificacionNavigation { get; set; }

    public virtual CatalogoMarca? MarcaNavigation { get; set; }
}
