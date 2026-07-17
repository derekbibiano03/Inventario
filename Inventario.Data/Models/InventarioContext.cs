using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

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

    public virtual DbSet<CatalogoArchivo> CatalogoArchivos { get; set; }

    public virtual DbSet<CatalogoBroker> CatalogoBrokers { get; set; }

    public virtual DbSet<CatalogoEconomico> CatalogoEconomicos { get; set; }

    public virtual DbSet<CatalogoEstatus> CatalogoEstatuses { get; set; }

    public virtual DbSet<CatalogoFrente> CatalogoFrentes { get; set; }

    public virtual DbSet<CatalogoGrupo> CatalogoGrupos { get; set; }

    public virtual DbSet<CatalogoMarca> CatalogoMarcas { get; set; }

    public virtual DbSet<CatalogoMovimientosEconomico> CatalogoMovimientosEconomicos { get; set; }

    public virtual DbSet<CatalogoOperadore> CatalogoOperadores { get; set; }

    public virtual DbSet<CatalogoPya> CatalogoPyas { get; set; }

    public virtual DbSet<CatalogoResponsableMaquinarium> CatalogoResponsableMaquinaria { get; set; }

    public virtual DbSet<CatalogoRolPya> CatalogoRolPyas { get; set; }

    public virtual DbSet<CatalogoTiposCombustible> CatalogoTiposCombustibles { get; set; }

    public virtual DbSet<CatalogoTiposEquipo> CatalogoTiposEquipos { get; set; }

    public virtual DbSet<HistorialLogs> HistoriaLogs { get; set; }

    public virtual DbSet<CatalogoTiposServicio> CatalogoTiposServicios { get; set; }

    public virtual DbSet<CatalogoTramo> CatalogoTramos { get; set; }

    public virtual DbSet<CatalogoUbicacionesProyecto> CatalogoUbicacionesProyectos { get; set; }

    public virtual DbSet<EconomicosArchivo> EconomicosArchivos { get; set; }

    public virtual DbSet<ServiciosEconomico> ServiciosEconomicos { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    public virtual DbSet<UsuariosRole> UsuariosRoles { get; set; }

    public virtual DbSet<UsuriosRole> UsuriosRoles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=192.168.0.24;Database=inventario;Username=admin_maestro;Password=7542gTFn45_ADM");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        // Configuración detallada del mapeo de la entidad HistorialLogs con la tabla de auditoría en PostgreSQL
        modelBuilder.Entity<HistorialLogs>(entity =>
        {
            // Vincula el modelo de C# directamente con la tabla física 'historial_logs' en PostgreSQL
            entity.ToTable("historial_logs");

            // Establece la propiedad IdLog como la llave primaria del modelo de logs
            entity.HasKey(e => e.IdLog);

            // Mapea la propiedad IdLog con la columna física 'id_log' y la configura como autoincrementable por defecto
            entity.Property(e => e.IdLog)
                .HasColumnName("id_log")
                .UseIdentityByDefaultColumn();

            // Mapea la propiedad DescripcionLog con la columna física 'descripcion_log' que almacena el texto del evento
            entity.Property(e => e.DescripcionLog)
                .HasColumnName("descripcion_log");

            // Mapea la propiedad TipoLog con la columna física 'tipo_log' para clasificar la categoría (ej. 'LOGIN')
            entity.Property(e => e.TipoLog)
                .HasColumnName("tipo_log");

            // Mapea la propiedad IdUsuario con la columna física 'id_usuario' que relaciona al autor del evento
            entity.Property(e => e.IdUsuario)
                .HasColumnName("id_usuario");

            // Mapea la propiedad FechaLog con la columna física 'fecha_log' que registra el momento exacto del suceso
            entity.Property(e => e.FechaLog)
                .HasColumnName("fecha_log");

            // Mapea la propiedad IpAddress con la columna física 'ip_address' forzando el tipo de dato de red 'inet' de PostgreSQL
            entity.Property(e => e.IpAddress)
                .HasColumnName("ip_address")
                .HasColumnType("inet");

            // Establece de forma explícita la relación de clave foránea entre HistorialLogs (hijo) y Usuario (padre)
            // indicando que un objeto Usuario tiene muchos HistorialLogs (colección mapeada en el modelo de Usuario)
            entity.HasOne(d => d.IdUsuarioNavigation)
                  .WithMany(p => p.HistorialLogs)
                  .HasForeignKey(d => d.IdUsuario)
                  .HasConstraintName("fk_usuario");
        });

        modelBuilder.Entity<CatalogoArchivo>(entity =>
        {
            entity.HasKey(e => e.IdArchivo).HasName("catalogo_archivos_pkey");

            entity.ToTable("catalogo_archivos");

            entity.Property(e => e.IdArchivo)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id_archivo");
            entity.Property(e => e.Archivo).HasColumnName("archivo");
            entity.Property(e => e.FechaSubida).HasColumnName("fecha_subida");
            entity.Property(e => e.NombreArchivo)
                .HasMaxLength(50)
                .HasColumnName("nombre_archivo");
        });

        modelBuilder.Entity<CatalogoBroker>(entity =>
        {
            entity.HasKey(e => e.IdBroker).HasName("catalogo_brokers_pkey");

            entity.ToTable("catalogo_brokers");

            entity.Property(e => e.IdBroker)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id_broker");
            entity.Property(e => e.NombreBroker).HasColumnName("nombre_broker");
        });

        modelBuilder.Entity<CatalogoEconomico>(entity =>
        {
            entity.HasKey(e => e.IdEconomico).HasName("catalogo_economicos_pkey");

            entity.ToTable("catalogo_economicos");

            entity.HasIndex(e => e.IdAdministrador, "IX_catalogo_economicos_id_administrador");

            entity.HasIndex(e => e.IdCombustible, "IX_catalogo_economicos_id_combustible");

            entity.HasIndex(e => e.IdEstatus, "IX_catalogo_economicos_id_estatus");

            entity.HasIndex(e => e.IdGrupo, "IX_catalogo_economicos_id_grupo");

            entity.HasIndex(e => e.IdOperador, "IX_catalogo_economicos_id_operador");

            entity.HasIndex(e => e.IdPropietario, "IX_catalogo_economicos_id_propietario");

            entity.HasIndex(e => e.IdResponsable, "IX_catalogo_economicos_id_responsable");

            entity.HasIndex(e => e.IdTipoEquipo, "IX_catalogo_economicos_id_tipo_equipo");

            entity.HasIndex(e => e.IdUbicacion, "IX_catalogo_economicos_id_ubicacion");

            entity.Property(e => e.IdEconomico)
                .HasMaxLength(20)
                .HasColumnName("id_economico")
                .ValueGeneratedOnAdd();
            entity.Property(e => e.Consecutivo).HasColumnName("consecutivo");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
            entity.Property(e => e.EstatusSeguro).HasColumnName("estatus_seguro");
            entity.Property(e => e.FamiliaMotor).HasColumnName("familia_motor");
            entity.Property(e => e.GradoPropiedad).HasColumnName("grado_propiedad");
            entity.Property(e => e.Horometro).HasColumnName("horometro");
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
            entity.Property(e => e.IdTipoEquipo)
                .HasMaxLength(45)
                .HasColumnName("id_tipo_equipo");
            entity.Property(e => e.IdUbicacion).HasColumnName("id_ubicacion");
            entity.Property(e => e.MarcaMotor).HasColumnName("marca_motor");
            entity.Property(e => e.Modelo).HasColumnName("modelo");
            entity.Property(e => e.ModeloMotor).HasColumnName("modelo_motor");
            entity.Property(e => e.Motor).HasColumnName("motor");
            entity.Property(e => e.ObservacionesAsignaciones).HasColumnName("observaciones_asignaciones");
            entity.Property(e => e.PeriodoFabricacion).HasColumnName("periodo_fabricacion");
            entity.Property(e => e.Placas).HasColumnName("placas");
            entity.Property(e => e.PolizaAdjunta).HasColumnName("poliza_adjunta");
            entity.Property(e => e.Serie).HasColumnName("serie");
            entity.Property(e => e.SerieMotor).HasColumnName("serie_motor");
            entity.Property(e => e.THK).HasColumnName("THK");

            entity.HasOne(d => d.IdAdministradorNavigation).WithMany(p => p.CatalogoEconomicoIdAdministradorNavigations)
                .HasForeignKey(d => d.IdAdministrador)
                .HasConstraintName("fk_administrador_economico");

            entity.HasOne(d => d.IdCombustibleNavigation).WithMany(p => p.CatalogoEconomicos)
                .HasForeignKey(d => d.IdCombustible)
                .HasConstraintName("fk_combustible_economico");

            entity.HasOne(d => d.IdEstatusNavigation).WithMany(p => p.CatalogoEconomicos)
                .HasForeignKey(d => d.IdEstatus)
                .HasConstraintName("fk_estatus_economico");

            entity.HasOne(d => d.IdGrupoNavigation).WithMany(p => p.CatalogoEconomicos)
                .HasForeignKey(d => d.IdGrupo)
                .HasConstraintName("fk_grupo_economico");

            entity.HasOne(d => d.IdMarcaNavigation).WithMany(p => p.CatalogoEconomicoIdMarcaNavigations)
                .HasForeignKey(d => d.IdMarca)
                .HasConstraintName("fk_marca_economico");

            entity.HasOne(d => d.IdOperadorNavigation).WithMany(p => p.CatalogoEconomicos)
                .HasForeignKey(d => d.IdOperador)
                .HasConstraintName("fk_operador_economico");

            entity.HasOne(d => d.IdPropietarioNavigation).WithMany(p => p.CatalogoEconomicoIdPropietarioNavigations)
                .HasForeignKey(d => d.IdPropietario)
                .HasConstraintName("fk_propietario_economico");

            entity.HasOne(d => d.IdResponsableNavigation).WithMany(p => p.CatalogoEconomicos)
                .HasForeignKey(d => d.IdResponsable)
                .HasConstraintName("fk_responsable_economico");

            entity.HasOne(d => d.IdTipoEquipoNavigation).WithMany(p => p.CatalogoEconomicos)
                .HasForeignKey(d => d.IdTipoEquipo)
                .HasConstraintName("fk_tipo_equipo_economico");

            entity.HasOne(d => d.IdUbicacionNavigation).WithMany(p => p.CatalogoEconomicos)
                .HasForeignKey(d => d.IdUbicacion)
                .HasConstraintName("fk_ubicacion_economico");

            entity.HasOne(d => d.MarcaMotorNavigation).WithMany(p => p.CatalogoEconomicoMarcaMotorNavigations)
                .HasForeignKey(d => d.MarcaMotor)
                .HasConstraintName("fk_marcamotor_economico");
        });

        modelBuilder.Entity<CatalogoEstatus>(entity =>
        {
            entity.HasKey(e => e.IdEstatus).HasName("catalogo_estatus_pkey");

            entity.ToTable("catalogo_estatus");

            entity.Property(e => e.IdEstatus)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id_estatus");
            entity.Property(e => e.DescripcionEstatus).HasColumnName("descripcion_estatus");
        });

        modelBuilder.Entity<CatalogoFrente>(entity =>
        {
            entity.HasKey(e => e.IdFrente).HasName("catalogo_frentes_pkey");

            entity.ToTable("catalogo_frentes");

            entity.HasIndex(e => e.IdTramo, "IX_catalogo_frentes_id_tramo");

            entity.Property(e => e.IdFrente)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id_frente");
            entity.Property(e => e.IdTramo).HasColumnName("id_tramo");
            entity.Property(e => e.NombreFrente).HasColumnName("nombre_frente");

            entity.HasOne(d => d.IdTramoNavigation).WithMany(p => p.CatalogoFrentes)
                .HasForeignKey(d => d.IdTramo)
                .HasConstraintName("fk_ubicacion");
        });

        modelBuilder.Entity<CatalogoGrupo>(entity =>
        {
            entity.HasKey(e => e.IdGrupo).HasName("catalogo_grupos_pkey");

            entity.ToTable("catalogo_grupos");

            entity.Property(e => e.IdGrupo)
                .HasMaxLength(10)
                .HasColumnName("id_grupo");
            entity.Property(e => e.DescripcionGrupo).HasColumnName("descripcion_grupo");
        });

        modelBuilder.Entity<CatalogoMarca>(entity =>
        {
            entity.HasKey(e => e.IdMarca).HasName("catalogo_marcas_pkey");

            entity.ToTable("catalogo_marcas");

            entity.Property(e => e.IdMarca)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id_marca");
            entity.Property(e => e.NombreMarca).HasColumnName("nombre_marca");
        });

        modelBuilder.Entity<CatalogoMovimientosEconomico>(entity =>
        {
            entity.HasKey(e => e.IdMovimiento).HasName("catalogo_movimientos_economicos_pkey");

            entity.ToTable("catalogo_movimientos_economicos");

            entity.HasIndex(e => e.IdEconomico, "IX_catalogo_movimientos_economicos_id_economico");

            entity.HasIndex(e => e.IdUbicacionLlegada, "IX_catalogo_movimientos_economicos_id_ubicacion_llegada");

            entity.HasIndex(e => e.IdUbicacionSalida, "IX_catalogo_movimientos_economicos_id_ubicacion_salida");

            entity.Property(e => e.IdMovimiento)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id_movimiento");
            entity.Property(e => e.IdEconomico)
                .HasMaxLength(20)
                .HasColumnName("id_economico");
            entity.Property(e => e.IdUbicacionLlegada).HasColumnName("id_ubicacion_llegada");
            entity.Property(e => e.IdUbicacionSalida).HasColumnName("id_ubicacion_salida");
            entity.Property(e => e.UbicacionPersonalizada).HasColumnName("ubicacion_personalizada");

            entity.HasOne(d => d.IdEconomicoNavigation).WithMany(p => p.CatalogoMovimientosEconomicos)
                .HasForeignKey(d => d.IdEconomico)
                .HasConstraintName("fk_economico_ubicacion");

            entity.HasOne(d => d.IdUbicacionLlegadaNavigation).WithMany(p => p.CatalogoMovimientosEconomicoIdUbicacionLlegadaNavigations)
                .HasForeignKey(d => d.IdUbicacionLlegada)
                .HasConstraintName("fk_llegada_proyecto_ubicacion");

            entity.HasOne(d => d.IdUbicacionSalidaNavigation).WithMany(p => p.CatalogoMovimientosEconomicoIdUbicacionSalidaNavigations)
                .HasForeignKey(d => d.IdUbicacionSalida)
                .HasConstraintName("fk_salida_proyecto_ubicacion");
        });

        modelBuilder.Entity<CatalogoOperadore>(entity =>
        {
            entity.HasKey(e => e.IdOperador).HasName("catalogo_operadores_pkey");

            entity.ToTable("catalogo_operadores");

            entity.Property(e => e.IdOperador)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id_operador");
            entity.Property(e => e.NombreOperador).HasColumnName("nombre_operador");
        });

        modelBuilder.Entity<CatalogoPya>(entity =>
        {
            entity.HasKey(e => e.IdPya).HasName("catalogo_pya_pkey");

            entity.ToTable("catalogo_pya");

            entity.HasIndex(e => e.IdRolPya, "IX_catalogo_pya_id_rol_pya");

            entity.Property(e => e.IdPya)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id_pya");
            entity.Property(e => e.IdRolPya).HasColumnName("id_rol_pya");
            entity.Property(e => e.Nombre).HasColumnName("nombre");

            entity.HasOne(d => d.IdRolPyaNavigation).WithMany(p => p.CatalogoPyas)
                .HasForeignKey(d => d.IdRolPya)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_rol_pya");
        });

        modelBuilder.Entity<CatalogoResponsableMaquinarium>(entity =>
        {
            entity.HasKey(e => e.IdResponsable).HasName("catalogo_responsable_maquinaria_pkey");

            entity.ToTable("catalogo_responsable_maquinaria");

            entity.Property(e => e.IdResponsable)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id_responsable");
            entity.Property(e => e.NombreResponsable).HasColumnName("nombre_responsable");
        });

        modelBuilder.Entity<CatalogoRolPya>(entity =>
        {
            entity.HasKey(e => e.IdRol).HasName("catalogo_rol_pya_pkey");

            entity.ToTable("catalogo_rol_pya");

            entity.Property(e => e.IdRol)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id_rol");
            entity.Property(e => e.DescripcionRol).HasColumnName("descripcion_rol");
        });

        modelBuilder.Entity<CatalogoTiposCombustible>(entity =>
        {
            entity.HasKey(e => e.IdCombustible).HasName("catalogo_tipos_combustible_pkey");

            entity.ToTable("catalogo_tipos_combustible");

            entity.Property(e => e.IdCombustible)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id_combustible");
            entity.Property(e => e.DescripcionCombustible).HasColumnName("descripcion_combustible");
        });

        modelBuilder.Entity<CatalogoTiposEquipo>(entity =>
        {
            entity.HasKey(e => e.IdTipoEquipo).HasName("catalogo_tipos_equipos_pkey");

            entity.ToTable("catalogo_tipos_equipos");

            entity.Property(e => e.IdTipoEquipo)
                .HasMaxLength(45)
                .HasColumnName("id_tipo_equipo");
            entity.Property(e => e.DescripcionTipoEquipo).HasColumnName("descripcion_tipo_equipo");
        });

        modelBuilder.Entity<CatalogoTiposServicio>(entity =>
        {
            entity.HasKey(e => e.IdTipoServicio).HasName("catalogo_tipos_servicios_pkey");

            entity.ToTable("catalogo_tipos_servicios");

            entity.Property(e => e.IdTipoServicio)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id_tipo_servicio");
            entity.Property(e => e.DescripcionServicio).HasColumnName("descripcion_servicio");
        });

        modelBuilder.Entity<CatalogoTramo>(entity =>
        {
            entity.HasKey(e => e.IdTramo).HasName("catalogo_tramos_pkey");

            entity.ToTable("catalogo_tramos");

            entity.HasIndex(e => e.IdUbicacion, "IX_catalogo_tramos_id_ubicacion");

            entity.Property(e => e.IdTramo)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id_tramo");
            entity.Property(e => e.IdUbicacion).HasColumnName("id_ubicacion");
            entity.Property(e => e.NombreTramo).HasColumnName("nombre_tramo");

            entity.HasOne(d => d.IdUbicacionNavigation).WithMany(p => p.CatalogoTramos)
                .HasForeignKey(d => d.IdUbicacion)
                .HasConstraintName("fk_ubicacion");
        });

        modelBuilder.Entity<CatalogoUbicacionesProyecto>(entity =>
        {
            entity.HasKey(e => e.IdUbicacion).HasName("catalogo_ubicaciones_proyectos_pkey");

            entity.ToTable("catalogo_ubicaciones_proyectos");

            entity.Property(e => e.IdUbicacion)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id_ubicacion");
            entity.Property(e => e.NombreProyecto).HasColumnName("nombre_proyecto");
            entity.Property(e => e.Ubicacion).HasColumnName("ubicacion");
        });

        modelBuilder.Entity<EconomicosArchivo>(entity =>
        {
            entity.HasKey(e => e.IdEconomicoArchivo).HasName("archivos_economicos_pkey");

            entity.ToTable("economicos_archivos");

            entity.HasIndex(e => e.IdArchivo, "IX_economicos_archivos_id_archivo");

            entity.HasIndex(e => e.IdEconomico, "IX_economicos_archivos_id_economico");

            entity.Property(e => e.IdEconomicoArchivo).HasColumnName("id_economico_archivo");
            entity.Property(e => e.IdArchivo).HasColumnName("id_archivo");
            entity.Property(e => e.IdEconomico)
                .HasMaxLength(20)
                .HasColumnName("id_economico");

            entity.HasOne(d => d.IdArchivoNavigation).WithMany(p => p.EconomicosArchivos)
                .HasForeignKey(d => d.IdArchivo)
                .HasConstraintName("fk_archivo_economico");

            entity.HasOne(d => d.IdEconomicoNavigation).WithMany(p => p.EconomicosArchivos)
                .HasForeignKey(d => d.IdEconomico)
                .HasConstraintName("fk_economico_archivo");
        });

        modelBuilder.Entity<ServiciosEconomico>(entity =>
        {
            entity.HasKey(e => e.IdServicioEconomico).HasName("servicios_economicos_pkey");

            entity.ToTable("servicios_economicos");

            entity.HasIndex(e => e.IdEconomico, "IX_servicios_economicos_id_economico");

            entity.HasIndex(e => e.IdTipoServicio, "IX_servicios_economicos_id_tipo_servicio");

            entity.Property(e => e.IdServicioEconomico)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id_servicio_economico");
            entity.Property(e => e.FechaServicio).HasColumnName("fecha_servicio");
            entity.Property(e => e.Horometro).HasColumnName("horometro");
            entity.Property(e => e.IdEconomico)
                .HasMaxLength(20)
                .HasColumnName("id_economico");
            entity.Property(e => e.IdTipoServicio).HasColumnName("id_tipo_servicio");
            entity.Property(e => e.Kilometraje).HasColumnName("kilometraje");

            entity.HasOne(d => d.IdEconomicoNavigation).WithMany(p => p.ServiciosEconomicos)
                .HasForeignKey(d => d.IdEconomico)
                .HasConstraintName("fk_economico_servicio");

            entity.HasOne(d => d.IdTipoServicioNavigation).WithMany(p => p.ServiciosEconomicos)
                .HasForeignKey(d => d.IdTipoServicio)
                .HasConstraintName("fk_servicio_economico");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("usuarios_pkey");

            entity.ToTable("usuarios");

            entity.Property(e => e.IdUsuario)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id_usuario");
            entity.Property(e => e.IdRol).HasColumnName("id_rol");
            entity.Property(e => e.NombreUsuario).HasColumnName("nombre_usuario");
            entity.Property(e => e.Password).HasColumnName("password");

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdRol)
                .HasConstraintName("fk_usuario_rol");
        });

        modelBuilder.Entity<UsuariosRole>(entity =>
        {
            entity.HasKey(e => e.IdRol).HasName("usurios_roles_pkey");

            entity.ToTable("usuarios_roles");

            entity.Property(e => e.IdRol)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id_rol");
            entity.Property(e => e.DescripcionRol).HasColumnName("descripcion_rol");
        });

        modelBuilder.Entity<UsuriosRole>(entity =>
        {
            entity.HasKey(e => e.IdLog).HasName("usurios_roles_pkey1");

            entity.ToTable("usurios_roles");

            entity.Property(e => e.IdLog)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id_log");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.InformacionLog).HasColumnName("informacion_log");
            entity.Property(e => e.TipoLog).HasColumnName("tipo_log");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.UsuriosRoles)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("fk_log_usuario");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}