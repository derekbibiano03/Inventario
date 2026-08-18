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
            // Validación de la entrada; si el parámetro es nulo o vacío, termina el proceso.
            if (string.IsNullOrWhiteSpace(rutaRemotaServidor))
            {
                return null;
            }

            try
            {
                // 1. Limpiamos cualquier prefijo 'ftp://' o prefijos incorrectos que vengan en el string.
                string rutaLimpia = rutaRemotaServidor.Replace("ftp://", "", StringComparison.OrdinalIgnoreCase).TrimStart('/');

                // 2. Construimos la URL HTTP base asegurándonos de que comience con http:// o https://.
                string urlWeb;
                if (rutaLimpia.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                    rutaLimpia.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                {
                    urlWeb = rutaLimpia;
                }
                else
                {
                    // Si el path no tiene protocolo, le asignamos http:// directamente.
                    urlWeb = $"http://{rutaLimpia}";
                }

                // 3. Escapamos espacios y caracteres especiales para evitar errores HTTP 400/404 (ej. reemplazar espacios por %20).
                Uri uriValida = new Uri(urlWeb);

                // 4. Extraemos únicamente el nombre del archivo para la ruta de guardado temporal.
                string nombreArchivo = Path.GetFileName(uriValida.AbsolutePath);

                // 5. Generamos una ruta local dentro de la carpeta Temp del sistema operativo con un GUID para evitar colisiones.
                string rutaTemporalLocal = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}_{nombreArchivo}");

                // 6. Instanciamos HttpClient para realizar la petición de descarga al servidor Web.
                using (HttpClient client = new HttpClient())
                {
                    // Realizamos la llamada síncrona para obtener el contenido completo del archivo en bytes.
                    byte[] fileBytes = client.GetByteArrayAsync(uriValida).GetAwaiter().GetResult();

                    // Escribimos los bytes directamente en el disco duro local.
                    File.WriteAllBytes(rutaTemporalLocal, fileBytes);
                }

                // Retornamos la ruta del archivo local generado para que la vista/proceso lo abra.
                return rutaTemporalLocal;
            }
            catch (Exception ex)
            {
                // Capturamos cualquier excepción de red o de E/S y lanzamos una nueva con el detalle correspondiente.
                throw new Exception($"Error al obtener el archivo desde el servidor Web: {ex.Message}", ex);
            }
        }

        // CORRECCIÓN AQUÍ: Cargar las tablas relacionales de los archivos
        public List<HistorialServicio> ObtenerHistorial()
        {
            return _context.HistorialServicios
                .Include(s => s.NoEconomicoNavigation)
                .Include(s => s.ServicioArchivos)
                    .ThenInclude(sa => sa.IdArchivoNavigation)
                .ToList();
        }
    }
}