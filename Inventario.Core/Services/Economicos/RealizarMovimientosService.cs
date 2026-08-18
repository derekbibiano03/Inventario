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