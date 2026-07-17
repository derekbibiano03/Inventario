using System;
using System.Net;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Inventario.Data.Migrations
{
    /// <inheritdoc />
    public partial class CrearTablaHistorialLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "catalogo_archivos",
                columns: table => new
                {
                    id_archivo = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    archivo = table.Column<string>(type: "text", nullable: false),
                    nombre_archivo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    fecha_subida = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("catalogo_archivos_pkey", x => x.id_archivo);
                });

            migrationBuilder.CreateTable(
                name: "catalogo_brokers",
                columns: table => new
                {
                    id_broker = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    nombre_broker = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("catalogo_brokers_pkey", x => x.id_broker);
                });

            migrationBuilder.CreateTable(
                name: "catalogo_estatus",
                columns: table => new
                {
                    id_estatus = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    descripcion_estatus = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("catalogo_estatus_pkey", x => x.id_estatus);
                });

            migrationBuilder.CreateTable(
                name: "catalogo_grupos",
                columns: table => new
                {
                    id_grupo = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    descripcion_grupo = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("catalogo_grupos_pkey", x => x.id_grupo);
                });

            migrationBuilder.CreateTable(
                name: "catalogo_marcas",
                columns: table => new
                {
                    id_marca = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    nombre_marca = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("catalogo_marcas_pkey", x => x.id_marca);
                });

            migrationBuilder.CreateTable(
                name: "catalogo_operadores",
                columns: table => new
                {
                    id_operador = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    nombre_operador = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("catalogo_operadores_pkey", x => x.id_operador);
                });

            migrationBuilder.CreateTable(
                name: "catalogo_responsable_maquinaria",
                columns: table => new
                {
                    id_responsable = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    nombre_responsable = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("catalogo_responsable_maquinaria_pkey", x => x.id_responsable);
                });

            migrationBuilder.CreateTable(
                name: "catalogo_rol_pya",
                columns: table => new
                {
                    id_rol = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    descripcion_rol = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("catalogo_rol_pya_pkey", x => x.id_rol);
                });

            migrationBuilder.CreateTable(
                name: "catalogo_tipos_combustible",
                columns: table => new
                {
                    id_combustible = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    descripcion_combustible = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("catalogo_tipos_combustible_pkey", x => x.id_combustible);
                });

            migrationBuilder.CreateTable(
                name: "catalogo_tipos_equipos",
                columns: table => new
                {
                    id_tipo_equipo = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false),
                    descripcion_tipo_equipo = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("catalogo_tipos_equipos_pkey", x => x.id_tipo_equipo);
                });

            migrationBuilder.CreateTable(
                name: "catalogo_tipos_servicios",
                columns: table => new
                {
                    id_tipo_servicio = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    descripcion_servicio = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("catalogo_tipos_servicios_pkey", x => x.id_tipo_servicio);
                });

            migrationBuilder.CreateTable(
                name: "catalogo_ubicaciones_proyectos",
                columns: table => new
                {
                    id_ubicacion = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    nombre_proyecto = table.Column<string>(type: "text", nullable: false),
                    ubicacion = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("catalogo_ubicaciones_proyectos_pkey", x => x.id_ubicacion);
                });

            migrationBuilder.CreateTable(
                name: "usuarios_roles",
                columns: table => new
                {
                    id_rol = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    descripcion_rol = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("usurios_roles_pkey", x => x.id_rol);
                });

            migrationBuilder.CreateTable(
                name: "catalogo_pya",
                columns: table => new
                {
                    id_pya = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    id_rol_pya = table.Column<int>(type: "integer", nullable: true),
                    nombre = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("catalogo_pya_pkey", x => x.id_pya);
                    table.ForeignKey(
                        name: "fk_rol_pya",
                        column: x => x.id_rol_pya,
                        principalTable: "catalogo_rol_pya",
                        principalColumn: "id_rol",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "catalogo_tramos",
                columns: table => new
                {
                    id_tramo = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    id_ubicacion = table.Column<int>(type: "integer", nullable: false),
                    nombre_tramo = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("catalogo_tramos_pkey", x => x.id_tramo);
                    table.ForeignKey(
                        name: "fk_ubicacion",
                        column: x => x.id_ubicacion,
                        principalTable: "catalogo_ubicaciones_proyectos",
                        principalColumn: "id_ubicacion",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "usuarios",
                columns: table => new
                {
                    id_usuario = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    nombre_usuario = table.Column<string>(type: "text", nullable: false),
                    password = table.Column<string>(type: "text", nullable: false),
                    id_rol = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("usuarios_pkey", x => x.id_usuario);
                    table.ForeignKey(
                        name: "fk_usuario_rol",
                        column: x => x.id_rol,
                        principalTable: "usuarios_roles",
                        principalColumn: "id_rol",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "catalogo_economicos",
                columns: table => new
                {
                    id_economico = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    consecutivo = table.Column<int>(type: "integer", nullable: true),
                    id_tipo_equipo = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false),
                    id_grupo = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    id_combustible = table.Column<int>(type: "integer", nullable: false),
                    id_propietario = table.Column<int>(type: "integer", nullable: false),
                    id_administrador = table.Column<int>(type: "integer", nullable: false),
                    id_estatus = table.Column<int>(type: "integer", nullable: true),
                    id_operador = table.Column<int>(type: "integer", nullable: false),
                    id_responsable = table.Column<int>(type: "integer", nullable: false),
                    id_ubicacion = table.Column<int>(type: "integer", nullable: false),
                    descripcion = table.Column<string>(type: "text", nullable: true),
                    modelo = table.Column<string>(type: "text", nullable: false),
                    serie = table.Column<string>(type: "text", nullable: false),
                    periodo_fabricacion = table.Column<int>(type: "integer", nullable: true),
                    motor = table.Column<string>(type: "text", nullable: true),
                    modelo_motor = table.Column<string>(type: "text", nullable: true),
                    serie_motor = table.Column<string>(type: "text", nullable: true),
                    familia_motor = table.Column<string>(type: "text", nullable: true),
                    horometro = table.Column<decimal>(type: "numeric", nullable: true),
                    Dimensiones = table.Column<string>(type: "text", nullable: true),
                    THK = table.Column<string>(type: "text", nullable: true),
                    placas = table.Column<string>(type: "text", nullable: true),
                    grado_propiedad = table.Column<string>(type: "text", nullable: false),
                    observaciones_asignaciones = table.Column<string>(type: "text", nullable: true),
                    estatus_seguro = table.Column<bool>(type: "boolean", nullable: false),
                    poliza_adjunta = table.Column<string>(type: "text", nullable: true),
                    id_marca = table.Column<int>(type: "integer", nullable: false),
                    marca_motor = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("catalogo_economicos_pkey", x => x.id_economico);
                    table.ForeignKey(
                        name: "fk_administrador_economico",
                        column: x => x.id_administrador,
                        principalTable: "catalogo_pya",
                        principalColumn: "id_pya",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_combustible_economico",
                        column: x => x.id_combustible,
                        principalTable: "catalogo_tipos_combustible",
                        principalColumn: "id_combustible",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_estatus_economico",
                        column: x => x.id_estatus,
                        principalTable: "catalogo_estatus",
                        principalColumn: "id_estatus");
                    table.ForeignKey(
                        name: "fk_grupo_economico",
                        column: x => x.id_grupo,
                        principalTable: "catalogo_grupos",
                        principalColumn: "id_grupo",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_marca_economico",
                        column: x => x.id_marca,
                        principalTable: "catalogo_marcas",
                        principalColumn: "id_marca",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_marcamotor_economico",
                        column: x => x.marca_motor,
                        principalTable: "catalogo_marcas",
                        principalColumn: "id_marca",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_operador_economico",
                        column: x => x.id_operador,
                        principalTable: "catalogo_operadores",
                        principalColumn: "id_operador",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_propietario_economico",
                        column: x => x.id_propietario,
                        principalTable: "catalogo_pya",
                        principalColumn: "id_pya",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_responsable_economico",
                        column: x => x.id_responsable,
                        principalTable: "catalogo_responsable_maquinaria",
                        principalColumn: "id_responsable",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_tipo_equipo_economico",
                        column: x => x.id_tipo_equipo,
                        principalTable: "catalogo_tipos_equipos",
                        principalColumn: "id_tipo_equipo",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_ubicacion_economico",
                        column: x => x.id_ubicacion,
                        principalTable: "catalogo_ubicaciones_proyectos",
                        principalColumn: "id_ubicacion",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "catalogo_frentes",
                columns: table => new
                {
                    id_frente = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    id_tramo = table.Column<int>(type: "integer", nullable: false),
                    nombre_frente = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("catalogo_frentes_pkey", x => x.id_frente);
                    table.ForeignKey(
                        name: "fk_ubicacion",
                        column: x => x.id_tramo,
                        principalTable: "catalogo_tramos",
                        principalColumn: "id_tramo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "historial_logs",
                columns: table => new
                {
                    id_log = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    descripcion_log = table.Column<string>(type: "text", nullable: true),
                    tipo_log = table.Column<string>(type: "text", nullable: true),
                    id_usuario = table.Column<int>(type: "integer", nullable: true),
                    fecha_log = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ip_address = table.Column<IPAddress>(type: "inet", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_historial_logs", x => x.id_log);
                    table.ForeignKey(
                        name: "fk_usuario",
                        column: x => x.id_usuario,
                        principalTable: "usuarios",
                        principalColumn: "id_usuario");
                });

            migrationBuilder.CreateTable(
                name: "usurios_roles",
                columns: table => new
                {
                    id_log = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    informacion_log = table.Column<string>(type: "text", nullable: true),
                    tipo_log = table.Column<string>(type: "text", nullable: true),
                    id_usuario = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("usurios_roles_pkey1", x => x.id_log);
                    table.ForeignKey(
                        name: "fk_log_usuario",
                        column: x => x.id_usuario,
                        principalTable: "usuarios",
                        principalColumn: "id_usuario");
                });

            migrationBuilder.CreateTable(
                name: "catalogo_movimientos_economicos",
                columns: table => new
                {
                    id_movimiento = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    id_economico = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    id_ubicacion_llegada = table.Column<int>(type: "integer", nullable: true),
                    id_ubicacion_salida = table.Column<int>(type: "integer", nullable: true),
                    ubicacion_personalizada = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("catalogo_movimientos_economicos_pkey", x => x.id_movimiento);
                    table.ForeignKey(
                        name: "fk_economico_ubicacion",
                        column: x => x.id_economico,
                        principalTable: "catalogo_economicos",
                        principalColumn: "id_economico");
                    table.ForeignKey(
                        name: "fk_llegada_proyecto_ubicacion",
                        column: x => x.id_ubicacion_llegada,
                        principalTable: "catalogo_ubicaciones_proyectos",
                        principalColumn: "id_ubicacion");
                    table.ForeignKey(
                        name: "fk_salida_proyecto_ubicacion",
                        column: x => x.id_ubicacion_salida,
                        principalTable: "catalogo_ubicaciones_proyectos",
                        principalColumn: "id_ubicacion");
                });

            migrationBuilder.CreateTable(
                name: "economicos_archivos",
                columns: table => new
                {
                    id_economico_archivo = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_archivo = table.Column<int>(type: "integer", nullable: true),
                    id_economico = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("archivos_economicos_pkey", x => x.id_economico_archivo);
                    table.ForeignKey(
                        name: "fk_archivo_economico",
                        column: x => x.id_archivo,
                        principalTable: "catalogo_archivos",
                        principalColumn: "id_archivo");
                    table.ForeignKey(
                        name: "fk_economico_archivo",
                        column: x => x.id_economico,
                        principalTable: "catalogo_economicos",
                        principalColumn: "id_economico");
                });

            migrationBuilder.CreateTable(
                name: "servicios_economicos",
                columns: table => new
                {
                    id_servicio_economico = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    id_tipo_servicio = table.Column<int>(type: "integer", nullable: true),
                    id_economico = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    fecha_servicio = table.Column<DateOnly>(type: "date", nullable: true),
                    horometro = table.Column<int>(type: "integer", nullable: true),
                    kilometraje = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("servicios_economicos_pkey", x => x.id_servicio_economico);
                    table.ForeignKey(
                        name: "fk_economico_servicio",
                        column: x => x.id_economico,
                        principalTable: "catalogo_economicos",
                        principalColumn: "id_economico");
                    table.ForeignKey(
                        name: "fk_servicio_economico",
                        column: x => x.id_tipo_servicio,
                        principalTable: "catalogo_tipos_servicios",
                        principalColumn: "id_tipo_servicio");
                });

            migrationBuilder.CreateIndex(
                name: "IX_catalogo_economicos_id_administrador",
                table: "catalogo_economicos",
                column: "id_administrador");

            migrationBuilder.CreateIndex(
                name: "IX_catalogo_economicos_id_combustible",
                table: "catalogo_economicos",
                column: "id_combustible");

            migrationBuilder.CreateIndex(
                name: "IX_catalogo_economicos_id_estatus",
                table: "catalogo_economicos",
                column: "id_estatus");

            migrationBuilder.CreateIndex(
                name: "IX_catalogo_economicos_id_grupo",
                table: "catalogo_economicos",
                column: "id_grupo");

            migrationBuilder.CreateIndex(
                name: "IX_catalogo_economicos_id_marca",
                table: "catalogo_economicos",
                column: "id_marca");

            migrationBuilder.CreateIndex(
                name: "IX_catalogo_economicos_id_operador",
                table: "catalogo_economicos",
                column: "id_operador");

            migrationBuilder.CreateIndex(
                name: "IX_catalogo_economicos_id_propietario",
                table: "catalogo_economicos",
                column: "id_propietario");

            migrationBuilder.CreateIndex(
                name: "IX_catalogo_economicos_id_responsable",
                table: "catalogo_economicos",
                column: "id_responsable");

            migrationBuilder.CreateIndex(
                name: "IX_catalogo_economicos_id_tipo_equipo",
                table: "catalogo_economicos",
                column: "id_tipo_equipo");

            migrationBuilder.CreateIndex(
                name: "IX_catalogo_economicos_id_ubicacion",
                table: "catalogo_economicos",
                column: "id_ubicacion");

            migrationBuilder.CreateIndex(
                name: "IX_catalogo_economicos_marca_motor",
                table: "catalogo_economicos",
                column: "marca_motor");

            migrationBuilder.CreateIndex(
                name: "IX_catalogo_frentes_id_tramo",
                table: "catalogo_frentes",
                column: "id_tramo");

            migrationBuilder.CreateIndex(
                name: "IX_catalogo_movimientos_economicos_id_economico",
                table: "catalogo_movimientos_economicos",
                column: "id_economico");

            migrationBuilder.CreateIndex(
                name: "IX_catalogo_movimientos_economicos_id_ubicacion_llegada",
                table: "catalogo_movimientos_economicos",
                column: "id_ubicacion_llegada");

            migrationBuilder.CreateIndex(
                name: "IX_catalogo_movimientos_economicos_id_ubicacion_salida",
                table: "catalogo_movimientos_economicos",
                column: "id_ubicacion_salida");

            migrationBuilder.CreateIndex(
                name: "IX_catalogo_pya_id_rol_pya",
                table: "catalogo_pya",
                column: "id_rol_pya");

            migrationBuilder.CreateIndex(
                name: "IX_catalogo_tramos_id_ubicacion",
                table: "catalogo_tramos",
                column: "id_ubicacion");

            migrationBuilder.CreateIndex(
                name: "IX_economicos_archivos_id_archivo",
                table: "economicos_archivos",
                column: "id_archivo");

            migrationBuilder.CreateIndex(
                name: "IX_economicos_archivos_id_economico",
                table: "economicos_archivos",
                column: "id_economico");

            migrationBuilder.CreateIndex(
                name: "IX_historial_logs_id_usuario",
                table: "historial_logs",
                column: "id_usuario");

            migrationBuilder.CreateIndex(
                name: "IX_servicios_economicos_id_economico",
                table: "servicios_economicos",
                column: "id_economico");

            migrationBuilder.CreateIndex(
                name: "IX_servicios_economicos_id_tipo_servicio",
                table: "servicios_economicos",
                column: "id_tipo_servicio");

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_id_rol",
                table: "usuarios",
                column: "id_rol");

            migrationBuilder.CreateIndex(
                name: "IX_usurios_roles_id_usuario",
                table: "usurios_roles",
                column: "id_usuario");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "catalogo_brokers");

            migrationBuilder.DropTable(
                name: "catalogo_frentes");

            migrationBuilder.DropTable(
                name: "catalogo_movimientos_economicos");

            migrationBuilder.DropTable(
                name: "economicos_archivos");

            migrationBuilder.DropTable(
                name: "historial_logs");

            migrationBuilder.DropTable(
                name: "servicios_economicos");

            migrationBuilder.DropTable(
                name: "usurios_roles");

            migrationBuilder.DropTable(
                name: "catalogo_tramos");

            migrationBuilder.DropTable(
                name: "catalogo_archivos");

            migrationBuilder.DropTable(
                name: "catalogo_economicos");

            migrationBuilder.DropTable(
                name: "catalogo_tipos_servicios");

            migrationBuilder.DropTable(
                name: "usuarios");

            migrationBuilder.DropTable(
                name: "catalogo_pya");

            migrationBuilder.DropTable(
                name: "catalogo_tipos_combustible");

            migrationBuilder.DropTable(
                name: "catalogo_estatus");

            migrationBuilder.DropTable(
                name: "catalogo_grupos");

            migrationBuilder.DropTable(
                name: "catalogo_marcas");

            migrationBuilder.DropTable(
                name: "catalogo_operadores");

            migrationBuilder.DropTable(
                name: "catalogo_responsable_maquinaria");

            migrationBuilder.DropTable(
                name: "catalogo_tipos_equipos");

            migrationBuilder.DropTable(
                name: "catalogo_ubicaciones_proyectos");

            migrationBuilder.DropTable(
                name: "usuarios_roles");

            migrationBuilder.DropTable(
                name: "catalogo_rol_pya");
        }
    }
}
