using GalloMeda.InventarioMaqunaria;
using Inventario.Core.DTOs;
using Inventario.Core.Services.Economicos;
using Inventario.Data.Models;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Inventario.Desktop.ViewModels.EconomicosViewModel
{
    public class HistorialServicioViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ICommand AltaServicioCommand { get; }
        public ICommand SeleccionarArchivosCommand { get; }
        public ICommand EliminarArchivoCommand { get; }
        public ICommand VerArchivoCommand { get; }

        private ICollectionView _vistaHistorialServicios;
        public ICollectionView VistaHistorialServicios
        {
            get => _vistaHistorialServicios;
            set { _vistaHistorialServicios = value; OnPropertyChanged(); }
        }

        private readonly CatalogoEconomicosService _economicosService;
        private readonly HistorialServicioService _historialServicio;
        private readonly GestorArchivosService _gestorArchivosService;

        public ObservableCollection<EconomicoMinimoDto> Economicos { get; set; }
        public ObservableCollection<HistorialServicio> HistorialServicio { get; set; }
        public ObservableCollection<string> ArchivosSeleccionados { get; set; }

        // PROPIEDAD PARA EL FILTRO DEL DATA GRID
        private string _filtroNoEconomico = string.Empty;
        public string FiltroNoEconomico
        {
            get => _filtroNoEconomico;
            set
            {
                _filtroNoEconomico = value;
                OnPropertyChanged();
                // Notifica a la vista que debe re-evaluar la condición del filtro
                VistaHistorialServicios?.Refresh();
            }
        }

        private string _noEconomico = null;
        public string NoEconomico
        {
            get => _noEconomico;
            set { _noEconomico = value; OnPropertyChanged(); }
        }

        private DateTime? _fechaMantenimiento = DateTime.Today;
        public DateTime? FechaMantenimiento
        {
            get => _fechaMantenimiento;
            set { _fechaMantenimiento = value; OnPropertyChanged(); }
        }

        private string _tipoMantenimiento = null;
        public string TipoMantenimiento
        {
            get => _tipoMantenimiento;
            set { _tipoMantenimiento = value; OnPropertyChanged(); }
        }

        private string _anotaciones = null;
        public string Anotaciones
        {
            get => _anotaciones;
            set { _anotaciones = value; OnPropertyChanged(); }
        }

        private string _horaskilometrosreales = null;
        public string Horaskilometrosreales
        {
            get => _horaskilometrosreales;
            set { _horaskilometrosreales = value; OnPropertyChanged(); }
        }

        public HistorialServicioViewModel(CatalogoEconomicosService economicosService,
                                          HistorialServicioService historialServiciosService,
                                          GestorArchivosService gestorArchivosService)
        {
            _economicosService = economicosService;
            _historialServicio = historialServiciosService;
            _gestorArchivosService = gestorArchivosService;

            Economicos = new ObservableCollection<EconomicoMinimoDto>();
            HistorialServicio = new ObservableCollection<HistorialServicio>();
            ArchivosSeleccionados = new ObservableCollection<string>();

            // ASIGNACIÓN Y CONFIGURACIÓN DEL PREDICADO DE FILTRADO
            VistaHistorialServicios = CollectionViewSource.GetDefaultView(HistorialServicio);
            VistaHistorialServicios.Filter = FiltrarPorEconomico;

            AltaServicioCommand = new RelayCommand(AltaServicio);
            SeleccionarArchivosCommand = new RelayCommand(SeleccionarArchivos);
            EliminarArchivoCommand = new RelayCommand<string>(EliminarArchivo);
            VerArchivoCommand = new RelayCommand<CatalogoArchivo>(EjecutarAbrirArchivo);

            CargarTipos();
            CargarHistorialMovimientos();
        }

        // MÉTODO PREDICADO QUE FILTRA CADA REGISTRO DE LA LISTA
        private bool FiltrarPorEconomico(object obj)
        {
            // Valida que el objeto sea una entidad de tipo HistorialServicio
            if (obj is not HistorialServicio servicio) return false;

            // Si la caja de texto está vacía o es espacio en blanco, muestra todos los registros
            if (string.IsNullOrWhiteSpace(FiltroNoEconomico)) return true;

            // Valida que el NoEconomico del servicio contenga el texto digitado (ignora mayúsculas/minúsculas)
            return servicio.NoEconomico != null &&
                   servicio.NoEconomico.Contains(FiltroNoEconomico, StringComparison.OrdinalIgnoreCase);
        }

        public void CargarHistorialMovimientos()
        {
            var datoshistorial = _historialServicio.ObtenerHistorial();
            HistorialServicio.Clear();

            foreach (var item in datoshistorial)
            {
                HistorialServicio.Add(item);
            }
            VistaHistorialServicios?.Refresh();
        }

        private void EjecutarAbrirArchivo(CatalogoArchivo archivo)
        {
            if (string.IsNullOrEmpty(archivo?.Archivo)) return;
            try
            {
                string rutaCompleta = _gestorArchivosService.ObtenerRutaAbsoluta(archivo.Archivo);

                if (!File.Exists(rutaCompleta))
                {
                    MessageBox.Show($"No se pudo recuperar el archivo físico desde el servidor de almacenamiento.",
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

        public void CargarTipos()
        {
            var datoseconomicos = _economicosService.ObtenerEconomicosCortos();
            Economicos.Clear();
            foreach (var economico in datoseconomicos)
            {
                Economicos.Add(economico);
            }
        }

        private void SeleccionarArchivos()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "Todos los archivos (*.*)|*.*|Documentos PDF (*.pdf)|*.pdf|Imágenes (*.jpg;*.png)|*.jpg;*.png"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string ruta in openFileDialog.FileNames)
                {
                    if (!ArchivosSeleccionados.Contains(ruta))
                    {
                        ArchivosSeleccionados.Add(ruta);
                    }
                }
            }
        }

        private void EliminarArchivo(string? ruta)
        {
            if (!string.IsNullOrEmpty(ruta) && ArchivosSeleccionados.Contains(ruta))
            {
                ArchivosSeleccionados.Remove(ruta);
            }
        }

        public void AltaServicio()
        {
            try
            {
                if (!FechaMantenimiento.HasValue)
                {
                    MessageBox.Show("Debe seleccionar una fecha válida.", "Error de Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(NoEconomico))
                {
                    MessageBox.Show("Debe seleccionar un número económico.", "Error de Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var dto = new HistorialServicio
                {
                    NoEconomico = this.NoEconomico,
                    FechaMantenimiento = DateOnly.FromDateTime(this.FechaMantenimiento.Value),
                    TipoMantenimiento = this.TipoMantenimiento,
                    Anotaciones = this.Anotaciones,
                    Horaskilometrosreales = this.Horaskilometrosreales
                };

                HistorialServicio servicioCreado = _historialServicio.RegistrarServicio(App.Session.IdUsuario, dto);

                if (ArchivosSeleccionados.Count > 0)
                {
                    List<string> listaRutas = ArchivosSeleccionados.ToList();
                    _gestorArchivosService.RegistrarArchivosServicios(listaRutas, servicioCreado.IdServicio);
                }

                MessageBox.Show("Se registró el Servicio y sus archivos de manera exitosa", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                ArchivosSeleccionados.Clear();
                Anotaciones = string.Empty;
                Horaskilometrosreales = string.Empty;

                CargarHistorialMovimientos();
            }
            catch (Exception ex)
            {
                string mensajeError = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                MessageBox.Show(mensajeError, "Error de Base de Datos / FTP", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}