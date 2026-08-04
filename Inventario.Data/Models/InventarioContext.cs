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

    public virtual DbSet<CatalogoHerramientasMateriale> CatalogoHerramientasMateriales { get; set; }

    public virtual DbSet<CatalogoMarca> CatalogoMarcas { get; set; }

    public virtual DbSet<CatalogoMovimientosEconomico> CatalogoMovimientosEconomicos { get; set; }

    public virtual DbSet<CatalogoProveedore> CatalogoProveedores { get; set; }

    public virtual DbSet<CatalogoPya> CatalogoPyas { get; set; }

    public virtual DbSet<CatalogoRolPya> CatalogoRolPyas { get; set; }

    public virtual DbSet<CatalogoTipoAdquisicion> CatalogoTipoAdquisicions { get; set; }

    public virtual DbSet<CatalogoTipoMaterial> CatalogoTipoMaterials { get; set; }

    public virtual DbSet<CatalogoTiposCombustible> CatalogoTiposCombustibles { get; set; }

    public virtual DbSet<CatalogoTiposEquipo> CatalogoTiposEquipos { get; set; }

    public virtual DbSet<CatalogoTiposServicio> CatalogoTiposServicios { get; set; }

    public virtual DbSet<CatalogoTramo> CatalogoTramos { get; set; }

    public virtual DbSet<CatalogoUbicacionesProyecto> CatalogoUbicacionesProyectos { get; set; }

    public virtual DbSet<EconomicosArchivo> EconomicosArchivos { get; set; }

    public virtual DbSet<Empleado> Empleados { get; set; }

    public virtual DbSet<HistorialLog> HistorialLogs { get; set; }

    public virtual DbSet<Requisicione> Requisiciones { get; set; }

    public virtual DbSet<RolEmpleado> RolEmpleados { get; set; }

    public virtual DbSet<ServiciosEconomico> ServiciosEconomicos { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    public virtual DbSet<UsuariosRole> UsuariosRoles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=192.168.0.24;Port=5432;Database=inventario;Username=admin_maestro;Password=7542gTFn45_ADM;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CatalogoArchivo>(entity =>
        {
            entity.HasKey(e => e.IdArchivo).HasName("catalogo_archivos_pkey");

            entity.ToTable("catalogo_archivos");

            entity.Property(e => e.IdArchivo)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id_archivo");
            entity.Property(e => e.Archivo).HasColumnName("archivo");
            entity.Property(e => e.FechaSubida).HasColumnName("fecha_subida");
            entity.Property(e => e.NombreArchivo).HasColumnName("nombre_archivo");
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

            entity.HasIndex(e => e.IdPropietario, "IX_catalogo_economicos_id_propietario");

            entity.HasIndex(e => e.IdTipoEquipo, "IX_catalogo_economicos_id_tipo_equipo");

            entity.HasIndex(e => e.IdUbicacion, "IX_catalogo_economicos_id_ubicacion");

            entity.Property(e => e.IdEconomico)
                .HasMaxLength(20)
                .HasColumnName("id_economico");
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
            entity.Property(e => e.Thk).HasColumnName("THK");
            entity.Property(e => e.TipoSeguro).HasColumnName("tipo_seguro");
            entity.Property(e => e.ValorAdquisicion).HasColumnName("valor_adquisicion");
            entity.Property(e => e.Verificado).HasColumnName("verificado");

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

            entity.HasOne(d => d.IdOperadorNavigation).WithMany(p => p.CatalogoEconomicoIdOperadorNavigations)
                .HasForeignKey(d => d.IdOperador)
                .HasConstraintName("fk_operador_empleado");

            entity.HasOne(d => d.IdPropietarioNavigation).WithMany(p => p.CatalogoEconomicoIdPropietarioNavigations)
                .HasForeignKey(d => d.IdPropietario)
                .HasConstraintName("fk_propietario_economico");

            entity.HasOne(d => d.IdResponsableNavigation).WithMany(p => p.CatalogoEconomicoIdResponsableNavigations)
                .HasForeignKey(d => d.IdResponsable)
                .HasConstraintName("fk_responsable_empleado");

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

        modelBuilder.Entity<CatalogoHerramientasMateriale>(entity =>
        {
            entity.HasKey(e => e.IdAdsquisicion).HasName("catalogo_herramientas_materiales_pkey");

            entity.ToTable("catalogo_herramientas_materiales");

            entity.Property(e => e.IdAdsquisicion).HasColumnName("id_adsquisicion");
            entity.Property(e => e.Cantidad).HasColumnName("cantidad");
            entity.Property(e => e.IdPersonaResguardo).HasColumnName("id_persona_resguardo");
            entity.Property(e => e.IdProveedor).HasColumnName("id_proveedor");
            entity.Property(e => e.IdTipoAdquisicion).HasColumnName("id_tipo_adquisicion");
            entity.Property(e => e.IdTipoMaterial).HasColumnName("id_tipo_material");
            entity.Property(e => e.IdUbicacionAlmacen).HasColumnName("id_ubicacion_almacen");
            entity.Property(e => e.Precio).HasColumnName("precio");
            entity.Property(e => e.UnidadMedida).HasColumnName("unidad_medida");

            entity.HasOne(d => d.IdProveedorNavigation).WithMany(p => p.CatalogoHerramientasMateriales)
                .HasForeignKey(d => d.IdProveedor)
                .HasConstraintName("fk_proveedor");

            entity.HasOne(d => d.IdTipoAdquisicionNavigation).WithMany(p => p.CatalogoHerramientasMateriales)
                .HasForeignKey(d => d.IdTipoAdquisicion)
                .HasConstraintName("fk_tipo_adq");

            entity.HasOne(d => d.IdTipoMaterialNavigation).WithMany(p => p.CatalogoHerramientasMateriales)
                .HasForeignKey(d => d.IdTipoMaterial)
                .HasConstraintName("fk_tipo_material");

            entity.HasOne(d => d.IdUbicacionAlmacenNavigation).WithMany(p => p.CatalogoHerramientasMateriales)
                .HasForeignKey(d => d.IdUbicacionAlmacen)
                .HasConstraintName("fk_ubicacion_adq");
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
            entity.Property(e => e.Archivo).HasColumnName("archivo");
            entity.Property(e => e.Archivo2).HasColumnName("archivo_2");
            entity.Property(e => e.FechaMovimiento).HasColumnName("fecha_movimiento");
            entity.Property(e => e.IdEconomico)
                .HasMaxLength(20)
                .HasColumnName("id_economico");
            entity.Property(e => e.IdUbicacionLlegada).HasColumnName("id_ubicacion_llegada");
            entity.Property(e => e.IdUbicacionSalida).HasColumnName("id_ubicacion_salida");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.NombreArchivo).HasColumnName("nombre_archivo");
            entity.Property(e => e.NombreArchivo2).HasColumnName("nombre_archivo_2");
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

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.CatalogoMovimientosEconomicos)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("fk_usuario_movimiento");
        });

        modelBuilder.Entity<CatalogoProveedore>(entity =>
        {
            entity.HasKey(e => e.IdProveedor).HasName("catalogo_proveedores_pkey");

            entity.ToTable("catalogo_proveedores");

            entity.Property(e => e.IdProveedor)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id_proveedor");
            entity.Property(e => e.CorreoElectronico).HasColumnName("correo_electronico");
            entity.Property(e => e.NombreProveedor).HasColumnName("nombre_proveedor");
            entity.Property(e => e.NumeroContacto).HasColumnName("numero_contacto");
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

        modelBuilder.Entity<CatalogoRolPya>(entity =>
        {
            entity.HasKey(e => e.IdRol).HasName("catalogo_rol_pya_pkey");

            entity.ToTable("catalogo_rol_pya");

            entity.Property(e => e.IdRol)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id_rol");
            entity.Property(e => e.DescripcionRol).HasColumnName("descripcion_rol");
        });

        modelBuilder.Entity<CatalogoTipoAdquisicion>(entity =>
        {
            entity.HasKey(e => e.IdTipoAdquisicion).HasName("catalogo_tipo_adquisicion_pkey");

            entity.ToTable("catalogo_tipo_adquisicion");

            entity.Property(e => e.IdTipoAdquisicion).HasColumnName("id_tipo_adquisicion");
            entity.Property(e => e.DescripcionAdquisicion).HasColumnName("descripcion_adquisicion");
        });

        modelBuilder.Entity<CatalogoTipoMaterial>(entity =>
        {
            entity.HasKey(e => e.IdTipoMaterial).HasName("catalogo_tipo_material_pkey");

            entity.ToTable("catalogo_tipo_material");

            entity.Property(e => e.IdTipoMaterial).HasColumnName("id_tipo_material");
            entity.Property(e => e.DescripcionMaterial).HasColumnName("descripcion_material");
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
            entity.Property(e => e.Siglas)
                .HasColumnType("character varying")
                .HasColumnName("siglas");
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

        modelBuilder.Entity<Empleado>(entity =>
        {
            entity.HasKey(e => e.NoEmpleado).HasName("empleados_pkey");

            entity.ToTable("empleados");

            entity.Property(e => e.NoEmpleado)
                .ValueGeneratedNever()
                .HasColumnName("no_empleado");
            entity.Property(e => e.Ds3).HasColumnName("ds3");
            entity.Property(e => e.IdRolEmpleado).HasColumnName("id_rol_empleado");
            entity.Property(e => e.NombreEmpleado).HasColumnName("nombre_empleado");

            entity.HasOne(d => d.IdRolEmpleadoNavigation).WithMany(p => p.Empleados)
                .HasForeignKey(d => d.IdRolEmpleado)
                .HasConstraintName("fk_empleado_rol");
        });

        modelBuilder.Entity<HistorialLog>(entity =>
        {
            entity.HasKey(e => e.IdLog).HasName("historial_logs_pkey");

            entity.ToTable("historial_logs");

            entity.Property(e => e.IdLog).HasColumnName("id_log");
            entity.Property(e => e.DescripcionLog).HasColumnName("descripcion_log");
            entity.Property(e => e.FechaLog).HasColumnName("fecha_log");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.IpAddress).HasColumnName("ip_address");
            entity.Property(e => e.TipoLog).HasColumnName("tipo_log");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.HistorialLogs)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("fk_usuario");
        });

        modelBuilder.Entity<Requisicione>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("requisiciones");

            entity.Property(e => e.Consecutivo).HasColumnName("consecutivo");
            entity.Property(e => e.FechaRequisicion).HasColumnName("fecha_requisicion");
            entity.Property(e => e.IdRequisicion).HasColumnName("id_requisicion");
            entity.Property(e => e.IdUbicacion).HasColumnName("id_ubicacion");
            entity.Property(e => e.RazonSocial).HasColumnName("razon_social");
            entity.Property(e => e.TipoRequisicion).HasColumnName("tipo_requisicion");

            entity.HasOne(d => d.IdUbicacionNavigation).WithMany()
                .HasForeignKey(d => d.IdUbicacion)
                .HasConstraintName("fk_ubicacion_requisicion");
        });

        modelBuilder.Entity<RolEmpleado>(entity =>
        {
            entity.HasKey(e => e.IdRolEmpleado).HasName("rol_empleado_pkey");

            entity.ToTable("rol_empleado");

            entity.Property(e => e.IdRolEmpleado)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id_rol_empleado");
            entity.Property(e => e.DescripcionRol).HasColumnName("descripcion_rol");
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

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
