// Define las librerías del sistema necesarias para funcionalidades básicas.
// Importa el ORM Entity Framework Core para la interacción con la base de datos.
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System;
// Define librerías para el manejo de colecciones genéricas.
using System.Collections.Generic;

// Define el espacio de nombres donde se agrupan las clases del modelo de datos.
namespace Inventario.Data.Models;

// Declaración de la clase parcial que hereda de DbContext para gestionar la base de datos.
public partial class InventarioContext : DbContext
{
    // Constructor por defecto requerido por Entity Framework para la instanciación sin parámetros.
    public InventarioContext()
    {
    }

    // Constructor que recibe las opciones de configuración e inyecta el contexto a la clase base.
    public InventarioContext(DbContextOptions<InventarioContext> options)
        : base(options)
    {
    }

    // Propiedad DbSet para mapear la entidad CatalogoArchivo con la tabla correspondiente.
    public virtual DbSet<CatalogoArchivo> CatalogoArchivos { get; set; }

    // Propiedad DbSet para mapear la entidad CatalogoBroker con la tabla correspondiente.
    public virtual DbSet<CatalogoBroker> CatalogoBrokers { get; set; }

    // Propiedad DbSet para mapear la entidad CatalogoEconomico con la tabla correspondiente.
    public virtual DbSet<CatalogoEconomico> CatalogoEconomicos { get; set; }

    // Propiedad DbSet para mapear la entidad CatalogoEstatus con la tabla correspondiente.
    public virtual DbSet<CatalogoEstatus> CatalogoEstatuses { get; set; }

    // Propiedad DbSet para mapear la entidad CatalogoFrente con la tabla correspondiente.
    public virtual DbSet<CatalogoFrente> CatalogoFrentes { get; set; }

    // Propiedad DbSet para mapear la entidad CatalogoGrupo con la tabla correspondiente.
    public virtual DbSet<CatalogoGrupo> CatalogoGrupos { get; set; }

    // Propiedad DbSet para mapear la entidad CatalogoMarca con la tabla correspondiente.
    public virtual DbSet<CatalogoMarca> CatalogoMarcas { get; set; }

    // Propiedad DbSet para mapear la entidad CatalogoMovimientosEconomico con la tabla correspondiente.
    public virtual DbSet<CatalogoMovimientosEconomico> CatalogoMovimientosEconomicos { get; set; }

    // Propiedad DbSet para mapear la entidad CatalogoOperadore con la tabla correspondiente.
    public virtual DbSet<CatalogoOperadore> CatalogoOperadores { get; set; }

    // Propiedad DbSet para mapear la entidad CatalogoPya con la tabla correspondiente.
    public virtual DbSet<CatalogoPya> CatalogoPyas { get; set; }

    // Propiedad DbSet para mapear la entidad CatalogoResponsableMaquinarium con la tabla correspondiente.
    public virtual DbSet<CatalogoResponsableMaquinarium> CatalogoResponsableMaquinaria { get; set; }

    // Propiedad DbSet para mapear la entidad CatalogoRolPya con la tabla correspondiente.
    public virtual DbSet<CatalogoRolPya> CatalogoRolPyas { get; set; }

    // Propiedad DbSet para mapear la entidad CatalogoTiposCombustible con la tabla correspondiente.
    public virtual DbSet<CatalogoTiposCombustible> CatalogoTiposCombustibles { get; set; }

    // Propiedad DbSet para mapear la entidad CatalogoTiposEquipo con la tabla correspondiente.
    public virtual DbSet<CatalogoTiposEquipo> CatalogoTiposEquipos { get; set; }

    // Propiedad DbSet para mapear la entidad CatalogoTiposServicio con la tabla correspondiente.
    public virtual DbSet<CatalogoTiposServicio> CatalogoTiposServicios { get; set; }

    // Propiedad DbSet para mapear la entidad CatalogoTramo con la tabla correspondiente.
    public virtual DbSet<CatalogoTramo> CatalogoTramos { get; set; }

    // Propiedad DbSet para mapear la entidad CatalogoUbicacionesProyecto con la tabla correspondiente.
    public virtual DbSet<CatalogoUbicacionesProyecto> CatalogoUbicacionesProyectos { get; set; }

    // Propiedad DbSet para mapear la entidad EconomicosArchivo con la tabla correspondiente.
    public virtual DbSet<EconomicosArchivo> EconomicosArchivos { get; set; }

    // Propiedad DbSet para mapear la entidad HistorialLog con la tabla correspondiente.
    public virtual DbSet<HistorialLog> HistorialLogs { get; set; }

    // Propiedad DbSet para mapear la entidad Requisicione con la tabla correspondiente.
    public virtual DbSet<Requisicione> Requisiciones { get; set; }

    // Propiedad DbSet para mapear la entidad ServiciosEconomico con la tabla correspondiente.
    public virtual DbSet<ServiciosEconomico> ServiciosEconomicos { get; set; }

    // Propiedad DbSet para mapear la entidad Usuario con la tabla correspondiente.
    public virtual DbSet<Usuario> Usuarios { get; set; }

    // Propiedad DbSet para mapear la entidad UsuariosRole con la tabla correspondiente.
    public virtual DbSet<UsuariosRole> UsuariosRoles { get; set; }

    // Propiedad DbSet para mapear la entidad UsuriosRole con la tabla correspondiente.
    public virtual DbSet<UsuriosRole> UsuriosRoles { get; set; }

    // Sobrescribe el método de configuración del contexto de base de datos.
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Revisa si el DbContext no fue configurado previamente.
        if (!optionsBuilder.IsConfigured)
        {
            // Crea el constructor de configuración indicando la ruta del directorio base.
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            // Obtiene la cadena de conexión guardada en appsettings.json.
            var connectionString = configuration.GetConnectionString("InventarioConnection");

            // Configura el proveedor de PostgreSQL usando la cadena de conexión leída.
            optionsBuilder.UseNpgsql(connectionString);
        }
    }

    // Sobrescribe el método que construye las relaciones y mapeos Fluent API de las entidades.
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configura la estructura y mapeo de la entidad CatalogoArchivo.
        modelBuilder.Entity<CatalogoArchivo>(entity =>
        {
            // Define la llave primaria de la tabla catalogo_archivos.
            entity.HasKey(e => e.IdArchivo).HasName("catalogo_archivos_pkey");

            // Mapea la entidad al nombre exacto de la tabla en la base de datos.
            entity.ToTable("catalogo_archivos");

            // Configura la columna id_archivo como autoincrementable siempre.
            entity.Property(e => e.IdArchivo)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id_archivo");

            // Mapea la propiedad Archivo a la columna archivo.
            entity.Property(e => e.Archivo).HasColumnName("archivo");

            // Mapea la propiedad FechaSubida a la columna fecha_subida.
            entity.Property(e => e.FechaSubida).HasColumnName("fecha_subida");

            // Mapea la propiedad NombreArchivo a la columna nombre_archivo.
            entity.Property(e => e.NombreArchivo).HasColumnName("nombre_archivo");
        });

        // Configura la estructura y mapeo de la entidad CatalogoBroker.
        modelBuilder.Entity<CatalogoBroker>(entity =>
        {
            // Define la llave primaria de la tabla catalogo_brokers.
            entity.HasKey(e => e.IdBroker).HasName("catalogo_brokers_pkey");

            // Mapea la entidad a la tabla catalogo_brokers.
            entity.ToTable("catalogo_brokers");

            // Configura id_broker como llave primaria autoincrementable.
            entity.Property(e => e.IdBroker)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id_broker");

            // Mapea NombreBroker a la columna nombre_broker.
            entity.Property(e => e.NombreBroker).HasColumnName("nombre_broker");
        });

        // Configura la estructura y relaciones de la entidad CatalogoEconomico.
        modelBuilder.Entity<CatalogoEconomico>(entity =>
        {
            // Establece la llave primaria para catalogo_economicos.
            entity.HasKey(e => e.IdEconomico).HasName("catalogo_economicos_pkey");

            // Asocia la entidad a la tabla de base de datos catalogo_economicos.
            entity.ToTable("catalogo_economicos");

            // Crea un índice en la columna id_administrador para optimizar búsquedas.
            entity.HasIndex(e => e.IdAdministrador, "IX_catalogo_economicos_id_administrador");

            // Crea un índice en la columna id_combustible.
            entity.HasIndex(e => e.IdCombustible, "IX_catalogo_economicos_id_combustible");

            // Crea un índice en la columna id_estatus.
            entity.HasIndex(e => e.IdEstatus, "IX_catalogo_economicos_id_estatus");

            // Crea un índice en la columna id_grupo.
            entity.HasIndex(e => e.IdGrupo, "IX_catalogo_economicos_id_grupo");

            // Crea un índice en la columna id_operador.
            entity.HasIndex(e => e.IdOperador, "IX_catalogo_economicos_id_operador");

            // Crea un índice en la columna id_propietario.
            entity.HasIndex(e => e.IdPropietario, "IX_catalogo_economicos_id_propietario");

            // Crea un índice en la columna id_responsable.
            entity.HasIndex(e => e.IdResponsable, "IX_catalogo_economicos_id_responsable");

            // Crea un índice en la columna id_tipo_equipo.
            entity.HasIndex(e => e.IdTipoEquipo, "IX_catalogo_economicos_id_tipo_equipo");

            // Crea un índice en la columna id_ubicacion.
            entity.HasIndex(e => e.IdUbicacion, "IX_catalogo_economicos_id_ubicacion");

            // Configura longitud máxima y nombre de columna para IdEconomico.
            entity.Property(e => e.IdEconomico)
                .HasMaxLength(20)
                .HasColumnName("id_economico");

            // Mapea Consecutivo a la columna consecutivo.
            entity.Property(e => e.Consecutivo).HasColumnName("consecutivo");

            // Mapea Descripcion a la columna descripcion.
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");

            // Mapea EstatusSeguro a la columna estatus_seguro.
            entity.Property(e => e.EstatusSeguro).HasColumnName("estatus_seguro");

            // Mapea FamiliaMotor a la columna familia_motor.
            entity.Property(e => e.FamiliaMotor).HasColumnName("familia_motor");

            // Mapea GradoPropiedad a la columna grado_propiedad.
            entity.Property(e => e.GradoPropiedad).HasColumnName("grado_propiedad");

            // Mapea Horometro a la columna horometro.
            entity.Property(e => e.Horometro).HasColumnName("horometro");

            // Mapea IdAdministrador a la columna id_administrador.
            entity.Property(e => e.IdAdministrador).HasColumnName("id_administrador");

            // Mapea IdCombustible a la columna id_combustible.
            entity.Property(e => e.IdCombustible).HasColumnName("id_combustible");

            // Mapea IdEstatus a la columna id_estatus.
            entity.Property(e => e.IdEstatus).HasColumnName("id_estatus");

            // Mapea IdGrupo especificando longitud y nombre de columna.
            entity.Property(e => e.IdGrupo)
                .HasMaxLength(10)
                .HasColumnName("id_grupo");

            // Mapea IdMarca a la columna id_marca.
            entity.Property(e => e.IdMarca).HasColumnName("id_marca");

            // Mapea IdOperador a la columna id_operador.
            entity.Property(e => e.IdOperador).HasColumnName("id_operador");

            // Mapea IdPropietario a la columna id_propietario.
            entity.Property(e => e.IdPropietario).HasColumnName("id_propietario");

            // Mapea IdResponsable a la columna id_responsable.
            entity.Property(e => e.IdResponsable).HasColumnName("id_responsable");

            // Mapea IdTipoEquipo con longitud máxima de 45 caracteres.
            entity.Property(e => e.IdTipoEquipo)
                .HasMaxLength(45)
                .HasColumnName("id_tipo_equipo");

            // Mapea IdUbicacion a la columna id_ubicacion.
            entity.Property(e => e.IdUbicacion).HasColumnName("id_ubicacion");

            // Mapea MarcaMotor a la columna marca_motor.
            entity.Property(e => e.MarcaMotor).HasColumnName("marca_motor");

            // Mapea Modelo a la columna modelo.
            entity.Property(e => e.Modelo).HasColumnName("modelo");

            // Mapea ModeloMotor a la columna modelo_motor.
            entity.Property(e => e.ModeloMotor).HasColumnName("modelo_motor");

            // Mapea Motor a la columna motor.
            entity.Property(e => e.Motor).HasColumnName("motor");

            // Mapea ObservacionesAsignaciones a la columna observaciones_asignaciones.
            entity.Property(e => e.ObservacionesAsignaciones).HasColumnName("observaciones_asignaciones");

            // Mapea PeriodoFabricacion a la columna periodo_fabricacion.
            entity.Property(e => e.PeriodoFabricacion).HasColumnName("periodo_fabricacion");

            // Mapea Placas a la columna placas.
            entity.Property(e => e.Placas).HasColumnName("placas");

            // Mapea PolizaAdjunta a la columna poliza_adjunta.
            entity.Property(e => e.PolizaAdjunta).HasColumnName("poliza_adjunta");

            // Mapea Serie a la columna serie.
            entity.Property(e => e.Serie).HasColumnName("serie");

            // Mapea SerieMotor a la columna serie_motor.
            entity.Property(e => e.SerieMotor).HasColumnName("serie_motor");

            // Mapea Thk a la columna THK.
            entity.Property(e => e.Thk).HasColumnName("THK");

            // Mapea ValorAdquisicion a la columna valor_adquisicion.
            entity.Property(e => e.ValorAdquisicion).HasColumnName("valor_adquisicion");

            // Define relación de uno a muchos entre Administrador y CatalogoEconomico.
            entity.HasOne(d => d.IdAdministradorNavigation).WithMany(p => p.CatalogoEconomicoIdAdministradorNavigations)
                .HasForeignKey(d => d.IdAdministrador)
                .HasConstraintName("fk_administrador_economico");

            // Define relación de uno a muchos entre Combustible y CatalogoEconomico.
            entity.HasOne(d => d.IdCombustibleNavigation).WithMany(p => p.CatalogoEconomicos)
                .HasForeignKey(d => d.IdCombustible)
                .HasConstraintName("fk_combustible_economico");

            // Define relación de uno a muchos entre Estatus y CatalogoEconomico.
            entity.HasOne(d => d.IdEstatusNavigation).WithMany(p => p.CatalogoEconomicos)
                .HasForeignKey(d => d.IdEstatus)
                .HasConstraintName("fk_estatus_economico");

            // Define relación de uno a muchos entre Grupo y CatalogoEconomico.
            entity.HasOne(d => d.IdGrupoNavigation).WithMany(p => p.CatalogoEconomicos)
                .HasForeignKey(d => d.IdGrupo)
                .HasConstraintName("fk_grupo_economico");

            // Define relación de uno a muchos entre Marca y CatalogoEconomico.
            entity.HasOne(d => d.IdMarcaNavigation).WithMany(p => p.CatalogoEconomicoIdMarcaNavigations)
                .HasForeignKey(d => d.IdMarca)
                .HasConstraintName("fk_marca_economico");

            // Define relación de uno a muchos entre Operador y CatalogoEconomico.
            entity.HasOne(d => d.IdOperadorNavigation).WithMany(p => p.CatalogoEconomicos)
                .HasForeignKey(d => d.IdOperador)
                .HasConstraintName("fk_operador_economico");

            // Define relación de uno a muchos entre Propietario y CatalogoEconomico.
            entity.HasOne(d => d.IdPropietarioNavigation).WithMany(p => p.CatalogoEconomicoIdPropietarioNavigations)
                .HasForeignKey(d => d.IdPropietario)
                .HasConstraintName("fk_propietario_economico");

            // Define relación de uno a muchos entre Responsable y CatalogoEconomico.
            entity.HasOne(d => d.IdResponsableNavigation).WithMany(p => p.CatalogoEconomicos)
                .HasForeignKey(d => d.IdResponsable)
                .HasConstraintName("fk_responsable_economico");

            // Define relación de uno a muchos entre TipoEquipo y CatalogoEconomico.
            entity.HasOne(d => d.IdTipoEquipoNavigation).WithMany(p => p.CatalogoEconomicos)
                .HasForeignKey(d => d.IdTipoEquipo)
                .HasConstraintName("fk_tipo_equipo_economico");

            // Define relación de uno a muchos entre Ubicacion y CatalogoEconomico.
            entity.HasOne(d => d.IdUbicacionNavigation).WithMany(p => p.CatalogoEconomicos)
                .HasForeignKey(d => d.IdUbicacion)
                .HasConstraintName("fk_ubicacion_economico");

            // Define relación de uno a muchos entre MarcaMotor y CatalogoEconomico.
            entity.HasOne(d => d.MarcaMotorNavigation).WithMany(p => p.CatalogoEconomicoMarcaMotorNavigations)
                .HasForeignKey(d => d.MarcaMotor)
                .HasConstraintName("fk_marcamotor_economico");
        });

        // Configura la entidad CatalogoEstatus.
        modelBuilder.Entity<CatalogoEstatus>(entity =>
        {
            // Define la llave primaria.
            entity.HasKey(e => e.IdEstatus).HasName("catalogo_estatus_pkey");

            // Asocia a la tabla catalogo_estatus.
            entity.ToTable("catalogo_estatus");

            // Asigna id_estatus como columna autoincrementable.
            entity.Property(e => e.IdEstatus)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id_estatus");

            // Mapea DescripcionEstatus a la columna descripcion_estatus.
            entity.Property(e => e.DescripcionEstatus).HasColumnName("descripcion_estatus");
        });

        // Configura la entidad CatalogoFrente.
        modelBuilder.Entity<CatalogoFrente>(entity =>
        {
            // Define la llave primaria.
            entity.HasKey(e => e.IdFrente).HasName("catalogo_frentes_pkey");

            // Asocia a la tabla catalogo_frentes.
            entity.ToTable("catalogo_frentes");

            // Crea índice sobre id_tramo.
            entity.HasIndex(e => e.IdTramo, "IX_catalogo_frentes_id_tramo");

            // Configura IdFrente como autoincrementable.
            entity.Property(e => e.IdFrente)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id_frente");

            // Mapea IdTramo a la columna id_tramo.
            entity.Property(e => e.IdTramo).HasColumnName("id_tramo");

            // Mapea NombreFrente a la columna nombre_frente.
            entity.Property(e => e.NombreFrente).HasColumnName("nombre_frente");

            // Define relación con la entidad Tramo vía llave foránea id_tramo.
            entity.HasOne(d => d.IdTramoNavigation).WithMany(p => p.CatalogoFrentes)
                .HasForeignKey(d => d.IdTramo)
                .HasConstraintName("fk_ubicacion");
        });

        // Configura la entidad CatalogoGrupo.
        modelBuilder.Entity<CatalogoGrupo>(entity =>
        {
            // Define la llave primaria de la tabla catalogo_grupos.
            entity.HasKey(e => e.IdGrupo).HasName("catalogo_grupos_pkey");

            // Asocia la entidad a la tabla catalogo_grupos.
            entity.ToTable("catalogo_grupos");

            // Configura propiedad IdGrupo con un máximo de 10 caracteres.
            entity.Property(e => e.IdGrupo)
                .HasMaxLength(10)
                .HasColumnName("id_grupo");

            // Mapea DescripcionGrupo a la columna descripcion_grupo.
            entity.Property(e => e.DescripcionGrupo).HasColumnName("descripcion_grupo");
        });

        // Configura la entidad CatalogoMarca.
        modelBuilder.Entity<CatalogoMarca>(entity =>
        {
            // Define la llave primaria.
            entity.HasKey(e => e.IdMarca).HasName("catalogo_marcas_pkey");

            // Mapea a la tabla catalogo_marcas.
            entity.ToTable("catalogo_marcas");

            // Asigna IdMarca como autoincrementable.
            entity.Property(e => e.IdMarca)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id_marca");

            // Mapea NombreMarca a la columna nombre_marca.
            entity.Property(e => e.NombreMarca).HasColumnName("nombre_marca");
        });

        // Configura la entidad CatalogoMovimientosEconomico.
        modelBuilder.Entity<CatalogoMovimientosEconomico>(entity =>
        {
            // Define llave primaria.
            entity.HasKey(e => e.IdMovimiento).HasName("catalogo_movimientos_economicos_pkey");

            // Mapea a la tabla catalogo_movimientos_economicos.
            entity.ToTable("catalogo_movimientos_economicos");

            // Crea índice sobre id_economico.
            entity.HasIndex(e => e.IdEconomico, "IX_catalogo_movimientos_economicos_id_economico");

            // Crea índice sobre id_ubicacion_llegada.
            entity.HasIndex(e => e.IdUbicacionLlegada, "IX_catalogo_movimientos_economicos_id_ubicacion_llegada");

            // Crea índice sobre id_ubicacion_salida.
            entity.HasIndex(e => e.IdUbicacionSalida, "IX_catalogo_movimientos_economicos_id_ubicacion_salida");

            // Configura IdMovimiento como autoincrementable.
            entity.Property(e => e.IdMovimiento)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id_movimiento");

            // Mapea propiedad Archivo a la columna archivo.
            entity.Property(e => e.Archivo).HasColumnName("archivo");

            // Mapea propiedad Archivo2 a la columna archivo_2.
            entity.Property(e => e.Archivo2).HasColumnName("archivo_2");

            // Mapea FechaMovimiento a la columna fecha_movimiento.
            entity.Property(e => e.FechaMovimiento).HasColumnName("fecha_movimiento");

            // Mapea IdEconomico limitando a 20 caracteres.
            entity.Property(e => e.IdEconomico)
                .HasMaxLength(20)
                .HasColumnName("id_economico");

            // Mapea IdUbicacionLlegada a la columna id_ubicacion_llegada.
            entity.Property(e => e.IdUbicacionLlegada).HasColumnName("id_ubicacion_llegada");

            // Mapea IdUbicacionSalida a la columna id_ubicacion_salida.
            entity.Property(e => e.IdUbicacionSalida).HasColumnName("id_ubicacion_salida");

            // Mapea IdUsuario a la columna id_usuario.
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");

            // Mapea NombreArchivo a la columna nombre_archivo.
            entity.Property(e => e.NombreArchivo).HasColumnName("nombre_archivo");

            // Mapea NombreArchivo2 a la columna nombre_archivo_2.
            entity.Property(e => e.NombreArchivo2).HasColumnName("nombre_archivo_2");

            // Mapea UbicacionPersonalizada a la columna ubicacion_personalizada.
            entity.Property(e => e.UbicacionPersonalizada).HasColumnName("ubicacion_personalizada");

            // Define relación con la entidad Economico.
            entity.HasOne(d => d.IdEconomicoNavigation).WithMany(p => p.CatalogoMovimientosEconomicos)
                .HasForeignKey(d => d.IdEconomico)
                .HasConstraintName("fk_economico_ubicacion");

            // Define relación de llegada con la ubicación correspondiente.
            entity.HasOne(d => d.IdUbicacionLlegadaNavigation).WithMany(p => p.CatalogoMovimientosEconomicoIdUbicacionLlegadaNavigations)
                .HasForeignKey(d => d.IdUbicacionLlegada)
                .HasConstraintName("fk_llegada_proyecto_ubicacion");

            // Define relación de salida con la ubicación correspondiente.
            entity.HasOne(d => d.IdUbicacionSalidaNavigation).WithMany(p => p.CatalogoMovimientosEconomicoIdUbicacionSalidaNavigations)
                .HasForeignKey(d => d.IdUbicacionSalida)
                .HasConstraintName("fk_salida_proyecto_ubicacion");

            // Define relación con la entidad Usuario.
            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.CatalogoMovimientosEconomicos)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("fk_usuario_movimiento");
        });

        // Configura la entidad CatalogoOperadore.
        modelBuilder.Entity<CatalogoOperadore>(entity =>
        {
            // Establece llave primaria.
            entity.HasKey(e => e.IdOperador).HasName("catalogo_operadores_pkey");

            // Asocia a la tabla catalogo_operadores.
            entity.ToTable("catalogo_operadores");

            // Configura IdOperador como autoincrementable.
            entity.Property(e => e.IdOperador)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id_operador");

            // Mapea NombreOperador a la columna nombre_operador.
            entity.Property(e => e.NombreOperador).HasColumnName("nombre_operador");
        });

        // Configura la entidad CatalogoPya.
        modelBuilder.Entity<CatalogoPya>(entity =>
        {
            // Define llave primaria.
            entity.HasKey(e => e.IdPya).HasName("catalogo_pya_pkey");

            // Mapea a la tabla catalogo_pya.
            entity.ToTable("catalogo_pya");

            // Indiza el campo id_rol_pya.
            entity.HasIndex(e => e.IdRolPya, "IX_catalogo_pya_id_rol_pya");

            // Configura IdPya como columna autoincrementable.
            entity.Property(e => e.IdPya)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id_pya");

            // Mapea IdRolPya a la columna id_rol_pya.
            entity.Property(e => e.IdRolPya).HasColumnName("id_rol_pya");

            // Mapea Nombre a la columna nombre.
            entity.Property(e => e.Nombre).HasColumnName("nombre");

            // Define relación opcional configurando borrado en nulo en caso de eliminar el rol.
            entity.HasOne(d => d.IdRolPyaNavigation).WithMany(p => p.CatalogoPyas)
                .HasForeignKey(d => d.IdRolPya)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_rol_pya");
        });

        // Configura la entidad CatalogoResponsableMaquinarium.
        modelBuilder.Entity<CatalogoResponsableMaquinarium>(entity =>
        {
            // Define la llave primaria.
            entity.HasKey(e => e.IdResponsable).HasName("catalogo_responsable_maquinaria_pkey");

            // Asocia a la tabla catalogo_responsable_maquinaria.
            entity.ToTable("catalogo_responsable_maquinaria");

            // Configura IdResponsable como autoincrementable.
            entity.Property(e => e.IdResponsable)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id_responsable");

            // Mapea NombreResponsable a la columna nombre_responsable.
            entity.Property(e => e.NombreResponsable).HasColumnName("nombre_responsable");
        });

        // Configura la entidad CatalogoRolPya.
        modelBuilder.Entity<CatalogoRolPya>(entity =>
        {
            // Define llave primaria.
            entity.HasKey(e => e.IdRol).HasName("catalogo_rol_pya_pkey");

            // Mapea a la tabla catalogo_rol_pya.
            entity.ToTable("catalogo_rol_pya");

            // Configura IdRol como columna autoincrementable.
            entity.Property(e => e.IdRol)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id_rol");

            // Mapea DescripcionRol a la columna descripcion_rol.
            entity.Property(e => e.DescripcionRol).HasColumnName("descripcion_rol");
        });

        // Configura la entidad CatalogoTiposCombustible.
        modelBuilder.Entity<CatalogoTiposCombustible>(entity =>
        {
            // Define llave primaria.
            entity.HasKey(e => e.IdCombustible).HasName("catalogo_tipos_combustible_pkey");

            // Mapea a la tabla catalogo_tipos_combustible.
            entity.ToTable("catalogo_tipos_combustible");

            // Configura IdCombustible como autoincrementable.
            entity.Property(e => e.IdCombustible)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id_combustible");

            // Mapea DescripcionCombustible a la columna descripcion_combustible.
            entity.Property(e => e.DescripcionCombustible).HasColumnName("descripcion_combustible");
        });

        // Configura la entidad CatalogoTiposEquipo.
        modelBuilder.Entity<CatalogoTiposEquipo>(entity =>
        {
            // Define llave primaria.
            entity.HasKey(e => e.IdTipoEquipo).HasName("catalogo_tipos_equipos_pkey");

            // Asocia a la tabla catalogo_tipos_equipos.
            entity.ToTable("catalogo_tipos_equipos");

            // Mapea IdTipoEquipo limitándolo a 45 caracteres máximo.
            entity.Property(e => e.IdTipoEquipo)
                .HasMaxLength(45)
                .HasColumnName("id_tipo_equipo");

            // Mapea DescripcionTipoEquipo a la columna descripcion_tipo_equipo.
            entity.Property(e => e.DescripcionTipoEquipo).HasColumnName("descripcion_tipo_equipo");
        });

        // Configura la entidad CatalogoTiposServicio.
        modelBuilder.Entity<CatalogoTiposServicio>(entity =>
        {
            // Define llave primaria.
            entity.HasKey(e => e.IdTipoServicio).HasName("catalogo_tipos_servicios_pkey");

            // Asocia a la tabla catalogo_tipos_servicios.
            entity.ToTable("catalogo_tipos_servicios");

            // Configura IdTipoServicio como autoincrementable.
            entity.Property(e => e.IdTipoServicio)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id_tipo_servicio");

            // Mapea DescripcionServicio a la columna descripcion_servicio.
            entity.Property(e => e.DescripcionServicio).HasColumnName("descripcion_servicio");
        });

        // Configura la entidad CatalogoTramo.
        modelBuilder.Entity<CatalogoTramo>(entity =>
        {
            // Define llave primaria.
            entity.HasKey(e => e.IdTramo).HasName("catalogo_tramos_pkey");

            // Asocia a la tabla catalogo_tramos.
            entity.ToTable("catalogo_tramos");

            // Indiza el campo id_ubicacion.
            entity.HasIndex(e => e.IdUbicacion, "IX_catalogo_tramos_id_ubicacion");

            // Configura IdTramo como columna autoincrementable.
            entity.Property(e => e.IdTramo)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id_tramo");

            // Mapea IdUbicacion a la columna id_ubicacion.
            entity.Property(e => e.IdUbicacion).HasColumnName("id_ubicacion");

            // Mapea NombreTramo a la columna nombre_tramo.
            entity.Property(e => e.NombreTramo).HasColumnName("nombre_tramo");

            // Establece la relación con la entidad Ubicación.
            entity.HasOne(d => d.IdUbicacionNavigation).WithMany(p => p.CatalogoTramos)
                .HasForeignKey(d => d.IdUbicacion)
                .HasConstraintName("fk_ubicacion");
        });

        // Configura la entidad CatalogoUbicacionesProyecto.
        modelBuilder.Entity<CatalogoUbicacionesProyecto>(entity =>
        {
            // Define llave primaria.
            entity.HasKey(e => e.IdUbicacion).HasName("catalogo_ubicaciones_proyectos_pkey");

            // Asocia a la tabla catalogo_ubicaciones_proyectos.
            entity.ToTable("catalogo_ubicaciones_proyectos");

            // Configura IdUbicacion como autoincrementable.
            entity.Property(e => e.IdUbicacion)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id_ubicacion");

            // Mapea NombreProyecto a la columna nombre_proyecto.
            entity.Property(e => e.NombreProyecto).HasColumnName("nombre_proyecto");

            // Mapea Siglas asignando el tipo de dato SQL en PostgreSQL.
            entity.Property(e => e.Siglas)
                .HasColumnType("character varying")
                .HasColumnName("siglas");

            // Mapea Ubicacion a la columna ubicacion.
            entity.Property(e => e.Ubicacion).HasColumnName("ubicacion");
        });

        // Configura la entidad pivote EconomicosArchivo.
        modelBuilder.Entity<EconomicosArchivo>(entity =>
        {
            // Define llave primaria.
            entity.HasKey(e => e.IdEconomicoArchivo).HasName("archivos_economicos_pkey");

            // Mapea a la tabla economicos_archivos.
            entity.ToTable("economicos_archivos");

            // Indiza el campo id_archivo.
            entity.HasIndex(e => e.IdArchivo, "IX_economicos_archivos_id_archivo");

            // Indiza el campo id_economico.
            entity.HasIndex(e => e.IdEconomico, "IX_economicos_archivos_id_economico");

            // Mapea IdEconomicoArchivo a id_economico_archivo.
            entity.Property(e => e.IdEconomicoArchivo).HasColumnName("id_economico_archivo");

            // Mapea IdArchivo a id_archivo.
            entity.Property(e => e.IdArchivo).HasColumnName("id_archivo");

            // Mapea IdEconomico limitando a 20 caracteres.
            entity.Property(e => e.IdEconomico)
                .HasMaxLength(20)
                .HasColumnName("id_economico");

            // Define relación foránea con la entidad Archivo.
            entity.HasOne(d => d.IdArchivoNavigation).WithMany(p => p.EconomicosArchivos)
                .HasForeignKey(d => d.IdArchivo)
                .HasConstraintName("fk_archivo_economico");

            // Define relación foránea con la entidad Economico.
            entity.HasOne(d => d.IdEconomicoNavigation).WithMany(p => p.EconomicosArchivos)
                .HasForeignKey(d => d.IdEconomico)
                .HasConstraintName("fk_economico_archivo");
        });

        // Configura la entidad HistorialLog.
        modelBuilder.Entity<HistorialLog>(entity =>
        {
            // Define la llave primaria.
            entity.HasKey(e => e.IdLog).HasName("historial_logs_pkey");

            // Mapea a la tabla historial_logs.
            entity.ToTable("historial_logs");

            // Mapea IdLog a la columna id_log.
            entity.Property(e => e.IdLog).HasColumnName("id_log");

            // Mapea DescripcionLog a la columna descripcion_log.
            entity.Property(e => e.DescripcionLog).HasColumnName("descripcion_log");

            // Mapea FechaLog a la columna fecha_log.
            entity.Property(e => e.FechaLog).HasColumnName("fecha_log");

            // Mapea IdUsuario a la columna id_usuario.
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");

            // Mapea IpAddress a la columna ip_address.
            entity.Property(e => e.IpAddress).HasColumnName("ip_address");

            // Mapea TipoLog a la columna tipo_log.
            entity.Property(e => e.TipoLog).HasColumnName("tipo_log");

            // Define relación con la entidad Usuario asociando el log.
            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.HistorialLogs)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("fk_usuario");
        });

        // Configura la entidad Requisicione.
        modelBuilder.Entity<Requisicione>(entity =>
        {
            // Especifica explícitamente que la vista o tabla no posee una llave primaria.
            entity
                .HasNoKey()
                .ToTable("requisiciones");

            // Mapea Consecutivo a la columna consecutivo.
            entity.Property(e => e.Consecutivo).HasColumnName("consecutivo");

            // Mapea FechaRequisicion a la columna fecha_requisicion.
            entity.Property(e => e.FechaRequisicion).HasColumnName("fecha_requisicion");

            // Mapea IdRequisicion a la columna id_requisicion.
            entity.Property(e => e.IdRequisicion).HasColumnName("id_requisicion");

            // Mapea IdUbicacion a la columna id_ubicacion.
            entity.Property(e => e.IdUbicacion).HasColumnName("id_ubicacion");

            // Mapea RazonSocial a la columna razon_social.
            entity.Property(e => e.RazonSocial).HasColumnName("razon_social");

            // Mapea TipoRequisicion a la columna tipo_requisicion.
            entity.Property(e => e.TipoRequisicion).HasColumnName("tipo_requisicion");

            // Define relación hacia Ubicacion sin navegación inversa.
            entity.HasOne(d => d.IdUbicacionNavigation).WithMany()
                .HasForeignKey(d => d.IdUbicacion)
                .HasConstraintName("fk_ubicacion_requisicion");
        });

        // Configura la entidad ServiciosEconomico.
        modelBuilder.Entity<ServiciosEconomico>(entity =>
        {
            // Define la llave primaria.
            entity.HasKey(e => e.IdServicioEconomico).HasName("servicios_economicos_pkey");

            // Mapea a la tabla servicios_economicos.
            entity.ToTable("servicios_economicos");

            // Indiza id_economico.
            entity.HasIndex(e => e.IdEconomico, "IX_servicios_economicos_id_economico");

            // Indiza id_tipo_servicio.
            entity.HasIndex(e => e.IdTipoServicio, "IX_servicios_economicos_id_tipo_servicio");

            // Configura IdServicioEconomico como autoincrementable.
            entity.Property(e => e.IdServicioEconomico)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id_servicio_economico");

            // Mapea FechaServicio a la columna fecha_servicio.
            entity.Property(e => e.FechaServicio).HasColumnName("fecha_servicio");

            // Mapea Horometro a la columna horometro.
            entity.Property(e => e.Horometro).HasColumnName("horometro");

            // Mapea IdEconomico ajustando límite de caracteres.
            entity.Property(e => e.IdEconomico)
                .HasMaxLength(20)
                .HasColumnName("id_economico");

            // Mapea IdTipoServicio a la columna id_tipo_servicio.
            entity.Property(e => e.IdTipoServicio).HasColumnName("id_tipo_servicio");

            // Mapea Kilometraje a la columna kilometraje.
            entity.Property(e => e.Kilometraje).HasColumnName("kilometraje");

            // Define relación con la entidad Economico.
            entity.HasOne(d => d.IdEconomicoNavigation).WithMany(p => p.ServiciosEconomicos)
                .HasForeignKey(d => d.IdEconomico)
                .HasConstraintName("fk_economico_servicio");

            // Define relación con el catálogo de tipos de servicio.
            entity.HasOne(d => d.IdTipoServicioNavigation).WithMany(p => p.ServiciosEconomicos)
                .HasForeignKey(d => d.IdTipoServicio)
                .HasConstraintName("fk_servicio_economico");
        });

        // Configura la entidad Usuario.
        modelBuilder.Entity<Usuario>(entity =>
        {
            // Define la llave primaria.
            entity.HasKey(e => e.IdUsuario).HasName("usuarios_pkey");

            // Mapea a la tabla usuarios.
            entity.ToTable("usuarios");

            // Configura IdUsuario como autoincrementable.
            entity.Property(e => e.IdUsuario)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id_usuario");

            // Mapea IdRol a la columna id_rol.
            entity.Property(e => e.IdRol).HasColumnName("id_rol");

            // Mapea NombreUsuario a la columna nombre_usuario.
            entity.Property(e => e.NombreUsuario).HasColumnName("nombre_usuario");

            // Mapea Password a la columna password.
            entity.Property(e => e.Password).HasColumnName("password");

            // Define relación con la tabla de roles de usuario.
            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdRol)
                .HasConstraintName("fk_usuario_rol");
        });

        // Configura la entidad UsuariosRole.
        modelBuilder.Entity<UsuariosRole>(entity =>
        {
            // Define la llave primaria.
            entity.HasKey(e => e.IdRol).HasName("usurios_roles_pkey");

            // Mapea a la tabla usuarios_roles.
            entity.ToTable("usuarios_roles");

            // Configura IdRol como autoincrementable.
            entity.Property(e => e.IdRol)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id_rol");

            // Mapea DescripcionRol a la columna descripcion_rol.
            entity.Property(e => e.DescripcionRol).HasColumnName("descripcion_rol");
        });

        // Configura la entidad UsuriosRole.
        modelBuilder.Entity<UsuriosRole>(entity =>
        {
            // Define la llave primaria de la tabla usurios_roles.
            entity.HasKey(e => e.IdLog).HasName("usurios_roles_pkey1");

            // Mapea a la tabla usurios_roles.
            entity.ToTable("usurios_roles");

            // Configura IdLog como autoincrementable.
            entity.Property(e => e.IdLog)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id_log");

            // Mapea IdUsuario a la columna id_usuario.
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");

            // Mapea InformacionLog a la columna informacion_log.
            entity.Property(e => e.InformacionLog).HasColumnName("informacion_log");

            // Mapea TipoLog a la columna tipo_log.
            entity.Property(e => e.TipoLog).HasColumnName("tipo_log");

            // Define relación foránea para auditar al usuario correspondiente.
            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.UsuriosRoles)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("fk_log_usuario");
        });

        // Llama al método parcial de personalización de mapeo adicional si existe.
        OnModelCreatingPartial(modelBuilder);
    }

    // Declaración del método parcial para extender la configuración de modelos sin modificar este archivo.
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}