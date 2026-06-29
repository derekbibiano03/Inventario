using System;
using System.Collections.Generic;

namespace Inventario.Data.Models;

public partial class CatalogoArchivo
{
    public int IdArchivo { get; set; }

    public string Archivo { get; set; } = null!;

    public string? NombreArchivo { get; set; }

    public DateTime? FechaSubida { get; set; }

    public virtual ICollection<EconomicosArchivo> EconomicosArchivos { get; set; } = new List<EconomicosArchivo>();
}
