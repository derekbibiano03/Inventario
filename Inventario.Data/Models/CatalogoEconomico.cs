using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventario.Data.Models;

public partial class CatalogoEconomico
{
    [NotMapped]
    public bool IsSelected { get; set; }

    [Key]
    public string IdEconomico { get; set; } = null!;

    public int? Consecutivo { get; set; }

    public required string IdTipoEquipo { get; set; }

    public required string IdGrupo { get; set; }

    public required int IdCombustible { get; set; }

    public required int IdPropietario { get; set; }

    public required int IdAdministrador { get; set; }

    public int? IdEstatus { get; set; }

    public required int IdOperador { get; set; }

    public required int IdResponsable { get; set; }

    public required int IdUbicacion { get; set; }

    public string? Descripcion { get; set; }

    public required string Modelo { get; set; }

    public required string Serie { get; set; }

    public int? PeriodoFabricacion { get; set; }

    public string? Motor { get; set; }

    public string? ModeloMotor { get; set; }

    public string? SerieMotor { get; set; }

    public string? FamiliaMotor { get; set; }

    public decimal? Horometro { get; set; }

    public string? Dimensiones { get; set; }

    public string? THK { get; set; }
    public string? Placas { get; set; }

    public required string GradoPropiedad { get; set; }

    public string? ObservacionesAsignaciones { get; set; }

    public required bool EstatusSeguro { get; set; }

    public string? PolizaAdjunta { get; set; }

    public required int IdMarca { get; set; }

    public required int MarcaMotor { get; set; }

    public virtual ICollection<CatalogoMovimientosEconomico> CatalogoMovimientosEconomicos { get; set; } = new List<CatalogoMovimientosEconomico>();

    public virtual ICollection<EconomicosArchivo> EconomicosArchivos { get; set; } = new List<EconomicosArchivo>();

    public virtual CatalogoPya? IdAdministradorNavigation { get; set; }

    public virtual CatalogoTiposCombustible? IdCombustibleNavigation { get; set; }

    public virtual CatalogoEstatus? IdEstatusNavigation { get; set; }

    public virtual CatalogoGrupo? IdGrupoNavigation { get; set; }

    public virtual CatalogoMarca? IdMarcaNavigation { get; set; }

    public virtual CatalogoOperadore? IdOperadorNavigation { get; set; }

    public virtual CatalogoPya? IdPropietarioNavigation { get; set; }

    public virtual CatalogoResponsableMaquinarium? IdResponsableNavigation { get; set; }

    public virtual CatalogoTiposEquipo? IdTipoEquipoNavigation { get; set; }

    public virtual CatalogoUbicacionesProyecto? IdUbicacionNavigation { get; set; }

    public virtual CatalogoMarca? MarcaMotorNavigation { get; set; }

    public virtual ICollection<ServiciosEconomico> ServiciosEconomicos { get; set; } = new List<ServiciosEconomico>();
}
