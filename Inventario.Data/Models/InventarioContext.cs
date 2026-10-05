using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace Inventario.Data.Models;

public partial class InventarioContext : DbContext
{
    public InventarioContext()
    {
    }

    public InventarioContext(DbContextOptions<InventarioContext> options)
        : base(options)
    {
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var configuration = new Microsoft.Extensions.Configuration.ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var connectionString = configuration.GetConnectionString("InventarioConnection");

            if (!string.IsNullOrEmpty(connectionString))
            {
                if (connectionString.StartsWith("DPAPI_"))
                {
                    byte[] encryptedBytes = Convert.FromBase64String(connectionString.Substring(6));
                    byte[] plainBytes = System.Security.Cryptography.ProtectedData.Unprotect(encryptedBytes, null, System.Security.Cryptography.DataProtectionScope.CurrentUser);
                    connectionString = System.Text.Encoding.UTF8.GetString(plainBytes);
                }

                connectionString = connectionString.Trim().Trim('"', '\'').TrimStart('\uFEFF', '\u200B');

                optionsBuilder.UseMySql(
                    connectionString,
                    ServerVersion.AutoDetect(connectionString)
                );
            }
        }
    }

    public virtual DbSet<CatalogoArchivo> CatalogoArchivos { get; set; }

    public virtual DbSet<CatalogoEconomico> CatalogoEconomicos { get; set; }

    public virtual DbSet<CatalogoEstatus> CatalogoEstatuses { get; set; }

    public virtual DbSet<CatalogoFrente> CatalogoFrentes { get; set; }

    public virtual DbSet<CatalogoGrupo> CatalogoGrupos { get; set; }

    public virtual DbSet<CatalogoMarca> CatalogoMarcas { get; set; }

    public virtual DbSet<CatalogoMovimientosEconomico> CatalogoMovimientosEconomicos { get; set; }

    public virtual DbSet<CatalogoProveedore> CatalogoProveedores { get; set; }

    public virtual DbSet<CatalogoPya> CatalogoPyas { get; set; }

    public virtual DbSet<CatalogoRolPya> CatalogoRolPyas { get; set; }

    public virtual DbSet<CatalogoTiposCombustible> CatalogoTiposCombustibles { get; set; }

    public virtual DbSet<CatalogoTiposEquipo> CatalogoTiposEquipos { get; set; }

    public virtual DbSet<CatalogoTramo> CatalogoTramos { get; set; }

    public virtual DbSet<CatalogoUbicacionesProyecto> CatalogoUbicacionesProyectos { get; set; }

    public virtual DbSet<EconomicosArchivo> EconomicosArchivos { get; set; }

    public virtual DbSet<EfmigrationsHistory> EfmigrationsHistories { get; set; }

    public virtual DbSet<Empleado> Empleados { get; set; }

    public virtual DbSet<HistorialLog> HistorialLogs { get; set; }

    public virtual DbSet<HistorialReporte> HistorialReportes { get; set; }

    public virtual DbSet<HistorialServicio> HistorialServicios { get; set; }

    public virtual DbSet<Requisicione> Requisiciones { get; set; }

    public virtual DbSet<RolEmpleado> RolEmpleados { get; set; }

    public virtual DbSet<ServicioArchivo> ServicioArchivos { get; set; }

    public virtual DbSet<TiposReporte> TiposReportes { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    public virtual DbSet<UsuariosRole> UsuariosRoles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<CatalogoArchivo>(entity =>
        {
            entity.HasKey(e => e.IdArchivo).HasName("PRIMARY");

            entity
                .ToTable("catalogo_archivos")
                .UseCollation("utf8mb4_general_ci");

            entity.Property(e => e.IdArchivo).HasColumnName("id_archivo");
            entity.Property(e => e.Archivo)
                .HasColumnType("text")
                .HasColumnName("archivo");
            entity.Property(e => e.FechaSubida)
                .HasColumnType("datetime")
                .HasColumnName("fecha_subida");
            entity.Property(e => e.NombreArchivo)
                .HasColumnType("text")
                .HasColumnName("nombre_archivo");
        });

        modelBuilder.Entity<CatalogoEconomico>(entity =>
        {
            entity.HasKey(e => e.IdEconomico).HasName("PRIMARY");

            entity
                .ToTable("catalogo_economicos")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.IdAdministrador, "fk_administrador_economico");

            entity.HasIndex(e => e.IdCombustible, "fk_eco_combustible");

            entity.HasIndex(e => e.IdEstatus, "fk_eco_estatus");

            entity.HasIndex(e => e.IdGrupo, "fk_eco_grupo");

            entity.HasIndex(e => e.IdMarca, "fk_eco_marca");

            entity.HasIndex(e => e.MarcaMotor, "fk_eco_marca_motor");

            entity.HasIndex(e => e.IdTipoEquipo, "fk_eco_tipo_equipo");

            entity.HasIndex(e => e.IdUbicacion, "fk_eco_ubicacion");

            entity.HasIndex(e => e.IdOperador, "fk_operador_economico");

            entity.HasIndex(e => e.IdPropietario, "fk_propietario_economico");

            entity.HasIndex(e => e.IdResponsable, "fk_responsabgle_economico");

            entity.Property(e => e.IdEconomico)
                .HasMaxLength(20)
                .HasColumnName("id_economico");
            entity.Property(e => e.Consecutivo).HasColumnName("consecutivo");
            entity.Property(e => e.Descripcion)
                .HasColumnType("text")
                .HasColumnName("descripcion");
            entity.Property(e => e.Dimensiones)
                .HasColumnType("text")
                .HasColumnName("dimensiones");
            entity.Property(e => e.EstatusSeguro).HasColumnName("estatus_seguro");
            entity.Property(e => e.FamiliaMotor)
                .HasColumnType("text")
                .HasColumnName("familia_motor");
            entity.Property(e => e.GradoPropiedad)
                .HasColumnType("text")
                .HasColumnName("grado_propiedad");
            entity.Property(e => e.Horometro)
                .HasPrecision(10, 2)
                .HasColumnName("horometro");
            entity.Property(e => e.IdAdministrador).HasColumnName("id_administrador");
            entity.Property(e => e.IdCombustible).HasColumnName("id_combustible");
            entity.Property(e => e.IdEstatus).HasColumnName("id_estatus");
            entity.Property(e => e.IdGrupo)
                .HasMaxLength(10)
                .HasColumnName("id_grupo");
            entity.Property(e => e.IdMarca).HasColumnName("id_marca");
            entity.Property(e => e.IdOperador).HasColumnName("id_operador");
            entity.Property(e => e.IdPropietario).HasColumnName("id_propietario");
            entity.Property(e => e.IdResponsable).HasColumnName("id_responsable");
            entity.Property(e => e.IdSamsara)
                .HasColumnType("text")
                .HasColumnName("id_samsara");
            entity.Property(e => e.IdTipoEquipo)
                .HasMaxLength(45)
                .HasColumnName("id_tipo_equipo");
            entity.Property(e => e.IdUbicacion).HasColumnName("id_ubicacion");
            entity.Property(e => e.MarcaMotor).HasColumnName("marca_motor");
            entity.Property(e => e.Modelo)
                .HasColumnType("text")
                .HasColumnName("modelo");
            entity.Property(e => e.ModeloMotor)
                .HasColumnType("text")
                .HasColumnName("modelo_motor");
            entity.Property(e => e.Motor)
                .HasColumnType("text")
                .HasColumnName("motor");
            entity.Property(e => e.ObservacionesAsignaciones)
                .HasColumnType("text")
                .HasColumnName("observaciones_asignaciones");
            entity.Property(e => e.PeriodoFabricacion).HasColumnName("periodo_fabricacion");
            entity.Property(e => e.Placas)
                .HasColumnType("text")
                .HasColumnName("placas");
            entity.Property(e => e.Serie)
                .HasColumnType("text")
                .HasColumnName("serie");
            entity.Property(e => e.SerieMotor)
                .HasColumnType("text")
                .HasColumnName("serie_motor");
            entity.Property(e => e.Thk)
                .HasColumnType("text")
                .HasColumnName("thk");
            entity.Property(e => e.TipoSeguro)
                .HasColumnType("text")
                .HasColumnName("tipo_seguro");
            entity.Property(e => e.ValorAdquisicion)
                .HasPrecision(12, 2)
                .HasColumnName("valor_adquisicion");
            entity.Property(e => e.Verificado).HasColumnName("verificado");

            entity.HasOne(d => d.IdAdministradorNavigation).WithMany(p => p.CatalogoEconomicoIdAdministradorNavigations)
                .HasForeignKey(d => d.IdAdministrador)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_administrador_economico");

            entity.HasOne(d => d.IdCombustibleNavigation).WithMany(p => p.CatalogoEconomicos)
                .HasForeignKey(d => d.IdCombustible)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_eco_combustible");

            entity.HasOne(d => d.IdEstatusNavigation).WithMany(p => p.CatalogoEconomicos)
                .HasForeignKey(d => d.IdEstatus)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_eco_estatus");

            entity.HasOne(d => d.IdGrupoNavigation).WithMany(p => p.CatalogoEconomicos)
                .HasForeignKey(d => d.IdGrupo)
                .HasConstraintName("fk_eco_grupo");

            entity.HasOne(d => d.IdMarcaNavigation).WithMany(p => p.CatalogoEconomicoIdMarcaNavigations)
                .HasForeignKey(d => d.IdMarca)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_eco_marca");

            entity.HasOne(d => d.IdOperadorNavigation).WithMany(p => p.CatalogoEconomicoIdOperadorNavigations)
                .HasForeignKey(d => d.IdOperador)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_operador_economico");

            entity.HasOne(d => d.IdPropietarioNavigation).WithMany(p => p.CatalogoEconomicoIdPropietarioNavigations)
                .HasForeignKey(d => d.IdPropietario)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_propietario_economico");

            entity.HasOne(d => d.IdResponsableNavigation).WithMany(p => p.CatalogoEconomicoIdResponsableNavigations)
                .HasForeignKey(d => d.IdResponsable)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_responsabgle_economico");

            entity.HasOne(d => d.IdTipoEquipoNavigation).WithMany(p => p.CatalogoEconomicos)
                .HasForeignKey(d => d.IdTipoEquipo)
                .HasConstraintName("fk_eco_tipo_equipo");

            entity.HasOne(d => d.IdUbicacionNavigation).WithMany(p => p.CatalogoEconomicos)
                .HasForeignKey(d => d.IdUbicacion)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_eco_ubicacion");

            entity.HasOne(d => d.MarcaMotorNavigation).WithMany(p => p.CatalogoEconomicoMarcaMotorNavigations)
                .HasForeignKey(d => d.MarcaMotor)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_eco_marca_motor");
        });

        modelBuilder.Entity<CatalogoEstatus>(entity =>
        {
            entity.HasKey(e => e.IdEstatus).HasName("PRIMARY");

            entity
                .ToTable("catalogo_estatus")
                .UseCollation("utf8mb4_general_ci");

            entity.Property(e => e.IdEstatus).HasColumnName("id_estatus");
            entity.Property(e => e.DescripcionEstatus)
                .HasColumnType("text")
                .HasColumnName("descripcion_estatus");
        });

        modelBuilder.Entity<CatalogoFrente>(entity =>
        {
            entity.HasKey(e => e.IdFrente).HasName("PRIMARY");

            entity
                .ToTable("catalogo_frentes")
                .UseCollation("utf8mb4_general_ci");

            entity.HasIndex(e => e.IdTramo, "fk_frente_tramo");

            entity.Property(e => e.IdFrente).HasColumnName("id_frente");
            entity.Property(e => e.IdTramo).HasColumnName("id_tramo");
            entity.Property(e => e.NombreFrente)
                .HasColumnType("text")
                .HasColumnName("nombre_frente");

            entity.HasOne(d => d.IdTramoNavigation).WithMany(p => p.CatalogoFrentes)
                .HasForeignKey(d => d.IdTramo)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_frente_tramo");
        });

        modelBuilder.Entity<CatalogoGrupo>(entity =>
        {
            entity.HasKey(e => e.IdGrupo).HasName("PRIMARY");

            entity
                .ToTable("catalogo_grupos")
                .UseCollation("utf8mb4_unicode_ci");

            entity.Property(e => e.IdGrupo)
                .HasMaxLength(10)
                .HasColumnName("id_grupo");
            entity.Property(e => e.DescripcionGrupo)
                .HasColumnType("text")
                .HasColumnName("descripcion_grupo");
        });

        modelBuilder.Entity<CatalogoMarca>(entity =>
        {
            entity.HasKey(e => e.IdMarca).HasName("PRIMARY");

            entity
                .ToTable("catalogo_marcas")
                .UseCollation("utf8mb4_general_ci");

            entity.Property(e => e.IdMarca).HasColumnName("id_marca");
            entity.Property(e => e.NombreMarca)
                .HasColumnType("text")
                .HasColumnName("nombre_marca");
        });

        modelBuilder.Entity<CatalogoMovimientosEconomico>(entity =>
        {
            entity.HasKey(e => e.IdMovimiento).HasName("PRIMARY");

            entity
                .ToTable("catalogo_movimientos_economicos")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.IdEconomico, "fk_economico_movimiento");

            entity.HasIndex(e => e.IdUbicacionLlegada, "fk_ubicacion_llegada_movimiento");

            entity.HasIndex(e => e.IdUbicacionSalida, "fk_ubicacion_salida_movimiento");

            entity.HasIndex(e => e.IdUsuario, "fk_usuario_movimiento");

            entity.Property(e => e.IdMovimiento).HasColumnName("id_movimiento");
            entity.Property(e => e.Archivo)
                .HasColumnType("text")
                .HasColumnName("archivo");
            entity.Property(e => e.Archivo2)
                .HasColumnType("text")
                .HasColumnName("archivo_2");
            entity.Property(e => e.FechaMovimiento)
                .HasColumnType("datetime")
                .HasColumnName("fecha_movimiento");
            entity.Property(e => e.IdEconomico)
                .HasMaxLength(20)
                .HasColumnName("id_economico");
            entity.Property(e => e.IdUbicacionLlegada).HasColumnName("id_ubicacion_llegada");
            entity.Property(e => e.IdUbicacionSalida).HasColumnName("id_ubicacion_salida");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.NombreArchivo)
                .HasColumnType("text")
                .HasColumnName("nombre_archivo");
            entity.Property(e => e.NombreArchivo2)
                .HasColumnType("text")
                .HasColumnName("nombre_archivo_2");
            entity.Property(e => e.UbicacionPersonalizada)
                .HasColumnType("text")
                .HasColumnName("ubicacion_personalizada");

            entity.HasOne(d => d.IdEconomicoNavigation).WithMany(p => p.CatalogoMovimientosEconomicos)
                .HasForeignKey(d => d.IdEconomico)
                .HasConstraintName("fk_economico_movimiento");

            entity.HasOne(d => d.IdUbicacionLlegadaNavigation).WithMany(p => p.CatalogoMovimientosEconomicoIdUbicacionLlegadaNavigations)
                .HasForeignKey(d => d.IdUbicacionLlegada)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_ubicacion_llegada_movimiento");

            entity.HasOne(d => d.IdUbicacionSalidaNavigation).WithMany(p => p.CatalogoMovimientosEconomicoIdUbicacionSalidaNavigations)
                .HasForeignKey(d => d.IdUbicacionSalida)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_ubicacion_salida_movimiento");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.CatalogoMovimientosEconomicos)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_usuario_movimiento");
        });

        modelBuilder.Entity<CatalogoProveedore>(entity =>
        {
            entity.HasKey(e => e.IdProveedor).HasName("PRIMARY");

            entity
                .ToTable("catalogo_proveedores")
                .UseCollation("utf8mb4_general_ci");

            entity.HasIndex(e => e.IdUbicacion, "fk_proveedor_ubicacion");

            entity.Property(e => e.IdProveedor).HasColumnName("id_proveedor");
            entity.Property(e => e.CorreoElectronico)
                .HasColumnType("text")
                .HasColumnName("correo_electronico");
            entity.Property(e => e.IdUbicacion).HasColumnName("id_ubicacion");
            entity.Property(e => e.NumeroTelefonico)
                .HasColumnType("text")
                .HasColumnName("numero_telefonico");
            entity.Property(e => e.RazonSocial)
                .HasColumnType("text")
                .HasColumnName("razon_social");

            entity.HasOne(d => d.IdUbicacionNavigation).WithMany(p => p.CatalogoProveedores)
                .HasForeignKey(d => d.IdUbicacion)
                .HasConstraintName("fk_proveedor_ubicacion");
        });

        modelBuilder.Entity<CatalogoPya>(entity =>
        {
            entity.HasKey(e => e.IdPya).HasName("PRIMARY");

            entity
                .ToTable("catalogo_pya")
                .UseCollation("utf8mb4_general_ci");

            entity.HasIndex(e => e.IdRolPya, "fk_rol_pya");

            entity.Property(e => e.IdPya).HasColumnName("id_pya");
            entity.Property(e => e.IdRolPya).HasColumnName("id_rol_pya");
            entity.Property(e => e.Nombre)
                .HasColumnType("text")
                .HasColumnName("nombre");

            entity.HasOne(d => d.IdRolPyaNavigation).WithMany(p => p.CatalogoPyas)
                .HasForeignKey(d => d.IdRolPya)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_rol_pya");
        });

        modelBuilder.Entity<CatalogoRolPya>(entity =>
        {
            entity.HasKey(e => e.IdRol).HasName("PRIMARY");

            entity
                .ToTable("catalogo_rol_pya")
                .UseCollation("utf8mb4_general_ci");

            entity.Property(e => e.IdRol).HasColumnName("id_rol");
            entity.Property(e => e.DescripcionRol)
                .HasColumnType("text")
                .HasColumnName("descripcion_rol");
        });

        modelBuilder.Entity<CatalogoTiposCombustible>(entity =>
        {
            entity.HasKey(e => e.IdCombustible).HasName("PRIMARY");

            entity
                .ToTable("catalogo_tipos_combustible")
                .UseCollation("utf8mb4_general_ci");

            entity.Property(e => e.IdCombustible).HasColumnName("id_combustible");
            entity.Property(e => e.DescripcionCombustible)
                .HasColumnType("text")
                .HasColumnName("descripcion_combustible");
        });

        modelBuilder.Entity<CatalogoTiposEquipo>(entity =>
        {
            entity.HasKey(e => e.IdTipoEquipo).HasName("PRIMARY");

            entity
                .ToTable("catalogo_tipos_equipos")
                .UseCollation("utf8mb4_unicode_ci");

            entity.Property(e => e.IdTipoEquipo)
                .HasMaxLength(45)
                .HasColumnName("id_tipo_equipo");
            entity.Property(e => e.DescripcionTipoEquipo)
                .HasColumnType("text")
                .HasColumnName("descripcion_tipo_equipo");
        });

        modelBuilder.Entity<CatalogoTramo>(entity =>
        {
            entity.HasKey(e => e.IdTramo).HasName("PRIMARY");

            entity
                .ToTable("catalogo_tramos")
                .UseCollation("utf8mb4_general_ci");

            entity.HasIndex(e => e.IdUbicacion, "fk_ubicacion_tramo");

            entity.Property(e => e.IdTramo).HasColumnName("id_tramo");
            entity.Property(e => e.IdUbicacion).HasColumnName("id_ubicacion");
            entity.Property(e => e.NombreTramo)
                .HasColumnType("text")
                .HasColumnName("nombre_tramo");

            entity.HasOne(d => d.IdUbicacionNavigation).WithMany(p => p.CatalogoTramos)
                .HasForeignKey(d => d.IdUbicacion)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_ubicacion_tramo");
        });

        modelBuilder.Entity<CatalogoUbicacionesProyecto>(entity =>
        {
            entity.HasKey(e => e.IdUbicacion).HasName("PRIMARY");

            entity
                .ToTable("catalogo_ubicaciones_proyectos")
                .UseCollation("utf8mb4_general_ci");

            entity.Property(e => e.IdUbicacion).HasColumnName("id_ubicacion");
            entity.Property(e => e.NombreProyecto)
                .HasColumnType("text")
                .HasColumnName("nombre_proyecto");
            entity.Property(e => e.Siglas)
                .HasMaxLength(50)
                .HasColumnName("siglas");
            entity.Property(e => e.Ubicacion)
                .HasColumnType("text")
                .HasColumnName("ubicacion");
        });

        modelBuilder.Entity<EconomicosArchivo>(entity =>
        {
            entity.HasKey(e => e.IdEconomicoArchivo).HasName("PRIMARY");

            entity
                .ToTable("economicos_archivos")
                .UseCollation("utf8mb4_general_ci");

            entity.HasIndex(e => e.IdArchivo, "fk_archivo_archivoeconomico");

            entity.HasIndex(e => e.IdEconomico, "fk_economico_archivoeconomico");

            entity.Property(e => e.IdEconomicoArchivo).HasColumnName("id_economico_archivo");
            entity.Property(e => e.IdArchivo).HasColumnName("id_archivo");
            entity.Property(e => e.IdEconomico)
                .HasMaxLength(20)
                .HasColumnName("id_economico")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasOne(d => d.IdArchivoNavigation).WithMany(p => p.EconomicosArchivos)
                .HasForeignKey(d => d.IdArchivo)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_archivo_archivoeconomico");

            entity.HasOne(d => d.IdEconomicoNavigation).WithMany(p => p.EconomicosArchivos)
                .HasForeignKey(d => d.IdEconomico)
                .HasConstraintName("fk_archivo_economico");
        });

        modelBuilder.Entity<EfmigrationsHistory>(entity =>
        {
            entity.HasKey(e => e.MigrationId).HasName("PRIMARY");

            entity.ToTable("__EFMigrationsHistory");

            entity.Property(e => e.MigrationId).HasMaxLength(150);
            entity.Property(e => e.ProductVersion).HasMaxLength(32);
        });

        modelBuilder.Entity<Empleado>(entity =>
        {
            entity.HasKey(e => e.NoEmpleado).HasName("PRIMARY");

            entity
                .ToTable("empleados")
                .UseCollation("utf8mb4_general_ci");

            entity.HasIndex(e => e.IdUbicacion, "empleado_ubicacion_fk");

            entity.HasIndex(e => e.IdRolEmpleado, "fk_rolempleado_empleado");

            entity.Property(e => e.NoEmpleado)
                .ValueGeneratedNever()
                .HasColumnName("no_empleado");
            entity.Property(e => e.Ds3).HasColumnName("ds3");
            entity.Property(e => e.IdRolEmpleado).HasColumnName("id_rol_empleado");
            entity.Property(e => e.IdUbicacion).HasColumnName("id_ubicacion");
            entity.Property(e => e.NombreEmpleado)
                .HasMaxLength(255)
                .HasColumnName("nombre_empleado");

            entity.HasOne(d => d.IdRolEmpleadoNavigation).WithMany(p => p.Empleados)
                .HasForeignKey(d => d.IdRolEmpleado)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_rolempleado_empleado");

            entity.HasOne(d => d.IdUbicacionNavigation).WithMany(p => p.Empleados)
                .HasForeignKey(d => d.IdUbicacion)
                .HasConstraintName("empleado_ubicacion_fk");
        });

        modelBuilder.Entity<HistorialLog>(entity =>
        {
            entity.HasKey(e => e.IdLog).HasName("PRIMARY");

            entity
                .ToTable("historial_logs")
                .UseCollation("utf8mb4_general_ci");

            entity.HasIndex(e => e.IdUsuario, "fk_log_usuario");

            entity.Property(e => e.IdLog).HasColumnName("id_log");
            entity.Property(e => e.DescripcionLog)
                .HasColumnType("text")
                .HasColumnName("descripcion_log");
            entity.Property(e => e.FechaLog)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("fecha_log");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.IpAddress)
                .HasMaxLength(45)
                .HasColumnName("ip_address");
            entity.Property(e => e.TipoLog)
                .HasColumnType("text")
                .HasColumnName("tipo_log");
            entity.Property(e => e.UserAgent)
                .HasColumnType("text")
                .HasColumnName("user_agent");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.HistorialLogs)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_log_usuario");
        });

        modelBuilder.Entity<HistorialReporte>(entity =>
        {
            entity.HasKey(e => e.IdReporte).HasName("PRIMARY");

            entity
                .ToTable("historial_reportes")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.IdEconomico, "fk_reporte_economico");

            entity.HasIndex(e => e.IdNuevoEstatus, "fk_reporte_nuevo_estatus");

            entity.HasIndex(e => e.IdTipoReporte, "fk_reporte_tipo_reporte");

            entity.HasIndex(e => e.IdUbicacion, "fk_reporte_ubicacion");

            entity.HasIndex(e => e.IdUsuario, "fk_reporte_usuario");

            entity.Property(e => e.IdReporte)
                .HasMaxLength(15)
                .HasColumnName("id_reporte");
            entity.Property(e => e.DescripcionReporte)
                .HasColumnType("text")
                .HasColumnName("descripcion_reporte");
            entity.Property(e => e.Horometro).HasColumnName("horometro");
            entity.Property(e => e.IdEconomico)
                .HasMaxLength(20)
                .HasColumnName("id_economico");
            entity.Property(e => e.IdNuevoEstatus).HasColumnName("id_nuevo_estatus");
            entity.Property(e => e.IdTipoReporte).HasColumnName("id_tipo_reporte");
            entity.Property(e => e.IdUbicacion).HasColumnName("id_ubicacion");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.PrioridadAtencion)
                .HasColumnType("text")
                .HasColumnName("prioridad_atencion");
            entity.Property(e => e.UbicacionDetalle)
                .HasColumnType("text")
                .HasColumnName("ubicacion_detalle");

            entity.HasOne(d => d.IdEconomicoNavigation).WithMany(p => p.HistorialReportes)
                .HasForeignKey(d => d.IdEconomico)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_reporte_economico");

            entity.HasOne(d => d.IdNuevoEstatusNavigation).WithMany(p => p.HistorialReportes)
                .HasForeignKey(d => d.IdNuevoEstatus)
                .HasConstraintName("fk_reporte_nuevo_estatus");

            entity.HasOne(d => d.IdTipoReporteNavigation).WithMany(p => p.HistorialReportes)
                .HasForeignKey(d => d.IdTipoReporte)
                .HasConstraintName("fk_reporte_tipo_reporte");

            entity.HasOne(d => d.IdUbicacionNavigation).WithMany(p => p.HistorialReportes)
                .HasForeignKey(d => d.IdUbicacion)
                .HasConstraintName("fk_reporte_ubicacion");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.HistorialReportes)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("fk_reporte_usuario");
        });

        modelBuilder.Entity<HistorialServicio>(entity =>
        {
            entity.HasKey(e => e.IdServicio).HasName("PRIMARY");

            entity
                .ToTable("historial_servicio")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.NoEconomico, "fk_servicio_economico");

            entity.Property(e => e.IdServicio).HasColumnName("id_servicio");
            entity.Property(e => e.Anotaciones)
                .HasColumnType("text")
                .HasColumnName("anotaciones");
            entity.Property(e => e.FechaMantenimiento).HasColumnName("fecha_mantenimiento");
            entity.Property(e => e.Horaskilometrosreales)
                .HasColumnType("text")
                .HasColumnName("horaskilometrosreales");
            entity.Property(e => e.NoEconomico)
                .HasMaxLength(20)
                .HasColumnName("no_economico");
            entity.Property(e => e.TipoMantenimiento)
                .HasMaxLength(50)
                .HasColumnName("tipo_mantenimiento");

            entity.HasOne(d => d.NoEconomicoNavigation).WithMany(p => p.HistorialServicios)
                .HasForeignKey(d => d.NoEconomico)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_servicio_economico");
        });

        modelBuilder.Entity<Requisicione>(entity =>
        {
            entity.HasKey(e => e.IdRequisicion).HasName("PRIMARY");

            entity
                .ToTable("requisiciones")
                .UseCollation("utf8mb4_general_ci");

            entity.HasIndex(e => e.IdAtencion, "fk_requisicion_atencion");

            entity.HasIndex(e => e.IdAutorizante, "fk_usuario_autorizasnte_req");

            entity.HasIndex(e => e.IdSolicitante, "fk_usuario_colicitante_req");

            entity.Property(e => e.IdRequisicion)
                .HasMaxLength(100)
                .HasColumnName("id_requisicion");
            entity.Property(e => e.ArchivoReq)
                .HasColumnType("text")
                .HasColumnName("archivo_req");
            entity.Property(e => e.Consecutivo).HasColumnName("consecutivo");
            entity.Property(e => e.Empresa)
                .HasMaxLength(5)
                .HasColumnName("empresa");
            entity.Property(e => e.Estatus)
                .HasMaxLength(50)
                .HasColumnName("estatus");
            entity.Property(e => e.FechaRequisicion).HasColumnName("fecha_requisicion");
            entity.Property(e => e.IdAtencion).HasColumnName("id_atencion");
            entity.Property(e => e.IdAutorizante).HasColumnName("id_autorizante");
            entity.Property(e => e.IdSolicitante).HasColumnName("id_solicitante");
            entity.Property(e => e.IdUbicacion).HasColumnName("id_ubicacion");
            entity.Property(e => e.TipoRequisicion)
                .HasMaxLength(50)
                .HasColumnName("tipo_requisicion");

            entity.HasOne(d => d.IdAtencionNavigation).WithMany(p => p.RequisicioneIdAtencionNavigations)
                .HasForeignKey(d => d.IdAtencion)
                .HasConstraintName("fk_requisicion_atencion");

            entity.HasOne(d => d.IdAutorizanteNavigation).WithMany(p => p.RequisicioneIdAutorizanteNavigations)
                .HasForeignKey(d => d.IdAutorizante)
                .HasConstraintName("fk_usuario_autorizasnte_req");

            entity.HasOne(d => d.IdSolicitanteNavigation).WithMany(p => p.RequisicioneIdSolicitanteNavigations)
                .HasForeignKey(d => d.IdSolicitante)
                .HasConstraintName("fk_usuario_colicitante_req");
        });

        modelBuilder.Entity<RolEmpleado>(entity =>
        {
            entity.HasKey(e => e.IdRolEmpleado).HasName("PRIMARY");

            entity
                .ToTable("rol_empleado")
                .UseCollation("utf8mb4_general_ci");

            entity.Property(e => e.IdRolEmpleado).HasColumnName("id_rol_empleado");
            entity.Property(e => e.DescripcionRol)
                .HasMaxLength(255)
                .HasColumnName("descripcion_rol");
        });

        modelBuilder.Entity<ServicioArchivo>(entity =>
        {
            entity.HasKey(e => e.IdServicioArchivo).HasName("PRIMARY");

            entity.ToTable("servicio_archivos");

            entity.HasIndex(e => e.IdArchivo, "fk_archivo_servicio");

            entity.HasIndex(e => e.IdServicio, "fk_servicio_archivo");

            entity.Property(e => e.IdServicioArchivo).HasColumnName("id_servicio_archivo");
            entity.Property(e => e.IdArchivo).HasColumnName("id_archivo");
            entity.Property(e => e.IdServicio).HasColumnName("id_servicio");

            entity.HasOne(d => d.IdArchivoNavigation).WithMany(p => p.ServicioArchivos)
                .HasForeignKey(d => d.IdArchivo)
                .HasConstraintName("fk_archivo_servicio");

            entity.HasOne(d => d.IdServicioNavigation).WithMany(p => p.ServicioArchivos)
                .HasForeignKey(d => d.IdServicio)
                .HasConstraintName("fk_servicio_archivo");
        });

        modelBuilder.Entity<TiposReporte>(entity =>
        {
            entity.HasKey(e => e.IdTipoReporte).HasName("PRIMARY");

            entity.ToTable("tipos_reportes");

            entity.Property(e => e.IdTipoReporte).HasColumnName("id_tipo_reporte");
            entity.Property(e => e.DescripcionTipoReporte)
                .HasColumnType("text")
                .HasColumnName("descripcion_tipo_reporte");
            entity.Property(e => e.Siglas)
                .HasColumnType("text")
                .HasColumnName("siglas");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("PRIMARY");

            entity
                .ToTable("usuarios")
                .UseCollation("utf8mb4_general_ci");

            entity.HasIndex(e => e.IdRol, "fk_usuarios_roles");

            entity.HasIndex(e => e.NoEmpleado, "usuario_empleado_fk");

            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.Area)
                .HasColumnType("text")
                .HasColumnName("area");
            entity.Property(e => e.Correoe)
                .HasColumnType("text")
                .HasColumnName("correoe");
            entity.Property(e => e.FirmaPath)
                .HasMaxLength(255)
                .HasColumnName("firma_path");
            entity.Property(e => e.IdRol).HasColumnName("id_rol");
            entity.Property(e => e.NoEmpleado).HasColumnName("no_empleado");
            entity.Property(e => e.NombreCompleto)
                .HasColumnType("text")
                .HasColumnName("nombre_completo");
            entity.Property(e => e.NombreUsuario)
                .HasColumnType("text")
                .HasColumnName("nombre_usuario");
            entity.Property(e => e.Password)
                .HasColumnType("text")
                .HasColumnName("password");

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdRol)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_usuarios_roles");

            entity.HasOne(d => d.NoEmpleadoNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.NoEmpleado)
                .HasConstraintName("usuario_empleado_fk");
        });

        modelBuilder.Entity<UsuariosRole>(entity =>
        {
            entity.HasKey(e => e.IdRol).HasName("PRIMARY");

            entity
                .ToTable("usuarios_roles")
                .UseCollation("utf8mb4_general_ci");

            entity.Property(e => e.IdRol).HasColumnName("id_rol");
            entity.Property(e => e.DescripcionRol)
                .HasColumnType("text")
                .HasColumnName("descripcion_rol");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
