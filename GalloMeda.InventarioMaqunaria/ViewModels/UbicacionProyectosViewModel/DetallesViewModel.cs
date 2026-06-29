using Inventario.Core.Services.Economicos;
using Inventario.Data.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;

namespace Inventario.Desktop.ViewModels.UbicacionProyectosViewModel
{
    public class DetallesViewModel : INotifyPropertyChanged
    {
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
                var servicio = new CatalogoEconomicosService(context);
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
        private readonly string _directorioBase = @"C:\Users\DEREK\source\repos\GalloMeda.InventarioMaqunaria\GalloMeda.InventarioMaqunaria\bin\Debug\net10.0-windows\ArchivosSistema";

        private void EjecutarAbrirArchivo(CatalogoArchivo archivo)
        {
            if (string.IsNullOrEmpty(archivo?.Archivo)) return;

            try
            {
                // Combinamos la carpeta raíz con el nombre del archivo guardado en la BD
                string rutaCompleta = Path.Combine(_directorioBase, archivo.Archivo);

                if (!File.Exists(rutaCompleta))
                {
                    MessageBox.Show($"No se encontró el archivo físico en la ruta consolidada:\n{rutaCompleta}",
                                    "Archivo No Encontrado", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = rutaCompleta,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al abrir el archivo: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EjecutarDescargarArchivo(CatalogoArchivo archivo)
        {
            if (string.IsNullOrEmpty(archivo?.Archivo)) return;

            string rutaCompletaOrigen = Path.Combine(_directorioBase, archivo.Archivo);

            if (!File.Exists(rutaCompletaOrigen))
            {
                MessageBox.Show($"El archivo original no existe en el almacenamiento local:\n{rutaCompletaOrigen}",
                                "Error de Origen", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                FileName = archivo.NombreArchivo, // Nombre amigable original (ej: "Ficha_Tecnica.pdf")
                DefaultExt = Path.GetExtension(archivo.NombreArchivo),
                Filter = $"Archivos ({Path.GetExtension(archivo.NombreArchivo)})|*{Path.GetExtension(archivo.NombreArchivo)}|Todos los archivos (*.*)|*.*"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    File.Copy(rutaCompletaOrigen, saveFileDialog.FileName, true);
                    MessageBox.Show("Archivo copiado exitosamente a la ruta destino.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al guardar el archivo: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}