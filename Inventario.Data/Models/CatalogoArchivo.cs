using System;
using System.Collections.Generic;

namespace Inventario.Data.Models;

public partial class CatalogoArchivo
{
    public int IdArchivo { get; set; }

    public string? Archivo { get; set; }

    public string? NombreArchivo { get; set; }

    public DateTime? FechaSubida { get; set; }

    public virtual ICollection<EconomicosArchivo> EconomicosArchivos { get; set; } = new List<EconomicosArchivo>();

    public virtual ICollection<ServicioArchivo> ServicioArchivos { get; set; } = new List<ServicioArchivo>();
}
