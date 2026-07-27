using Inventario.Core.Services.Logs;
using Inventario.Data.Models;
using Microsoft.EntityFrameworkCore;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Inventario.Core.Services.Economicos
{
    public class RealizarMovimientosService
    {
        private readonly InventarioContext _context;
        private readonly LogsService _logsService;

        public RealizarMovimientosService(InventarioContext context, LogsService logsService)
        {
            _context = context;
            _logsService = logsService;
        }

        private readonly string _hostUbuntu = "192.168.0.24";
        private readonly string _usuarioSsh = "admin_bibiano";
        private readonly string _contrasenaSsh = "11122003drbr";
        private readonly string _directorioRemoto = "/var/www/ArchivosEconomicos/MovimientosEconomicos";

        public bool RegistrarMovimientosMultiples(
            int idUsuarioOperativo,
            List<string> listaIdEconomicos,
            int idUbicacionLlegada,
            int idUbicacionSalida,
            DateTime fechaMovimiento,
            string? rutaOriginal,
            string? rutaOriginal2)
        {
            if (listaIdEconomicos == null || !listaIdEconomicos.Any())
            {
                return false;
            }

            using var transaction = _context.Database.BeginTransaction();

            try
            {
                string? nombreArchivoSolo = null;
                string? rutaGuardadaServidor = null;

                string? nombreArchivo2Solo = null;
                string? ruta2GuardadaServidor = null;

                bool tieneArchivo1 = !string.IsNullOrEmpty(rutaOriginal) && File.Exists(rutaOriginal);
                bool tieneArchivo2 = !string.IsNullOrEmpty(rutaOriginal2) && File.Exists(rutaOriginal2);

                if (tieneArchivo1 || tieneArchivo2)
                {
                    string? nombreUnicoRemoto1 = null;
                    string? nombreUnicoRemoto2 = null;

                    if (tieneArchivo1)
                    {
                        nombreArchivoSolo = Path.GetFileName(rutaOriginal);
                        string? extension1 = Path.GetExtension(rutaOriginal);
                        nombreUnicoRemoto1 = $"{Guid.NewGuid()}{extension1}";
                    }

                    if (tieneArchivo2)
                    {
                        nombreArchivo2Solo = Path.GetFileName(rutaOriginal2);
                        string? extension2 = Path.GetExtension(rutaOriginal2);
                        nombreUnicoRemoto2 = $"{Guid.NewGuid()}{extension2}";
                    }

                    var (ruta1, ruta2) = SubirArchivosPorSftp(
                        tieneArchivo1 ? rutaOriginal : null,
                        nombreUnicoRemoto1,
                        tieneArchivo2 ? rutaOriginal2 : null,
                        nombreUnicoRemoto2
                    );

                    rutaGuardadaServidor = ruta1;
                    ruta2GuardadaServidor = ruta2;
                }

                foreach (var idEconomico in listaIdEconomicos)
                {
                    var modeloDb = new CatalogoMovimientosEconomico
                    {
                        IdEconomico = idEconomico,
                        IdUbicacionLlegada = idUbicacionLlegada,
                        IdUbicacionSalida = idUbicacionSalida,
                        FechaMovimiento = fechaMovimiento,
                        NombreArchivo = nombreArchivoSolo,
                        Archivo = rutaGuardadaServidor,
                        NombreArchivo2 = nombreArchivo2Solo,
                        Archivo2 = ruta2GuardadaServidor,
                        IdUsuario = idUsuarioOperativo
                    };

                    _context.CatalogoMovimientosEconomicos.Add(modeloDb);

                    var equipo = _context.CatalogoEconomicos.FirstOrDefault(e => e.IdEconomico == idEconomico);
                    if (equipo != null)
                    {
                        equipo.IdUbicacion = idUbicacionLlegada;
                    }
                    _context.SaveChanges();
                    int idMovimiento = modeloDb.IdMovimiento;
                    _logsService.RegistrarMovimientoEquipo(idUsuarioOperativo, idMovimiento);
                }

                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception($"Error al registrar movimientos: {ex.InnerException?.Message ?? ex.Message}", ex);
            }
        }

        public List<CatalogoMovimientosEconomico> ObtenerHistorial()
        {
            var resultado = _context.CatalogoMovimientosEconomicos
                .Include(e => e.IdUbicacionSalidaNavigation)
                .Include(e => e.IdUbicacionLlegadaNavigation)
                .Include(e => e.IdUsuarioNavigation)
                .Include(e => e.IdEconomicoNavigation)
                .ToList();

            return resultado;
        }

        public string? ObtenerArchivoTemporalDesdeSftp(string rutaRemotaServidor)
        {
            if (string.IsNullOrEmpty(rutaRemotaServidor))
            {
                return null;
            }

            try
            {
                string nombreArchivo = Path.GetFileName(rutaRemotaServidor);
                string rutaTemporalLocal = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}_{nombreArchivo}");

                using (var client = new SftpClient(_hostUbuntu, _usuarioSsh, _contrasenaSsh))
                {
                    client.Connect();

                    if (client.Exists(rutaRemotaServidor))
                    {
                        using (var fileStream = File.Create(rutaTemporalLocal))
                        {
                            client.DownloadFile(rutaRemotaServidor, fileStream);
                        }

                        client.Disconnect();
                        return rutaTemporalLocal;
                    }

                    client.Disconnect();
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener el archivo del servidor: {ex.Message}", ex);
            }
        }

        private (string? rutaRemota1, string? rutaRemota2) SubirArchivosPorSftp(
            string? rutaLocal1,
            string? nombreUnicoRemoto1,
            string? rutaLocal2,
            string? nombreUnicoRemoto2)
        {
            string? rutaRemotaCompleta1 = null;
            string? rutaRemotaCompleta2 = null;

            using (var client = new SftpClient(_hostUbuntu, _usuarioSsh, _contrasenaSsh))
            {
                client.Connect();

                if (!client.Exists(_directorioRemoto))
                {
                    client.CreateDirectory(_directorioRemoto);
                }

                if (!string.IsNullOrEmpty(rutaLocal1) && !string.IsNullOrEmpty(nombreUnicoRemoto1))
                {
                    rutaRemotaCompleta1 = $"{_directorioRemoto.TrimEnd('/')}/{nombreUnicoRemoto1}";
                    using (var fileStream1 = new FileStream(rutaLocal1, FileMode.Open, FileAccess.Read))
                    {
                        client.UploadFile(fileStream1, rutaRemotaCompleta1);
                    }
                }

                if (!string.IsNullOrEmpty(rutaLocal2) && !string.IsNullOrEmpty(nombreUnicoRemoto2))
                {
                    rutaRemotaCompleta2 = $"{_directorioRemoto.TrimEnd('/')}/{nombreUnicoRemoto2}";
                    using (var fileStream2 = new FileStream(rutaLocal2, FileMode.Open, FileAccess.Read))
                    {
                        client.UploadFile(fileStream2, rutaRemotaCompleta2);
                    }
                }

                client.Disconnect();
            }

            return (rutaRemotaCompleta1, rutaRemotaCompleta2);
        }
    }
}