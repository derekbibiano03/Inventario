using Inventario.Core.Services.Economicos;
using Inventario.Core.Services.Logs;
using Inventario.Data.Models;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace Inventario.Desktop.ViewModels.UbicacionProyectosViewModel
{
    public class DetallesViewModel : INotifyPropertyChanged
    {
        // 1. Declaración de dependencias del servicio de archivos y el contexto de base de datos.
        private readonly GestorArchivosService _archivosService;
        private CatalogoEconomico _detalle;
        private ObservableCollection<CatalogoArchivo> _archivosAnexados;

        // Propiedad que almacena el objeto principal con los detalles del equipo
        public CatalogoEconomico Detalle
        {
            get => _detalle;
            set
            {
                _detalle = value;
                OnPropertyChanged();
            }
        }

        // Colección enlazada a la interfaz de usuario para mostrar los nombres de archivos
        public ObservableCollection<CatalogoArchivo> ArchivosAnexados
        {
            get => _archivosAnexados;
            set
            {
                _archivosAnexados = value;
                OnPropertyChanged();
            }
        }

        // Comandos expuestos al XAML
        public ICommand AbrirArchivoCommand { get; }
        public ICommand DescargarArchivoCommand { get; }

        public DetallesViewModel(string idEconomico)
        {
            // 2. Inicialización del servicio encargado de interactuar con el servidor de archivos Ubuntu.
            _archivosService = new GestorArchivosService();

            // Inicializamos comandos usando expresiones Lambda
            AbrirArchivoCommand = new RelayCommand<CatalogoArchivo>(EjecutarAbrirArchivo);
            DescargarArchivoCommand = new RelayCommand<CatalogoArchivo>(EjecutarDescargarArchivo);

            ArchivosAnexados = new ObservableCollection<CatalogoArchivo>();
            CargarDatosCompletos(idEconomico);
        }

        private void CargarDatosCompletos(string idEconomico)
        {
            using (var context = new Data.Models.InventarioContext())
            {
                var contexto = new InventarioContext();
                var logsService = new LogsService(contexto);
                var servicio = new CatalogoEconomicosService(context, logsService);
                Detalle = servicio.ObtenerDetalleCompleto(idEconomico);

                // Si el equipo contiene registros en su tabla intermedia, extraemos los archivos lógicos
                if (Detalle?.EconomicosArchivos != null)
                {
                    var listaArchivos = Detalle.EconomicosArchivos
                        .Where(ae => ae.IdArchivoNavigation != null)
                        .Select(ae => ae.IdArchivoNavigation)
                        .ToList();

                    foreach (var archivo in listaArchivos)
                    {
                        ArchivosAnexados.Add(archivo);
                    }
                }
            }
        }

        // Método ejecutado al dar clic sobre el archivo en la lista
        private void EjecutarAbrirArchivo(CatalogoArchivo archivo)
        {
            // 3. Validación de seguridad básica para evitar procesar objetos nulos o vacíos.
            if (string.IsNullOrEmpty(archivo?.Archivo)) return;

            try
            {
                // 4. El ViewModel le solicita al servicio la ruta absoluta del archivo.
                //    Si usas SFTP, el servicio se conectará a Ubuntu, descargará el archivo de forma oculta a la carpeta temporal de Windows
                //    y nos devolverá la ruta local resultante. Si usas Samba, resolverá la ruta de red de inmediato.
                string rutaCompleta = _archivosService.ObtenerRutaAbsoluta(archivo.Archivo);

                // 5. Validación que confirma que el archivo ahora sí existe físicamente en el entorno local para ser abierto.
                if (!File.Exists(rutaCompleta))
                {
                    MessageBox.Show($"No se pudo recuperar el archivo físico desde el servidor de almacenamiento.",
                                    "Archivo No Encontrado", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // 6. Ejecuta el proceso de inicio del sistema operativo para abrir el archivo con su visor predeterminado (ej: Adobe Reader o Navegador).
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = rutaCompleta,
                    UseShellExecute = true // Requerido en .NET Core / .NET 5+ para que use el visor predeterminado del sistema.
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al abrir el archivo: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EjecutarDescargarArchivo(CatalogoArchivo archivo)
        {
            // 7. Validación de seguridad básica para evitar errores de referencia nula.
            if (string.IsNullOrEmpty(archivo?.Archivo)) return;

            try
            {
                // 8. Solicita la ruta local al servicio. Si no está en caché temporal, la descarga desde Ubuntu en tiempo real de forma transparente.
                string rutaCompletaOrigen = _archivosService.ObtenerRutaAbsoluta(archivo.Archivo);

                if (!File.Exists(rutaCompletaOrigen))
                {
                    MessageBox.Show($"No se pudo recuperar el archivo de origen desde el servidor.",
                                    "Error de Origen", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // 9. Configuración y visualización del cuadro de diálogo para salvar el archivo en la ruta elegida por el usuario.
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    FileName = archivo.NombreArchivo, // Nombre original descriptivo del archivo (ej: "Ficha_Tecnica.pdf")
                    DefaultExt = Path.GetExtension(archivo.NombreArchivo),
                    Filter = $"Archivos ({Path.GetExtension(archivo.NombreArchivo)})|*{Path.GetExtension(archivo.NombreArchivo)}|Todos los archivos (*.*)|*.*"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    // 10. Realiza la copia del archivo temporal localmente recuperado hacia la carpeta elegida por el usuario (ej: Descargas, Escritorio).
                    File.Copy(rutaCompletaOrigen, saveFileDialog.FileName, true);
                    MessageBox.Show("Archivo descargado y guardado exitosamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar el archivo: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}