using Inventario.Core.DTOs;
using Inventario.Core.Services.Logs;
using Inventario.Data;
using Inventario.Data.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Inventario.Core.Services.Economicos
{
    public class CatalogoEconomicosService
    {
        private readonly InventarioContext _context;

        private readonly LogsService _logsService;

        public CatalogoEconomicosService(InventarioContext context, LogsService logsService)
        {
            _context = context;
            _logsService = logsService;
        }

        public CatalogoEconomico? ObtenerPorIdParaEditar(string idEconomico)
        {
            return _context.CatalogoEconomicos.FirstOrDefault(e => e.IdEconomico == idEconomico);
        }

        public bool ActualizarEconomico(CatalogoEconomico economicoEditado)
        {
            if (economicoEditado == null) return false;

            try
            {
                return _context.SaveChanges() >= 0;
            }
            catch (DbUpdateException)
            {
                throw;
            }
        }

        public List<EconomicoMinimoDto> ObtenerEconomicosCortos()
        {
            var resultado = _context.CatalogoEconomicos
                .Where(e => e.IdGrupo != "SUV")
                .Include(e => e.IdMarcaNavigation)
                .Include(e => e.IdTipoEquipoNavigation)
                .Select(e => new EconomicoMinimoDto
                {
                    IdEconomico = e.IdEconomico,
                    Descripcion = e.Descripcion,
                    IdMarca = e.IdMarca,
                    NombreMarca = e.IdMarcaNavigation != null ? e.IdMarcaNavigation.NombreMarca : "Sin Marca",
                    Modelo = e.Modelo,
                    Serie = e.Serie,
                    PeriodoFabricacion = e.PeriodoFabricacion,
                    IdUbicacion = e.IdUbicacion,
                    IdUbicacionNavigation = e.IdUbicacionNavigation,
                    IdTipoEquipo = e.IdTipoEquipo,
                    IdTipoEquipoNavigation = e.IdTipoEquipoNavigation,
                    IdMarcaNavigation = e.IdMarcaNavigation,
                    TipoSeguro = e.TipoSeguro
                })
                .ToList();
            return resultado;
        }

        public List<EconomicoMinimoDto> ObtenerEconomicosCortosCompletos()
        {
            var resultado = _context.CatalogoEconomicos
                .Include(e => e.IdMarcaNavigation)
                .Include(e => e.IdTipoEquipoNavigation)
                .Select(e => new EconomicoMinimoDto
                {
                    IdEconomico = e.IdEconomico,
                    Descripcion = e.Descripcion,
                    IdMarca = e.IdMarca,
                    NombreMarca = e.IdMarcaNavigation != null ? e.IdMarcaNavigation.NombreMarca : "Sin Marca",
                    Modelo = e.Modelo,
                    Serie = e.Serie,
                    PeriodoFabricacion = e.PeriodoFabricacion,
                    IdUbicacion = e.IdUbicacion,
                    IdUbicacionNavigation = e.IdUbicacionNavigation,
                    IdTipoEquipo = e.IdTipoEquipo,
                    IdTipoEquipoNavigation = e.IdTipoEquipoNavigation,
                    IdMarcaNavigation = e.IdMarcaNavigation,
                    TipoSeguro = e.TipoSeguro
                })
                .ToList();
            return resultado;
        }

        public List<EconomicoMinimoDto> ObtenerSUVCortos()
        {
            var resultado = _context.CatalogoEconomicos
                .Where(e => e.IdGrupo == "SUV")
                .Include(e => e.IdMarcaNavigation)
                .Include(e => e.IdTipoEquipoNavigation)
                .Select(e => new EconomicoMinimoDto
                {
                    IdEconomico = e.IdEconomico,
                    Descripcion = e.Descripcion,
                    IdMarca = e.IdMarca,
                    NombreMarca = e.IdMarcaNavigation != null ? e.IdMarcaNavigation.NombreMarca : "Sin Marca",
                    Modelo = e.Modelo,
                    Serie = e.Serie,
                    PeriodoFabricacion = e.PeriodoFabricacion,
                    IdUbicacion = e.IdUbicacion,
                    IdUbicacionNavigation = e.IdUbicacionNavigation,
                    IdTipoEquipo = e.IdTipoEquipo,
                    IdTipoEquipoNavigation = e.IdTipoEquipoNavigation,
                    IdMarcaNavigation = e.IdMarcaNavigation,
                    TipoSeguro = e.TipoSeguro
                })
                .ToList();
            return resultado;
        }

        public CatalogoEconomico? ObtenerDetalleCompleto(string idEconomico)
        {
            return _context.CatalogoEconomicos
                
                .Include(e => e.IdMarcaNavigation)
                .Include(e => e.MarcaMotorNavigation)
                .Include(e => e.IdTipoEquipoNavigation)
                .Include(e => e.IdGrupoNavigation)
                .Include(e => e.IdUbicacionNavigation)
                .Include(e => e.IdCombustibleNavigation)
                .Include(e => e.IdPropietarioNavigation)
                .Include(e => e.IdAdministradorNavigation)
                .Include(e => e.IdEstatusNavigation)
                .Include(e => e.IdOperadorNavigation)
                .Include(e => e.IdResponsableNavigation)
                .Include(e => e.EconomicosArchivos)
                    .ThenInclude(ae => ae.IdArchivoNavigation)
                .AsNoTracking()
                .FirstOrDefault(e => e.IdEconomico == idEconomico);
        }

        public bool RegistrarEconomico(int idUsuarioOperativo, EconomicoAltaDto dto)
        {
            if (ValidarSerieDuplicada(dto.Serie))
            {
                throw new Exception($"El número de serie '{dto.Serie}' ya se encuentra registrado.");
            }

            var grupoSeleccionado = _context.CatalogoGrupos.FirstOrDefault(g => g.IdGrupo == dto.IdGrupo);
            if (grupoSeleccionado == null)
            {
                throw new Exception("El grupo seleccionado no es válido o no existe en la base de datos.");
            }

            var modeloDb = new CatalogoEconomico
            {
                IdEconomico = string.Empty,
                IdTipoEquipo = dto.IdTipoEquipo,
                IdGrupo = dto.IdGrupo,
                IdCombustible = dto.IdCombustible,
                IdPropietario = dto.IdPropietario ,
                IdAdministrador = dto.IdAdministrador,
                IdOperador = dto.IdOperador,
                IdResponsable = dto.IdResponsable,
                IdUbicacion = dto.IdUbicacion,
                IdMarca = dto.IdMarca,
                Modelo = dto.Modelo,
                Serie = dto.Serie,
                PeriodoFabricacion = dto.PeriodoFab,
                GradoPropiedad = dto.GradoPropiedad,
                MarcaMotor = dto.MarcaMotor,
                ModeloMotor = dto.ModeloMotor,
                SerieMotor = dto.SerieMotor,
                FamiliaMotor = dto.FamiliaMotor,
                Placas = dto.Placas,
                EstatusSeguro = dto.EstatusSeguro,
                ObservacionesAsignaciones = dto.Observaciones,
                Descripcion = grupoSeleccionado.DescripcionGrupo,
                Horometro = dto.Horometro,
                Thk = dto.THK,
                Dimensiones = dto.Dimensiones
            };

            _context.CatalogoEconomicos.Add(modeloDb);
            _context.SaveChanges(); 


            string? idGenerado = _context.CatalogoEconomicos
                .FromSqlRaw("SELECT * FROM catalogo_economicos WHERE serie = {0}", dto.Serie)
                .AsNoTracking()
                .Select(e => e.IdEconomico)
                .FirstOrDefault();

            if (string.IsNullOrEmpty(idGenerado))
            {
                idGenerado = modeloDb.IdEconomico;
            }

            _logsService.RegistrarAltaEquipoExitoso(idUsuarioOperativo, idGenerado);

            return true;
        }

        public List<CatalogoEconomico> ObtenerEconomicosPorUbicacion(int idUbicacion)
        {
            return _context.CatalogoEconomicos
                .Include(e => e.IdMarcaNavigation)
                .Where(e => e.IdUbicacion == idUbicacion)
                .AsNoTracking()
                .ToList();
        }

        public bool ValidarSerieDuplicada(string serie)
        {
            bool existe = _context.CatalogoEconomicos.Any(e => e.Serie == serie);
            return existe;
        }

        public List<CatalogoEconomico> ObtenerEconomicosPorListaDeIds(string[] ids)
        {
            // Ejecuta una consulta con cargas de relaciones filtrando registros mediante coincidencia de conjunto por medio de WHERE IN.
            return _context.CatalogoEconomicos
                // CORRECCIÓN: Asegura la inclusión de la marca para el despliegue del listado filtrado por IDs.
                .Include(e => e.IdMarcaNavigation)
                // NUEVA CORRECCIÓN CRÍTICA: Carga explícitamente la relación de la marca del motor para evitar valores nulos en los detalles.
                .Include(e => e.MarcaMotorNavigation)
                // Carga la relación con el tipo de equipo asociado.
                .Include(e => e.IdTipoEquipoNavigation)
                // Carga la relación con el grupo de inventario correspondiente.
                .Include(e => e.IdGrupoNavigation)
                // Carga la relación con la ubicación o frente de obra asignada.
                .Include(e => e.IdUbicacionNavigation)
                // Carga la relación con el tipo de combustible requerido.
                .Include(e => e.IdCombustibleNavigation)
                // Carga la relación de la entidad propietaria de la maquinaria.
                .Include(e => e.IdPropietarioNavigation)
                // Carga la relación de la organización que administra las operaciones.
                .Include(e => e.IdAdministradorNavigation)
                // Carga la relación del estatus de ciclo de vida del activo.
                .Include(e => e.IdEstatusNavigation)
                // Carga la relación del operador técnico asignado temporal o fijamente.
                .Include(e => e.IdOperadorNavigation)
                // Carga la relación del ingeniero o encargado responsable del activo.
                .Include(e => e.IdResponsableNavigation)
                // Filtra los renglones donde el ID del económico coincida con alguno de los elementos del arreglo provisto.
                .Where(e => ids.Contains(e.IdEconomico))
                // Omite el guardado en caché de seguimiento optimizando el rendimiento de lectura.
                .AsNoTracking()
                // Transforma y descarga la respuesta de la base de datos en una colección estructurada de tipo List.
                .ToList();
        }

        public List<CatalogoEconomico> ObtenerEconomicosCompletosFiltrados(string idTipoEquipo, int? idEstatus)
        {
            IQueryable<CatalogoEconomico> query = _context.CatalogoEconomicos;

            query = query
                .Include(e => e.IdMarcaNavigation)
                .Include(e => e.IdTipoEquipoNavigation)
                .Include(e => e.IdGrupoNavigation)
                .Include(e => e.IdUbicacionNavigation)
                .Include(e => e.IdCombustibleNavigation)
                .Include(e => e.IdPropietarioNavigation)
                .Include(e => e.IdAdministradorNavigation)
                .Include(e => e.IdEstatusNavigation)
                .Include(e => e.IdOperadorNavigation)
                .Include(e => e.IdResponsableNavigation);

            if (!string.IsNullOrEmpty(idTipoEquipo))
            {
                query = query.Where(e => e.IdTipoEquipo == idTipoEquipo);
            }
            if (idEstatus.HasValue)
            {
                query = query.Where(e => e.IdEstatus == idEstatus.Value);
            }
            return query.AsNoTracking().ToList();
        }
    }
}