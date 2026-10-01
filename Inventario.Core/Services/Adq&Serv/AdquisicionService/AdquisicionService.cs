using ClosedXML.Excel;
using Inventario.Core.DTOs.Requisicion;
using Inventario.Data.Models;
using Microsoft.EntityFrameworkCore;

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;

namespace Inventario.Core.Services.Adq_Serv.AdquisicionService
{
    public class AdquisicionService
    {
        private readonly InventarioContext _context;

        public AdquisicionService(InventarioContext context)
        {
            _context = context;
        }
        private DateTime _fechaActual = DateTime.Now;

        private void OnPropertyChanged()
        {
            throw new NotImplementedException();
        }

        private readonly string _hostFtp = "ftp://170.10.162.13/";
        private readonly string _usuarioFtp = "dbibiano@enlaceferroviario.com";
        private readonly string _contrasenaFtp = "drbr11122003DRBR.";
        private readonly string _directorioRemoto = "servidor/ArchivosEconomicos/Requisiciones";

        private void SubirArchivoPorFtp(string rutaLocal, string nombreRemoto)
        {
            // Asegurar que la ruta remota termine en '/' y el nombre no empiece con '/'
            string directorioLimpio = _directorioRemoto.TrimEnd('/');
            string archivoLimpio = nombreRemoto.TrimStart('/');

            string urlDestino = $"{_hostFtp.TrimEnd('/')}/{directorioLimpio}/{archivoLimpio}";

            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(urlDestino);
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = new NetworkCredential(_usuarioFtp, _contrasenaFtp);
            request.UseBinary = true;
            request.UsePassive = true;

            byte[] fileContents = File.ReadAllBytes(rutaLocal);
            request.ContentLength = fileContents.Length;

            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(fileContents, 0, fileContents.Length);
            }
        }

        public bool NuevaRequisicion(int idSolicitante, int idAutorizante,
                                     int idUbicacion, DateOnly fechaRequisicion, string tipoRequisicion,
                                     string empresa, string estatus, string rutaArchivoLocal, string nombreArchivoRemoto)
        {
            try
            {
                if (!string.IsNullOrEmpty(rutaArchivoLocal) && File.Exists(rutaArchivoLocal))
                {
                    SubirArchivoPorFtp(rutaArchivoLocal, nombreArchivoRemoto);
                }
                else
                {
                    throw new FileNotFoundException("El archivo local a subir no existe o la ruta está vacía.");
                }
                int ultimoConsecutivo = _context.Requisiciones
                    .Where(r => r.Empresa == empresa && r.IdUbicacion == idUbicacion)
                    .Select(r => (int?)r.Consecutivo)
                    .Max() ?? 0;
                int siguienteConsecutivo = ultimoConsecutivo + 1;
                string idRequisicionGenerado = $"{empresa} - RC - {siguienteConsecutivo:D3} - {idUbicacion} - {_fechaActual:yyyy}";
                var nuevaRequisicion = new Requisicione
                {
                    IdRequisicion = idRequisicionGenerado,
                    IdUbicacion = idUbicacion,
                    Consecutivo = siguienteConsecutivo,
                    FechaRequisicion = fechaRequisicion,
                    TipoRequisicion = tipoRequisicion,
                    Empresa = empresa,
                    Estatus = estatus,
                    ArchivoReq = nombreArchivoRemoto,
                    IdSolicitante = idSolicitante,
                    IdAutorizante = idAutorizante
                };
                _context.Requisiciones.Add(nuevaRequisicion);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al procesar la nueva requisición: {ex.Message}");
            }
        }

        public async Task<List<RequisicionListDto>> ObtenerRequisicionesAsync(int idUsuario)
        {
            return await _context.Requisiciones
                // 1. El filtro va ANTES del Select y utilizando los IDs numéricos de la base de datos
                .Where(r => r.IdAutorizante == idUsuario && r.Estatus == "EN ESPERA") // O ajusta si es por solicitante: r.IdSolicitante == idUsuario
                .Include(r => r.IdSolicitanteNavigation)
                .Include(r => r.IdAutorizanteNavigation)
                .OrderByDescending(r => r.FechaRequisicion)
                .Select(r => new RequisicionListDto
                {
                    IdRequisicion = r.IdRequisicion,
                    FechaRequisicion = (DateOnly)r.FechaRequisicion,
                    Empresa = r.Empresa,
                    Estatus = r.Estatus,
                    Solicitante = r.IdSolicitanteNavigation != null ? r.IdSolicitanteNavigation.NombreCompleto : "N/A",
                    Autorizante = r.IdAutorizanteNavigation != null ? r.IdAutorizanteNavigation.NombreCompleto : "N/A",
                    ArchivoReq = r.ArchivoReq
                })
                .ToListAsync();
        }

        public string AutorizarYDescargarArchivo(string idRequisicion, string nombreArchivoRemoto, string firmaPath)
        {
            try
            {
                if (string.IsNullOrEmpty(nombreArchivoRemoto))
                {
                    throw new Exception("El nombre del archivo remoto es nulo o está vacío.");
                }

                string directorioLimpio = _directorioRemoto.TrimEnd('/');
                string archivoLimpio = nombreArchivoRemoto.TrimStart('/');
                string urlRemota = $"{_hostFtp.TrimEnd('/')}/{directorioLimpio}/{archivoLimpio}";

                string soloNombreArchivo = Path.GetFileName(nombreArchivoRemoto);
                string rutaLocalTemporal = Path.Combine(Path.GetTempPath(), soloNombreArchivo);

                // 1. Descargar el archivo Excel original desde el FTP
                using (WebClient client = new WebClient())
                {
                    client.Credentials = new NetworkCredential(_usuarioFtp, _contrasenaFtp);
                    client.DownloadFile(urlRemota, rutaLocalTemporal);
                }

                // 2. Modificar el Excel con ClosedXML para insertar la imagen de la firma
                using (var workbook = new XLWorkbook(rutaLocalTemporal))
                {
                    var ws = workbook.Worksheet(1);

                    if (!string.IsNullOrWhiteSpace(firmaPath))
                    {
                        string urlFirmaWeb = $"http://enlaceferroviario.com{firmaPath}";

                        using (var webClient = new WebClient())
                        {
                            webClient.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
                            byte[] imageBytes = webClient.DownloadData(urlFirmaWeb);

                            if (imageBytes != null && imageBytes.Length > 0)
                            {
                                using (var ms = new MemoryStream(imageBytes))
                                {
                                    var image = ws.AddPicture(ms);
                                    image.Name = "FirmaAutorizante";
                                    image.MoveTo(ws.Cell("E66"));
                                    image.WithSize(140, 50);
                                }
                            }
                        }
                    }

                    workbook.Save();
                }

                // 3. Volver a subir el archivo actualizado al servidor FTP
                SubirArchivoPorFtp(rutaLocalTemporal, nombreArchivoRemoto);

                // 4. Actualizar el estatus en la Base de Datos a "AUTORIZADA"
                var requisicionDb = _context.Requisiciones.FirstOrDefault(r => r.IdRequisicion == idRequisicion);
                if (requisicionDb != null)
                {
                    requisicionDb.Estatus = "AUTORIZADA";
                    _context.SaveChanges();
                }

                return rutaLocalTemporal;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al autorizar el archivo: {ex.Message}", ex);
            }
        }

        public void DescargarYAbrirArchivoExcel(string nombreArchivoRemoto)
        {
            try
            {
                if (string.IsNullOrEmpty(nombreArchivoRemoto))
                {
                    throw new ArgumentException("El nombre del archivo remoto es nulo o está vacío.", nameof(nombreArchivoRemoto));
                }

                string directorioLimpio = _directorioRemoto.TrimEnd('/');
                string archivoLimpio = nombreArchivoRemoto.TrimStart('/');
                string urlRemota = $"{_hostFtp.TrimEnd('/')}/{directorioLimpio}/{archivoLimpio}";

                // Extraer solo el nombre del archivo por si viene con rutas relativas para evitar conflictos en la ruta temporal
                string soloNombreArchivo = Path.GetFileName(nombreArchivoRemoto);
                string rutaLocalTemporal = Path.Combine(Path.GetTempPath(), soloNombreArchivo);

                using (WebClient client = new WebClient())
                {
                    client.Credentials = new NetworkCredential(_usuarioFtp, _contrasenaFtp);
                    client.DownloadFile(urlRemota, rutaLocalTemporal);
                }

                if (File.Exists(rutaLocalTemporal))
                {
                    Process.Start(new ProcessStartInfo(rutaLocalTemporal) { UseShellExecute = true });
                }
                else
                {
                    throw new FileNotFoundException("No se pudo descargar el archivo temporal desde el FTP.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error en DescargarYAbrirArchivoExcel: {ex.Message}", ex);
            }
        }


    }
}