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
    // Clase encargada de la gestión física (FTP) y persistencia en BD de archivos adjuntos
    public class GestorArchivosService
    {
        // Instancia del contexto de Entity Framework para realizar operaciones en la BD
        private readonly InventarioContext _context;

        // Configuración de credenciales y ruta base del servidor FTP remoto
        private readonly string _hostFtp = "ftp://170.10.162.13/";
        private readonly string _usuarioFtp = "dbibiano@enlaceferroviario.com";
        private readonly string _contrasenaFtp = "drbr11122003DRBR.";
        private readonly string _directorioRemoto = "servidor/ArchivosEconomicos/";

        // Constructor que recibe e inyecta el contexto de la base de datos
        public GestorArchivosService(InventarioContext context)
        {
            // Asigna la instancia de BD recibida a la variable privada
            _context = context;

            // Construye la ruta de la carpeta temporal en el disco local del sistema operativo
            string rutaTemporalLocal = Path.Combine(Path.GetTempPath(), "ArchivosEconomicosTemp");

            // Verifica si el directorio temporal local no existe en el sistema
            if (!Directory.Exists(rutaTemporalLocal))
            {
                // Crea el directorio temporal local para descargas intermedias
                Directory.CreateDirectory(rutaTemporalLocal);
            }
        }

        // Método auxiliar para limpiar caracteres no válidos de los nombres de archivo
        private string SanitizarNombreArchivo(string nombreOriginal)
        {
            // Reemplaza los espacios en blanco por guiones bajos
            string sinEspacios = nombreOriginal.Replace(" ", "_");

            // Obtiene un arreglo con los caracteres no permitidos para archivos en el SO
            char[] caracteresInvalidos = Path.GetInvalidFileNameChars();

            // Recorre cada carácter inválido presente en el arreglo
            foreach (char c in caracteresInvalidos)
            {
                // Remueve el carácter inválido reemplazándolo por una cadena vacía
                sinEspacios = sinEspacios.Replace(c.ToString(), "");
            }

            // Devuelve el nombre de archivo totalmente limpio y seguro
            return sinEspacios;
        }

        // Método auxiliar para formatear la URL absoluta de destino en el servidor FTP
        private string ConstruirUrlFtp(string servidorBase, string directorio, string nombreArchivo)
        {
            // Elimina barras inclinadas al final del host si existen
            string baseLimpia = servidorBase.TrimEnd('/');

            // Elimina barras inclinadas al inicio/final de la ruta del directorio
            string directorioLimpio = directorio.Trim('/');

            // Retorna la URL estructurada de forma correcta con sus separadores
            return $"{baseLimpia}/{directorioLimpio}/{nombreArchivo}";
        }

        // Método privado genérico para realizar el envío del archivo físico por FTP mediante buffer
        private void SubirArchivoPorFtp(string rutaLocal, string nombreRemoto)
        {
            // Concatena la dirección completa del recurso FTP de destino
            string urlDestino = $"{_hostFtp}{_directorioRemoto}{nombreRemoto}";

            // Crea la petición HTTP/FTP inicial indicando la URL de destino
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(urlDestino);

            // Define el método del protocolo FTP como subida de archivo (UploadFile)
            request.Method = WebRequestMethods.Ftp.UploadFile;

            // Establece las credenciales de autenticación del servidor FTP
            request.Credentials = new NetworkCredential(_usuarioFtp, _contrasenaFtp);

            // Activa la transferencia en modo binario para evitar corrupción de bytes
            request.UseBinary = true;

            // Habilita el modo pasivo para prevenir bloqueos por Firewall del cliente
            request.UsePassive = true;

            // Lee la totalidad de bytes del archivo físico local
            byte[] fileContents = File.ReadAllBytes(rutaLocal);

            // Asigna la longitud del archivo en el encabezado de la petición
            request.ContentLength = fileContents.Length;

            // Abre el flujo de datos de transmisión hacia la red FTP
            using (Stream requestStream = request.GetRequestStream())
            {
                // Escribe los bytes del archivo en el stream de la petición
                requestStream.Write(fileContents, 0, fileContents.Length);
            }
        }

        // Método restaurado que vincula una lista de archivos a un ID de Servicio específico
        public void RegistrarArchivosServicios(List<string> rutasArchivos, int idServicio)
        {
            // Valida si la lista de rutas recibida es nula o no contiene elementos
            if (rutasArchivos == null || !rutasArchivos.Any())
            {
                // Cancela la ejecución si no hay archivos que procesar
                return;
            }

            // Recorre cada una de las rutas locales de archivos pasadas como parámetro
            foreach (string rutaLocal in rutasArchivos)
            {
                // Valida si el archivo físico realmente existe en la ruta del disco
                if (!File.Exists(rutaLocal))
                {
                    // Lanza una excepción si el archivo no es localizado
                    throw new FileNotFoundException($"El archivo no fue encontrado en la ruta especificada: {rutaLocal}");
                }

                // Extrae únicamente el nombre del archivo con su extensión
                string nombreOriginal = Path.GetFileName(rutaLocal);

                // Sanitiza el nombre eliminando espacios y caracteres prohibidos
                string nombreSanitizado = SanitizarNombreArchivo(nombreOriginal);

                // Genera la URL completa de destino dentro del servidor FTP
                string urlFtpFinal = ConstruirUrlFtp(_hostFtp, _directorioRemoto, nombreSanitizado);

                // Transfiere el archivo hacia la ruta del servidor FTP
                SubirArchivoPorFtp(rutaLocal, nombreSanitizado);

                // Instancia la entidad de catálogo para el archivo subido
                var catalogoArchivo = new CatalogoArchivo
                {
                    // Asigna el nombre sanitizado
                    NombreArchivo = nombreSanitizado,
                    // Asigna la URL FTP asociada
                    Archivo = urlFtpFinal,
                    // Establece la fecha de subida del sistema
                    FechaSubida = DateTime.Now
                };

                // Agrega el nuevo registro de catálogo al DBSet correspondiente
                _context.CatalogoArchivos.Add(catalogoArchivo);

                // Guarda cambios para generar de inmediato la clave primaria (IdArchivo)
                _context.SaveChanges();

                // Crea la entidad de relación entre el Servicio y el Archivo
                var relacionServicio = new ServicioArchivo
                {
                    // Asigna la clave del servicio
                    IdServicio = idServicio,
                    // Asigna la clave primaria recién creada del catálogo de archivo
                    IdArchivo = catalogoArchivo.IdArchivo
                };

                // Agrega la relación a la tabla puente ServicioArchivos
                _context.ServicioArchivos.Add(relacionServicio);

                // Impacta la relación en la base de datos
                _context.SaveChanges();
            }
        }

        // Método que procesa múltiples archivos e inserta sus relaciones para múltiples Económicos
        public List<(CatalogoArchivo Archivo, string IdEconomico)> RegistrarArchivosEconomicos(List<string> rutasOriginales, List<string> idsEconomicos)
        {
            // Instancia la lista para almacenar la tupla de datos que se registrarán en los logs
            var registrosLog = new List<(CatalogoArchivo Archivo, string IdEconomico)>();

            // Valida que ambas listas contengan datos válidos
            if (rutasOriginales == null || !rutasOriginales.Any() || idsEconomicos == null || !idsEconomicos.Any())
            {
                // Retorna la lista de logs vacía
                return registrosLog;
            }

            // Recorre cada una de las rutas de archivos recibidas
            foreach (string rutaOriginal in rutasOriginales)
            {
                // Verifica que el archivo a subir exista físicamente
                if (!File.Exists(rutaOriginal))
                {
                    // Lanza excepción de archivo no encontrado
                    throw new FileNotFoundException($"El archivo físico no existe: {rutaOriginal}");
                }

                // Obtiene el nombre del archivo original
                string nombreOriginal = Path.GetFileName(rutaOriginal);

                // Obtiene la extensión del archivo (ej. .pdf, .jpg)
                string extension = Path.GetExtension(rutaOriginal);

                // Genera un nombre único global (GUID) concatenado con su extensión para evitar sobrescribir
                string nombreFisicoUnico = Guid.NewGuid().ToString() + extension;

                // Subir archivo al servidor FTP UNA SOLA VEZ para este archivo
                SubirArchivoPorFtp(rutaOriginal, nombreFisicoUnico);

                // Instancia el nuevo modelo de objeto CatalogoArchivo
                CatalogoArchivo nuevoArchivo = new CatalogoArchivo
                {
                    // Asigna el nombre aleatorizado que se guardó en FTP
                    Archivo = nombreFisicoUnico,
                    // Guarda el nombre legible original
                    NombreArchivo = nombreOriginal,
                    // Establece la fecha de subida en formato UTC
                    FechaSubida = DateTime.UtcNow
                };

                // Agrega la entidad al contexto de EF
                _context.CatalogoArchivos.Add(nuevoArchivo);

                // Asocia el archivo único recién subido con CADA UNO de los económicos seleccionados
                foreach (string idEconomico in idsEconomicos)
                {
                    // Crea la entidad puente
                    EconomicosArchivo nuevaRelacion = new EconomicosArchivo
                    {
                        // Asigna la clave del económico
                        IdEconomico = idEconomico,
                        // Asigna la propiedad de navegación al nuevo archivo registrado
                        IdArchivoNavigation = nuevoArchivo
                    };

                    // Agrega la relación a la tabla intermedia
                    _context.EconomicosArchivos.Add(nuevaRelacion);

                    // Almacena el par de datos en la lista auxiliar para la auditoría de logs
                    registrosLog.Add((nuevoArchivo, idEconomico));
                }
            }

            // Devuelve el listado procesado para ser utilizado posteriormente por el servicio de logs
            return registrosLog;
        }

        // Método básico para subir un archivo único al FTP y devolver su GUID generado
        public string GuardarArchivo(string rutaOriginal)
        {
            // Valida que la ruta origen contenga un archivo válido
            if (!File.Exists(rutaOriginal))
            {
                // Lanza excepción si la ruta no existe
                throw new FileNotFoundException("El archivo de origen no existe en la ruta especificada.");
            }

            // Extrae la extensión del archivo
            string extension = Path.GetExtension(rutaOriginal);

            // Genera un nombre de destino único con formato GUID
            string nuevoNombre = Guid.NewGuid().ToString() + extension;

            // Envía el archivo al servidor FTP
            SubirArchivoPorFtp(rutaOriginal, nuevoNombre);

            // Devuelve el nombre generado
            return nuevoNombre;
        }

        // Método para obtener o descargar un archivo del servidor FTP a la carpeta temporal local
        public string ObtenerRutaAbsoluta(string nombreArchivoBD)
        {
            // Si el nombre viene vacío o nulo retorna cadena vacía
            if (string.IsNullOrWhiteSpace(nombreArchivoBD))
                return string.Empty;

            // Sanitiza para aislar solo el nombre del archivo libre de rutas relativas o absolutas
            string nombreLimpio = Path.GetFileName(nombreArchivoBD);

            // Genera la ruta completa local dentro del directorio temporal del SO
            string rutaLocalTemporal = Path.Combine(Path.GetTempPath(), "ArchivosEconomicosTemp", nombreLimpio);

            // Verifica si el archivo no ha sido descargado localmente antes
            if (!File.Exists(rutaLocalTemporal))
            {
                // Formatea la ruta del host
                string baseHost = _hostFtp.EndsWith("/") ? _hostFtp : _hostFtp + "/";

                // Formatea la ruta del directorio
                string directorio = _directorioRemoto.Trim('/') + "/";

                // Concatena la URL FTP completa del recurso a descargar
                string urlOrigen = $"{baseHost}{directorio}{nombreLimpio}";

                // Inicializa la solicitud web FTP
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(urlOrigen);

                // Especifica la acción de descarga (DownloadFile)
                request.Method = WebRequestMethods.Ftp.DownloadFile;

                // Asigna las credenciales
                request.Credentials = new NetworkCredential(_usuarioFtp, _contrasenaFtp);

                // Habilita el modo de transferencia binaria
                request.UseBinary = true;

                // Habilita el modo pasivo
                request.UsePassive = true;

                // Ejecuta la respuesta del servidor FTP
                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                // Obtiene el flujo de lectura entrante
                using (Stream responseStream = response.GetResponseStream())
                // Crea el archivo local de destino para volcar los datos
                using (FileStream fileStream = File.Create(rutaLocalTemporal))
                {
                    // Copia el contenido del stream remoto al archivo local
                    responseStream.CopyTo(fileStream);
                }
            }

            // Devuelve la ruta física completa del archivo alojado temporalmente en la máquina
            return rutaLocalTemporal;
        }
    }
}