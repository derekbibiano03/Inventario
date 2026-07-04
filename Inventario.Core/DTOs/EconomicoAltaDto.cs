namespace Inventario.Core.DTOs
{
    public class EconomicoAltaDto
    {
        // 1. Identificadores numéricos (claves foráneas) cambiados a anulables para soportar controles de WPF
        public required string IdTipoEquipo { get; set; }
        public required string IdGrupo { get; set; }

        // CORRECCIÓN: Se remueve 'required' y se cambia 'int' por 'int?'
        public int? IdCombustible { get; set; }
        public int? IdPropietario { get; set; }
        public int? IdAdministrador { get; set; }
        public int? IdUbicacion { get; set; }
        public int? IdOperador { get; set; }
        public int? IdResponsable { get; set; }
        public required string GradoPropiedad { get; set; }

        // 2. Cadenas de texto y marcas
        public string? Observaciones { get; set; }
        public int? IdMarca { get; set; } // CORRECCIÓN: int?
        public required string Modelo { get; set; }
        public required string Serie { get; set; }
        public int? PeriodoFab { get; set; }

        // 3. Motores y adicionales
        public int? MarcaMotor { get; set; } // CORRECCIÓN: int?
        public string? ModeloMotor { get; set; }
        public string? SerieMotor { get; set; }
        public string? FamiliaMotor { get; set; }
        public string? Placas { get; set; }
        public string? PolizaAdj { get; set; }
        public string? THK { get; set; }
        public int? Horometro { get; set; }
        public string? Dimensiones { get; set; }

        // 4. Booleano del seguro
        public required bool EstatusSeguro { get; set; }
    }
}