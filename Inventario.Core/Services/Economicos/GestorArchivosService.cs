using System;
using System.IO;
using Inventario.Data.Models;
using Renci.SshNet; // Librería requerida para habilitar transferencias por SSH/SFTP de forma nativa.

namespace Inventario.Core.Services.Economicos
{
    public class GestorArchivosService
    {
        // 1. Declaración de variables privadas que definen las credenciales seguras de conexión SSH a tu servidor.
        private readonly string _hostUbuntu = "192.168.0.24"; // Reemplaza por la IP de tu nuevo servidor Ubuntu.
        private readonly string _usuarioSsh = "admin_bibiano"; // El usuario del sistema operativo de tu servidor.
        private readonly string _contrasenaSsh = "11122003drbr"; // La contraseña SSH del usuario en Ubuntu.
        private readonly string _directorioRemoto = "/var/www/ArchivosEconomicos/"; // Ruta física real dentro de tu disco de Ubuntu.

        public GestorArchivosService()
        {
            // 2. Al inicializar el servicio en Windows, creamos de manera local la carpeta temporal en caso de que necesitemos descargas.
            string rutaTemporalLocal = Path.Combine(Path.GetTempPath(), "ArchivosEconomicosTemp");
            if (!Directory.Exists(rutaTemporalLocal))
            {
                Directory.CreateDirectory(rutaTemporalLocal);
            }
        }

        public (CatalogoArchivo archivoCatalogado, EconomicosArchivo relacionEconomico) RegistrarArchivoEconomico(string rutaOriginal, string idEconomico)
        {
            // 3. Validación que verifica que el archivo exista en la computadora del cliente.
            if (!File.Exists(rutaOriginal))
            {
                throw new FileNotFoundException("El archivo físico seleccionado no existe en la ruta proporcionada.");
            }

            // 4. Extracción de los componentes del nombre y extensión original.
            string nombreOriginal = Path.GetFileName(rutaOriginal);
            string extension = Path.GetExtension(rutaOriginal);
            string nombreFisicoUnico = Guid.NewGuid().ToString() + extension;

            // 5. Subida del archivo físico encriptado al servidor usando SFTP.
            SubirArchivoPorSftp(rutaOriginal, nombreFisicoUnico);

            // 6. Creación de la entidad del catálogo con el identificador único del servidor.
            CatalogoArchivo nuevoArchivo = new CatalogoArchivo
            {
                Archivo = nombreFisicoUnico,
                NombreArchivo = nombreOriginal,
                FechaSubida = DateTime.Now
            };

            // 7. Instanciación de la relación.
            EconomicosArchivo nuevaRelacion = new EconomicosArchivo
            {
                IdEconomico = idEconomico
            };

            return (nuevoArchivo, nuevaRelacion);
        }

        private void SubirArchivoPorSftp(string rutaLocal, string nombreRemoto)
        {
            // 8. Inicialización del cliente SFTP conectándose al servidor remoto usando los datos de red y credenciales.
            using (var client = new SftpClient(_hostUbuntu, _usuarioSsh, _contrasenaSsh))
            {
                // 9. Establece formalmente la conexión segura SSH cifrada.
                client.Connect();

                // 10. Verifica si la carpeta de destino final no existe dentro de la jerarquía física de Ubuntu para crearla.
                if (!client.Exists(_directorioRemoto))
                {
                    client.CreateDirectory(_directorioRemoto);
                }

                // 11. Abre el archivo local de la computadora Windows como un flujo de lectura binario secuencial.
                using (var fileStream = new FileStream(rutaLocal, FileMode.Open))
                {
                    // 12. Combina la ruta remota de Linux con el nombre del archivo final y sube el flujo de datos de forma segura.
                    client.UploadFile(fileStream, Path.Combine(_directorioRemoto, nombreRemoto));
                }

                // 13. Desconecta de forma limpia la sesión cifrada liberando recursos de red en ambos servidores.
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
            // 14. Cuando el software pida el archivo desde WPF, se descargará del servidor Ubuntu al directorio temporal local de Windows.
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