using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Inventario.Data.Models;

namespace Inventario.Core.Services.Economicos
{
    public class GestorArchivosService
    {
        private readonly string _directorioAlmacenamiento;

        public GestorArchivosService()
        {
            string rutaPersonalizada = @"D:\ArchivosEconomicos";
            _directorioAlmacenamiento = rutaPersonalizada;

            if (!Directory.Exists(_directorioAlmacenamiento))
            {
                Directory.CreateDirectory(_directorioAlmacenamiento);
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
            string rutaDestinoCompleta = Path.Combine(_directorioAlmacenamiento, nombreFisicoUnico);

            File.Copy(rutaOriginal, rutaDestinoCompleta, true);

            CatalogoArchivo nuevoArchivo = new CatalogoArchivo
            {
                Archivo = nombreFisicoUnico,
                NombreArchivo = nombreOriginal,
                FechaSubida = DateTime.Now
            };

            EconomicosArchivo nuevaRelacion = new EconomicosArchivo
            {
                IdEconomico = idEconomico
            };

            return (nuevoArchivo, nuevaRelacion);
        }

        public string GuardarArchivo(string rutaOriginal)
        {
            if (!File.Exists(rutaOriginal))
            {
                throw new FileNotFoundException("El archivo de origen no existe en la ruta especificada.");
            }
            string extension = Path.GetExtension(rutaOriginal);
            string nuevoNombre = Guid.NewGuid().ToString() + extension;
            string rutaDestinoCompleta = Path.Combine(_directorioAlmacenamiento, nuevoNombre);
            File.Copy(rutaOriginal, rutaDestinoCompleta, true);
            return nuevoNombre;
        }

        public string ObtenerRutaAbsoluta(string nombreArchivoBD)
        {
            return Path.Combine(_directorioAlmacenamiento, nombreArchivoBD);
        }
    }
}