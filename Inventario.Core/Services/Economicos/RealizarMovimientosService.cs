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
    public class RealizarMovimientosService
    {
        private readonly InventarioContext _context;
        private readonly LogsService _logsService;

        public RealizarMovimientosService(InventarioContext context, LogsService logsService)
        {
            _context = context;
            _logsService = logsService;
        }

        private readonly string _hostServidor = "170.10.162.13";
        private readonly string _usuarioFtp = "dbibiano@enlaceferroviario.com";
        private readonly string _contrasenaFtp = "drbr11122003DRBR.";
        private readonly string _directorioRemoto = "servidor/ArchivosEconomicos/MovimientosEconomicos";

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

                    var (ruta1, ruta2) = SubirArchivosPorFtpNativo(
                        tieneArchivo1 ? rutaOriginal : null,
                        nombreUnicoRemoto1,
                        tieneArchivo2 ? rutaOriginal2 : null,
                        nombreUnicoRemoto2
                    );

                    rutaGuardadaServidor = ruta1;
                    ruta2GuardadaServidor = ruta2;
                }

                var movimientosNuevos = new List<CatalogoMovimientosEconomico>();

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
                    movimientosNuevos.Add(modeloDb);

                    var equipo = _context.CatalogoEconomicos.FirstOrDefault(e => e.IdEconomico == idEconomico);
                    if (equipo != null)
                    {
                        equipo.IdUbicacion = idUbicacionLlegada;
                    }
                }

                // Se ejecuta una sola vez para guardar todos los movimientos y actualizaciones de golpe
                _context.SaveChanges();

                // Registrar los logs una vez guardados los IDs generados
                foreach (var movimiento in movimientosNuevos)
                {
                    _logsService.RegistrarMovimientoEquipo(idUsuarioOperativo, movimiento.IdMovimiento);
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


        public bool ModificarMovimiento(
            int idMovimiento,
            int idUsuarioOperativo,
            int idUbicacionLlegada,
            int idUbicacionSalida,
            DateTime fechaMovimiento,
            string? nuevaRutaLocal1,
            string? nuevaRutaLocal2,
            bool conservarArchivo1,
            bool conservarArchivo2)
        {
            using var transaction = _context.Database.BeginTransaction();

            try
            {
                var movimientoDb = _context.CatalogoMovimientosEconomicos.FirstOrDefault(m => m.IdMovimiento == idMovimiento);
                if (movimientoDb == null)
                {
                    return false;
                }

                // --- GESTIÓN ARCHIVO 1 ---
                if (!conservarArchivo1)
                {
                    // Si no se desea conservar, eliminamos el anterior del FTP si existe
                    if (!string.IsNullOrEmpty(movimientoDb.Archivo))
                    {
                        EliminarArchivoFtpNativo(movimientoDb.Archivo);
                    }
                    movimientoDb.Archivo = null;
                    movimientoDb.NombreArchivo = null;

                    // Si hay una nueva ruta local para reemplazarlo
                    if (!string.IsNullOrEmpty(nuevaRutaLocal1) && File.Exists(nuevaRutaLocal1))
                    {
                        string extension1 = Path.GetExtension(nuevaRutaLocal1);
                        string nombreUnico1 = $"{Guid.NewGuid()}{extension1}";
                        string rutaRemota1 = $"{_directorioRemoto.TrimEnd('/')}/{nombreUnico1}";

                        SubirUnArchivoFtpNativo(nuevaRutaLocal1, rutaRemota1);

                        movimientoDb.Archivo = rutaRemota1;
                        movimientoDb.NombreArchivo = Path.GetFileName(nuevaRutaLocal1);
                    }
                }

                // --- GESTIÓN ARCHIVO 2 ---
                if (!conservarArchivo2)
                {
                    // Si no se desea conservar, eliminamos el anterior del FTP si existe
                    if (!string.IsNullOrEmpty(movimientoDb.Archivo2))
                    {
                        EliminarArchivoFtpNativo(movimientoDb.Archivo2);
                    }
                    movimientoDb.Archivo2 = null;
                    movimientoDb.NombreArchivo2 = null;

                    // Si hay una nueva ruta local para reemplazarlo
                    if (!string.IsNullOrEmpty(nuevaRutaLocal2) && File.Exists(nuevaRutaLocal2))
                    {
                        string extension2 = Path.GetExtension(nuevaRutaLocal2);
                        string nombreUnico2 = $"{Guid.NewGuid()}{extension2}";
                        string rutaRemota2 = $"{_directorioRemoto.TrimEnd('/')}/{nombreUnico2}";

                        SubirUnArchivoFtpNativo(nuevaRutaLocal2, rutaRemota2);

                        movimientoDb.Archivo2 = rutaRemota2;
                        movimientoDb.NombreArchivo2 = Path.GetFileName(nuevaRutaLocal2);
                    }
                }

                // Actualizar propiedades del movimiento
                movimientoDb.IdUbicacionLlegada = idUbicacionLlegada;
                movimientoDb.IdUbicacionSalida = idUbicacionSalida;
                movimientoDb.FechaMovimiento = fechaMovimiento;
                movimientoDb.IdUsuario = idUsuarioOperativo;

                _context.SaveChanges();
                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception($"Error al modificar el movimiento: {ex.InnerException?.Message ?? ex.Message}", ex);
            }
        }

        public bool EliminarMovimiento(int idMovimiento)
        {
            using var transaction = _context.Database.BeginTransaction();

            try
            {
                var movimientoDb = _context.CatalogoMovimientosEconomicos.FirstOrDefault(m => m.IdMovimiento == idMovimiento);
                if (movimientoDb == null)
                {
                    return false;
                }

                // Eliminar archivos físicos asociados del servidor FTP
                if (!string.IsNullOrEmpty(movimientoDb.Archivo))
                {
                    EliminarArchivoFtpNativo(movimientoDb.Archivo);
                }

                if (!string.IsNullOrEmpty(movimientoDb.Archivo2))
                {
                    EliminarArchivoFtpNativo(movimientoDb.Archivo2);
                }

                _context.CatalogoMovimientosEconomicos.Remove(movimientoDb);
                _context.SaveChanges();
                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception($"Error al eliminar el movimiento: {ex.InnerException?.Message ?? ex.Message}", ex);
            }
        }

        private void EliminarArchivoFtpNativo(string rutaRemota)
        {
            try
            {
                string urlFtp = $"ftp://{_hostServidor.Trim('/')}/{rutaRemota.TrimStart('/')}";
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(urlFtp);
                request.Method = WebRequestMethods.Ftp.DeleteFile;
                request.Credentials = new NetworkCredential(_usuarioFtp, _contrasenaFtp);

                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse()) { }
            }
            catch (WebException)
            {
                // Si el archivo ya no existe en el servidor, ignoramos el error para no bloquear la operación de BD
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
            return ObtenerArchivoTemporalDesdeFtp(rutaRemotaServidor);
        }

        public string? ObtenerArchivoTemporalDesdeFtp(string rutaRemotaServidor)
        {
            if (string.IsNullOrEmpty(rutaRemotaServidor))
            {
                return null;
            }

            try
            {
                string nombreArchivo = Path.GetFileName(rutaRemotaServidor);
                string rutaTemporalLocal = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}_{nombreArchivo}");

                string urlFtp = $"ftp://{_hostServidor.Trim('/')}/{rutaRemotaServidor.TrimStart('/')}";
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(urlFtp);
                request.Method = WebRequestMethods.Ftp.DownloadFile;
                request.Credentials = new NetworkCredential(_usuarioFtp, _contrasenaFtp);

                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                using (Stream responseStream = response.GetResponseStream())
                using (FileStream fileStream = File.Create(rutaTemporalLocal))
                {
                    responseStream.CopyTo(fileStream);
                }

                return rutaTemporalLocal;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener el archivo del servidor FTP: {ex.Message}", ex);
            }
        }

        private (string? rutaRemota1, string? rutaRemota2) SubirArchivosPorFtpNativo(
            string? rutaLocal1,
            string? nombreUnicoRemoto1,
            string? rutaLocal2,
            string? nombreUnicoRemoto2)
        {
            string? rutaRemotaCompleta1 = null;
            string? rutaRemotaCompleta2 = null;

            CrearDirectorioFtpNativo(_directorioRemoto);

            if (!string.IsNullOrEmpty(rutaLocal1) && !string.IsNullOrEmpty(nombreUnicoRemoto1))
            {
                rutaRemotaCompleta1 = $"{_directorioRemoto.TrimEnd('/')}/{nombreUnicoRemoto1}";
                SubirUnArchivoFtpNativo(rutaLocal1, rutaRemotaCompleta1);
            }

            if (!string.IsNullOrEmpty(rutaLocal2) && !string.IsNullOrEmpty(nombreUnicoRemoto2))
            {
                rutaRemotaCompleta2 = $"{_directorioRemoto.TrimEnd('/')}/{nombreUnicoRemoto2}";
                SubirUnArchivoFtpNativo(rutaLocal2, rutaRemotaCompleta2);
            }

            return (rutaRemotaCompleta1, rutaRemotaCompleta2);
        }

        private void SubirUnArchivoFtpNativo(string rutaLocal, string rutaRemota)
        {
            string urlFtp = $"ftp://{_hostServidor.Trim('/')}/{rutaRemota.TrimStart('/')}";
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(urlFtp);
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = new NetworkCredential(_usuarioFtp, _contrasenaFtp);

            byte[] fileContents = File.ReadAllBytes(rutaLocal);
            request.ContentLength = fileContents.Length;

            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(fileContents, 0, fileContents.Length);
            }
        }

        private void CrearDirectorioFtpNativo(string directorio)
        {
            try
            {
                string urlFtp = $"ftp://{_hostServidor.Trim('/')}/{directorio.TrimStart('/')}";
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(urlFtp);
                request.Method = WebRequestMethods.Ftp.MakeDirectory;
                request.Credentials = new NetworkCredential(_usuarioFtp, _contrasenaFtp);

                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse()) { }
            }
            catch (WebException ex)
            {
                if (ex.Response is FtpWebResponse response && response.StatusCode != FtpStatusCode.ActionNotTakenFileUnavailable)
                {
                    throw;
                }
            }
        }
    }
}