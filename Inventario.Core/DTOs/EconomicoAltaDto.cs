namespace Inventario.Core.DTOs
{
    public class EconomicoAltaDto
    {
        // 1. Identificadores numéricos (claves foráneas) y de texto
        public string IdTipoEquipo { get; set; }
        public string IdGrupo { get; set; }
        public int? IdCombustible { get; set; }
        public int? IdPropietario { get; set; }
        public int? IdAdministrador { get; set; }
        public int? IdUbicacion { get; set; }
        public int? IdOperador { get; set; }
        public int? IdResponsable { get; set; }
        public string GradoPropiedad { get; set; }

        // 2. Cadenas de texto
        public string Observaciones { get; set; }
        public int? IdMarca { get; set; }
        public string Modelo { get; set; }
        public string Serie { get; set; }
        public int? PeriodoFab { get; set; } // El año como entero nulo

        // 3. Motores y adicionales
        public int? MarcaMotor { get; set; }
        public string ModeloMotor { get; set; }
        public string SerieMotor { get; set; }
        public string FamiliaMotor { get; set; }
        public string Placas { get; set; }
        public string PolizaAdj { get; set; }
        public string THK { get; set;  }
        public int Horometro { get; set; }
        public string Dimensiones { get; set; }

        // 4. Booleano del seguro
        public bool EstatusSeguro { get; set; }
    }
}