using Inventario.Core.Services.Logs;
using Inventario.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace Inventario.Core.Services.Economicos
{
    public class HistorialServicioService
    {
        private readonly InventarioContext _context;
        private readonly LogsService _logsService;

        public HistorialServicioService(InventarioContext context, LogsService logsService)
        {
            _context = context;
            _logsService = logsService;
        }

        public HistorialServicio RegistrarServicio(int idUsuarioOperativo, HistorialServicio dto)
        {
            var nuevoRegistro = new HistorialServicio
            {
                NoEconomico = dto.NoEconomico,
                FechaMantenimiento = dto.FechaMantenimiento,
                TipoMantenimiento = dto.TipoMantenimiento,
                Anotaciones = dto.Anotaciones,
                Horaskilometrosreales = dto.Horaskilometrosreales
            };

            _context.HistorialServicios.Add(nuevoRegistro);
            _context.SaveChanges();

            int idServicio = nuevoRegistro.IdServicio;
            _logsService.RegistrarServicioEquipo(idUsuarioOperativo, idServicio);

            return nuevoRegistro;
        }

        public void GuardarArchivosRelacionados(List<(CatalogoArchivo archivoCatalogado, ServicioArchivo relacionServicio)> archivosProcesados)
        {
            foreach (var item in archivosProcesados)
            {
                _context.CatalogoArchivos.Add(item.archivoCatalogado);
                _context.ServicioArchivos.Add(item.relacionServicio);
            }

            _context.SaveChanges();
        }

        public string? ObtenerArchivoTemporalDesdeFtp(string rutaRemotaServidor)
        {
            if (string.IsNullOrWhiteSpace(rutaRemotaServidor))
            {
                return null;
            }
            try
            {
                string rutaLimpia = rutaRemotaServidor.Replace("ftp://", "", StringComparison.OrdinalIgnoreCase).TrimStart('/');
                string urlWeb;
                if (rutaLimpia.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                    rutaLimpia.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                {
                    urlWeb = rutaLimpia;
                }
                else
                {
                    urlWeb = $"http://{rutaLimpia}";
                }

                Uri uriValida = new Uri(urlWeb);
                string nombreArchivo = Path.GetFileName(uriValida.AbsolutePath);
                string rutaTemporalLocal = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}_{nombreArchivo}");
                using (HttpClient client = new HttpClient())
                {
                    byte[] fileBytes = client.GetByteArrayAsync(uriValida).GetAwaiter().GetResult();
                    File.WriteAllBytes(rutaTemporalLocal, fileBytes);
                }
                return rutaTemporalLocal;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener el archivo desde el servidor Web: {ex.Message}", ex);
            }
        }

        public List<HistorialServicio> ObtenerHistorial()
        {
            return _context.HistorialServicios
                .Include(s => s.NoEconomicoNavigation)
                .Include(s => s.ServicioArchivos)
                    .ThenInclude(sa => sa.IdArchivoNavigation)
                .ToList();
        }

        public void ModificarServicio(HistorialServicio dto, List<(CatalogoArchivo archivoCatalogado, ServicioArchivo relacionServicio)> nuevosArchivos, List<int> idsArchivosAEliminar)
        {
            var servicioExistente = _context.HistorialServicios
                .Include(s => s.ServicioArchivos)
                .FirstOrDefault(s => s.IdServicio == dto.IdServicio);

            if (servicioExistente == null)
            {
                throw new Exception($"El servicio con ID {dto.IdServicio} no existe.");
            }

            using var transaction = _context.Database.BeginTransaction();
            try
            {
                servicioExistente.NoEconomico = dto.NoEconomico;
                servicioExistente.FechaMantenimiento = dto.FechaMantenimiento;
                servicioExistente.TipoMantenimiento = dto.TipoMantenimiento;
                servicioExistente.Anotaciones = dto.Anotaciones;
                servicioExistente.Horaskilometrosreales = dto.Horaskilometrosreales;

                if (idsArchivosAEliminar != null && idsArchivosAEliminar.Any())
                {
                    var relacionesAEliminar = servicioExistente.ServicioArchivos
                        .Where(sa => sa.IdArchivo.HasValue && idsArchivosAEliminar.Contains(sa.IdArchivo.Value))
                        .ToList();

                    foreach (var relacion in relacionesAEliminar)
                    {
                        _context.ServicioArchivos.Remove(relacion);

                        if (relacion.IdArchivo.HasValue)
                        {
                            var archivoCatalogo = _context.CatalogoArchivos.Find(relacion.IdArchivo.Value);
                            if (archivoCatalogo != null)
                            {
                                _context.CatalogoArchivos.Remove(archivoCatalogo);
                            }
                        }
                    }
                }

                if (nuevosArchivos != null && nuevosArchivos.Any())
                {
                    foreach (var item in nuevosArchivos)
                    {
                        _context.CatalogoArchivos.Add(item.archivoCatalogado);
                        _context.SaveChanges();

                        item.relacionServicio.IdServicio = servicioExistente.IdServicio;
                        item.relacionServicio.IdArchivo = item.archivoCatalogado.IdArchivo;

                        _context.ServicioArchivos.Add(item.relacionServicio);
                    }
                }

                _context.SaveChanges();
                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }

        public void EliminarServicio(int idServicio)
        {
            var servicio = _context.HistorialServicios
                                   .Include(s => s.ServicioArchivos)
                                   .FirstOrDefault(s => s.IdServicio == idServicio);

            if (servicio != null)
            {
                // 1. Elimina las relaciones y registros de archivos asociados en cascada
                if (servicio.ServicioArchivos != null && servicio.ServicioArchivos.Any())
                {
                    foreach (var relacion in servicio.ServicioArchivos.ToList())
                    {
                        _context.ServicioArchivos.Remove(relacion);

                        if (relacion.IdArchivo.HasValue)
                        {
                            var archivoCatalogo = _context.CatalogoArchivos.Find(relacion.IdArchivo.Value);
                            if (archivoCatalogo != null)
                            {
                                _context.CatalogoArchivos.Remove(archivoCatalogo);
                            }
                        }
                    }
                }

                // 2. Elimina la entidad principal
                _context.HistorialServicios.Remove(servicio);

                // 3. Persiste los cambios
                _context.SaveChanges();
            }
        }

        public void ActualizarServicio(HistorialServicio servicioEditado)
        {
            // Busca el registro existente en la base de datos
            var servicioExistente = _context.HistorialServicios
                                            .FirstOrDefault(s => s.IdServicio == servicioEditado.IdServicio);

            if (servicioExistente == null)
            {
                throw new Exception($"No se encontró el registro de servicio con ID {servicioEditado.IdServicio}.");
            }

            // Actualiza las propiedades editables
            servicioExistente.FechaMantenimiento = servicioEditado.FechaMantenimiento;
            servicioExistente.TipoMantenimiento = servicioEditado.TipoMantenimiento;
            servicioExistente.Anotaciones = servicioEditado.Anotaciones;
            servicioExistente.Horaskilometrosreales = servicioEditado.Horaskilometrosreales;

            // Guarda los cambios en MySQL / PostgreSQL / SQL Server
            _context.SaveChanges();
        }
    }
}