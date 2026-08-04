using Inventario.Core.Services.Logs;
using Inventario.Data.Models;
using Renci.SshNet;
using System;
using System.IO;

namespace Inventario.Core.Services.Economicos
{
    public class GestorArchivosService
    {

        private readonly string _hostUbuntu = "192.168.0.24";
        private readonly string _usuarioSsh = "admin_bibiano";
        private readonly string _contrasenaSsh = "11122003drbr";
        private readonly string _directorioRemoto = "/var/www/ArchivosEconomicos/";

        public GestorArchivosService()
        {
            string rutaTemporalLocal = Path.Combine(Path.GetTempPath(), "ArchivosEconomicosTemp");
            if (!Directory.Exists(rutaTemporalLocal))
            {
                Directory.CreateDirectory(rutaTemporalLocal);
            }

        }

        public (CatalogoArchivo archivoCatalogado, EconomicosArchivo relacionEconomico) RegistrarArchivoEconomico( string rutaOriginal, string idEconomico)
        {
            if (!File.Exists(rutaOriginal))
            {
                throw new FileNotFoundException("El archivo físico seleccionado no existe en la ruta proporcionada.");
            }

            string nombreOriginal = Path.GetFileName(rutaOriginal);
            string extension = Path.GetExtension(rutaOriginal);
            string nombreFisicoUnico = Guid.NewGuid().ToString() + extension;

            SubirArchivoPorSftp(rutaOriginal, nombreFisicoUnico);

            // 1. Crear el objeto del catálogo de archivos
            CatalogoArchivo nuevoArchivo = new CatalogoArchivo
            {
                Archivo = nombreFisicoUnico,
                NombreArchivo = nombreOriginal,
                FechaSubida = DateTime.UtcNow
            };

            // 2. Crear la relación asignando 'IdArchivoNavigation' (o 'ArchivoNavigation') al objeto nuevoArchivo
            EconomicosArchivo nuevaRelacion = new EconomicosArchivo
            {
                IdEconomico = idEconomico,
                IdArchivoNavigation = nuevoArchivo // Entity Framework vincula el IdArchivo generado automáticamente

            };

            return (nuevoArchivo, nuevaRelacion);
        }

        private void SubirArchivoPorSftp(string rutaLocal, string nombreRemoto)
        {
            using (var client = new SftpClient(_hostUbuntu, _usuarioSsh, _contrasenaSsh))
            {
                client.Connect();

                if (!client.Exists(_directorioRemoto))
                {
                    client.CreateDirectory(_directorioRemoto);
                }

                using (var fileStream = new FileStream(rutaLocal, FileMode.Open))
                {
                    client.UploadFile(fileStream, Path.Combine(_directorioRemoto, nombreRemoto));
                }

                client.Disconnect();
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

            SubirArchivoPorSftp(rutaOriginal, nuevoNombre);

            return nuevoNombre;
        }

        public string ObtenerRutaAbsoluta(string nombreArchivoBD)
        {
            string rutaLocalTemporal = Path.Combine(Path.GetTempPath(), "ArchivosEconomicosTemp", nombreArchivoBD);
            if (!File.Exists(rutaLocalTemporal))
            {
                using (var client = new SftpClient(_hostUbuntu, _usuarioSsh, _contrasenaSsh))
                {
                    client.Connect();
                    using (var fileStream = File.OpenWrite(rutaLocalTemporal))
                    {
                        client.DownloadFile(Path.Combine(_directorioRemoto, nombreArchivoBD), fileStream);
                    }
                    client.Disconnect();
                }
            }
            return rutaLocalTemporal;
        }
    }
}