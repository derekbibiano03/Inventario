using System;
using System.Collections.Generic;

namespace Inventario.Data.Models;

public partial class CatalogoProveedore
{
    public int IdProveedor { get; set; }

    public string? NombreProveedor { get; set; }

    public string? NumeroContacto { get; set; }

    public string? CorreoElectronico { get; set; }

    public virtual ICollection<CatalogoHerramientasMateriale> CatalogoHerramientasMateriales { get; set; } = new List<CatalogoHerramientasMateriale>();
}
