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
    public class GestorArchivosService
    {
        private readonly InventarioContext _context; 

        
        private readonly string _hostFtp = "ftp://170.10.162.13/";
        private readonly string _usuarioFtp = "dbibiano@enlaceferroviario.com"; // O usuario principal de cPanel 'irvinglunap'
        private readonly string _contrasenaFtp = "drbr11122003DRBR.";

        private readonly string _directorioRemoto = "servidor/ArchivosEconomicos/";

        public GestorArchivosService(InventarioContext context) 
        { 
            _context = context; 

            string rutaTemporalLocal = Path.Combine(Path.GetTempPath(), "ArchivosEconomicosTemp"); // Define la ruta temporal en disco local.
            if (!Directory.Exists(rutaTemporalLocal)) 
            { 
                Directory.CreateDirectory(rutaTemporalLocal); 
            } 
        } 

        private string SanitizarNombreArchivo(string nombreOriginal) 
        { 
            string sinEspacios = nombreOriginal.Replace(" ", "_"); 

            char[] caracteresInvalidos = Path.GetInvalidFileNameChars(); 
            foreach (char c in caracteresInvalidos) 
            { 
                sinEspacios = sinEspacios.Replace(c.ToString(), ""); 
            } 

            return sinEspacios; 
        } 

        private string ConstruirUrlFtp(string servidorBase, string directorio, string nombreArchivo) 
        { 
            string baseLimpia = servidorBase.TrimEnd('/'); 
            string directorioLimpio = directorio.Trim('/'); 
            return $"{baseLimpia}/{directorioLimpio}/{nombreArchivo}"; 
        } 

        private void SubirArchivoServidorFtp(string rutaLocal, string urlFtpDestino) 
        { 
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(urlFtpDestino); 
            request.Method = WebRequestMethods.Ftp.UploadFile; 
            request.Credentials = new NetworkCredential(_usuarioFtp, _contrasenaFtp); 
            request.UsePassive = true; 
            request.UseBinary = true; 
            request.KeepAlive = false; 

            using (FileStream fileStream = File.OpenRead(rutaLocal)) 
            using (Stream requestStream = request.GetRequestStream()) 
            { 
                fileStream.CopyTo(requestStream); 
            } 

            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse()) 
            { 
                
            } 
        } 

        public void RegistrarArchivosServicios(List<string> rutasArchivos, int idServicio) 
        { 
            if (rutasArchivos == null || !rutasArchivos.Any()) 
            { 
                return; 
            } 

            foreach (string rutaLocal in rutasArchivos) 
            { 
                if (!File.Exists(rutaLocal)) 
                { 
                    throw new FileNotFoundException($"El archivo no fue encontrado en la ruta especificada: {rutaLocal}"); 
                } 

                string nombreOriginal = Path.GetFileName(rutaLocal); 
                string nombreSanitizado = SanitizarNombreArchivo(nombreOriginal); 

                
                string urlFtpFinal = ConstruirUrlFtp(_hostFtp, _directorioRemoto, nombreSanitizado); 

                SubirArchivoServidorFtp(rutaLocal, urlFtpFinal); 

                var catalogoArchivo = new CatalogoArchivo 
                { 
                    NombreArchivo = nombreSanitizado, 
                    Archivo = urlFtpFinal, 
                    FechaSubida = DateTime.Now 
                }; 

                _context.CatalogoArchivos.Add(catalogoArchivo); 
                _context.SaveChanges(); 

                var relacionServicio = new ServicioArchivo 
                { 
                    IdServicio = idServicio, 
                    IdArchivo = catalogoArchivo.IdArchivo 
                }; 

                _context.ServicioArchivos.Add(relacionServicio); 
                _context.SaveChanges(); 
            } 
        } 

        public (CatalogoArchivo archivoCatalogado, EconomicosArchivo relacionEconomico) RegistrarArchivoEconomico(string rutaOriginal, string idEconomico)
        {
            if (!File.Exists(rutaOriginal))
            {
                throw new FileNotFoundException("El archivo físico seleccionado no existe en la ruta proporcionada.");
            }

            string nombreOriginal = Path.GetFileName(rutaOriginal);
            string extension = Path.GetExtension(rutaOriginal);
            string nombreFisicoUnico = Guid.NewGuid().ToString() + extension;

            SubirArchivoPorFtp(rutaOriginal, nombreFisicoUnico);

            
            CatalogoArchivo nuevoArchivo = new CatalogoArchivo
            {
                Archivo = nombreFisicoUnico,
                NombreArchivo = nombreOriginal,
                FechaSubida = DateTime.UtcNow
            };

            
            EconomicosArchivo nuevaRelacion = new EconomicosArchivo
            {
                IdEconomico = idEconomico,
                IdArchivoNavigation = nuevoArchivo
            };

            return (nuevoArchivo, nuevaRelacion);
        }

        private void SubirArchivoPorFtp(string rutaLocal, string nombreRemoto)
        {
            string urlDestino = $"{_hostFtp}{_directorioRemoto}{nombreRemoto}";

            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(urlDestino);
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = new NetworkCredential(_usuarioFtp, _contrasenaFtp);
            request.UseBinary = true;
            request.UsePassive = true; // Modo pasivo recomendado para evitar bloqueos de firewall

            byte[] fileContents = File.ReadAllBytes(rutaLocal);
            request.ContentLength = fileContents.Length;

            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(fileContents, 0, fileContents.Length);
            }
        }

        public string GuardarArchivo(string rutaOriginal)
        {
            if (!File.Exists(rutaOriginal))
            {
                throw new FileNotFoundException("El archivo de origen no existe en la ruta especificada.");
            }
            string extension = Path.GetExtension(rutaOriginal);
            string nuevoNombre = Guid.NewGuid().ToString() + extension;

            SubirArchivoPorFtp(rutaOriginal, nuevoNombre);

            return nuevoNombre;
        }

        public string ObtenerRutaAbsoluta(string nombreArchivoBD)
        {
            if (string.IsNullOrWhiteSpace(nombreArchivoBD))
                return string.Empty;

            // Extrae solo el nombre del archivo en caso de que nombreArchivoBD sea una URL completa o una ruta local
            string nombreLimpio = Path.GetFileName(nombreArchivoBD);

            string rutaLocalTemporal = Path.Combine(Path.GetTempPath(), "ArchivosEconomicosTemp", nombreLimpio);

            if (!File.Exists(rutaLocalTemporal))
            {
                string baseHost = _hostFtp.EndsWith("/") ? _hostFtp : _hostFtp + "/";
                string directorio = _directorioRemoto.Trim('/') + "/";
                string urlOrigen = $"{baseHost}{directorio}{nombreLimpio}";

                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(urlOrigen);
                request.Method = WebRequestMethods.Ftp.DownloadFile;
                request.Credentials = new NetworkCredential(_usuarioFtp, _contrasenaFtp);
                request.UseBinary = true;
                request.UsePassive = true;

                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                using (Stream responseStream = response.GetResponseStream())
                using (FileStream fileStream = File.Create(rutaLocalTemporal))
                {
                    responseStream.CopyTo(fileStream);
                }
            }

            return rutaLocalTemporal;
        }
    }
}