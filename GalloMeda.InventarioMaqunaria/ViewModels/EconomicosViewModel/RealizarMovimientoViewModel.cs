using GalloMeda.InventarioMaqunaria;
using Inventario.Core.Services.Auth;
using Inventario.Core.Services.Catalogos;
using Inventario.Core.Services.Economicos;
using Inventario.Core.Services.UbicacionProyecto;
using Inventario.Data.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using System.Windows.Input;

namespace Inventario.Desktop.ViewModels.EconomicosViewModel
{
    public class RealizarMovimientoViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        // Modifica 'string' por 'string?' agregando el signo de interrogación
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ICommand RegistrarMovimientoCommand { get; }
        public ICommand SeleccionarArchivoCommand { get; }
        public ICommand SeleccionarArchivo2Command { get; }
        public ICollectionView VistaHistorialMovimientos { get; set; }

        private readonly UbicacionProyeectoService _ubicacionService;
        private readonly CatalogoEconomicosService _economicosService;
        private readonly RealizarMovimientosService _realizarMovimientosService;

        private List<CatalogoEconomico> _economicosCargadosUbicacion;

        public ObservableCollection<CatalogoUbicacionesProyecto> UbicacionesFiltro { get; set; }
        public ObservableCollection<CatalogoUbicacionesProyecto> UbicacionesFin { get; set; }
        public ObservableCollection<CatalogoEconomico> EconomicosFiltrados { get; set; }
        public ObservableCollection<CatalogoMovimientosEconomico> MovimientosEconomicos { get; set; }

        private int _idUbicacionFiltroSeleccionado;
        public int IdUbicacionFiltroSeleccionado
        {
            get => _idUbicacionFiltroSeleccionado;
            set
            {
                if (_idUbicacionFiltroSeleccionado != value)
                {
                    _idUbicacionFiltroSeleccionado = value;
                    OnPropertyChanged();
                    CargarEconomicosPorUbicacion();
                }
            }
        }

        private DateTime _fechaSalida = DateTime.UtcNow;
        public DateTime FechaSalida
        {
            get => _fechaSalida;
            set { _fechaSalida = value; OnPropertyChanged(); }
        }

        private  int _idUbicacionFinSeleccionado ;
        public int IdUbicacionFinSeleccionado
        {
            get => _idUbicacionFinSeleccionado;
            set
            {
                if (_idUbicacionFinSeleccionado != value)
                {
                    _idUbicacionFinSeleccionado = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _textoBusquedaId = string.Empty;
        public string TextoBusquedaId
        {
            get => _textoBusquedaId;
            set
            {
                if (_textoBusquedaId != value)
                {
                    _textoBusquedaId = value;
                    OnPropertyChanged();
                    AplicarFiltroTexto();
                }
            }
        }

        private string _rutaArchivoAdjunto = string.Empty;
        public string RutaArchivoAdjunto
        {
            get => _rutaArchivoAdjunto;
            set { _rutaArchivoAdjunto = value; OnPropertyChanged(); }
        }

        private string _rutaArchivoAdjunto2 = string.Empty;
        public string RutaArchivoAdjunto2
        {
            get => _rutaArchivoAdjunto2;
            set { _rutaArchivoAdjunto2 = value; OnPropertyChanged(); }
        }

        public ICommand VerArchivoCommand { get; }

        public RealizarMovimientoViewModel(
            UbicacionProyeectoService ubicacionService,
            CatalogoEconomicosService economicosService,
            RealizarMovimientosService realizarMovimientosService)
        {
            // Asignación de servicios pasados por inyección de dependencias
            _ubicacionService = ubicacionService;
            _economicosService = economicosService;
            _realizarMovimientosService = realizarMovimientosService;

            // Inicialización de colecciones para evitar referencias nulas
            _economicosCargadosUbicacion = new List<CatalogoEconomico>();
            UbicacionesFiltro = new ObservableCollection<CatalogoUbicacionesProyecto>();
            UbicacionesFin = new ObservableCollection<CatalogoUbicacionesProyecto>();
            EconomicosFiltrados = new ObservableCollection<CatalogoEconomico>();

            // Inicialización del comando de registro y archivos
            RegistrarMovimientoCommand = new RelayCommand(EjecutarRegistrarMovimiento);
            SeleccionarArchivoCommand = new RelayCommand(EjecutarSeleccionarArchivo);
            SeleccionarArchivo2Command = new RelayCommand(EjecutarSeleccionarArchivo2);

            // 1. Instanciar la ObservableCollection PRIMERO antes de crear la vista de filtrado
            MovimientosEconomicos = new ObservableCollection<CatalogoMovimientosEconomico>();

            // 2. Vincular la vista predeterminada a la colección ya instanciada
            VistaHistorialMovimientos = CollectionViewSource.GetDefaultView(MovimientosEconomicos);
            VerArchivoCommand = new RelayCommand<string>(EjecutarVerArchivo);

            // 3. Cargar catalogos iniciales
            CargarTipos();

            // 4. Cargar el historial de movimientos desde el servicio a la tabla
            CargarHistorialMovimientos();
        }

        // Método dedicado a obtener y actualizar los registros del historial en la vista
        public void CargarHistorialMovimientos()
        {
            // Llama al servicio para traer los datos de la base de datos
            var datosHistorial = _realizarMovimientosService.ObtenerHistorial();

            // Limpia la colección observable para evitar datos duplicados
            MovimientosEconomicos.Clear();

            // Recorre la lista retornada por el servicio y la inserta en la colección de la UI
            foreach (var item in datosHistorial)
            {
                MovimientosEconomicos.Add(item);
            }

            // Refresca la vista de la colección para notificar los cambios al DataGrid
            VistaHistorialMovimientos?.Refresh();
        }

        private void CargarTipos()
        {
            var datosUbi = _ubicacionService.ObtenerUbicaciones();

            UbicacionesFiltro.Clear();
            UbicacionesFin.Clear();

            foreach (var datoU in datosUbi)
            {
                UbicacionesFiltro.Add(datoU);
                UbicacionesFin.Add(datoU);
            }
        }


        private void EjecutarVerArchivo(string rutaRemota)
        {
            // Valida que la ruta remota no venga vacía
            if (string.IsNullOrEmpty(rutaRemota))
            {
                System.Windows.MessageBox.Show("El movimiento no cuenta con este archivo cargado.", "Advertencia", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Solicita al servicio descargar el archivo a la carpeta temporal local
                string? rutaTemporal = _realizarMovimientosService.ObtenerArchivoTemporalDesdeSftp(rutaRemota);

                // Si el archivo se descargó correctamente
                if (!string.IsNullOrEmpty(rutaTemporal) && System.IO.File.Exists(rutaTemporal))
                {
                    // Configura el proceso para abrir el archivo con su programa asociado en Windows
                    var psi = new ProcessStartInfo
                    {
                        FileName = rutaTemporal, // Ruta del archivo PDF/imagen
                        UseShellExecute = true  // Ejecuta la aplicación asociada por defecto en el sistema operativo
                    };

                    Process.Start(psi); // Abre el archivo
                }
                else
                {
                    System.Windows.MessageBox.Show("No se encontró el archivo en el servidor remoto.", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error al intentar visualizar el archivo: {ex.Message}", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private void CargarEconomicosPorUbicacion()
        {
            EconomicosFiltrados.Clear();
            _economicosCargadosUbicacion.Clear();

            if (IdUbicacionFiltroSeleccionado == 0)
            {
                return;
            }

            var datosEconomicos = _economicosService.ObtenerEconomicosPorUbicacion(IdUbicacionFiltroSeleccionado);
            _economicosCargadosUbicacion = datosEconomicos;

            AplicarFiltroTexto();
        }

        private void AplicarFiltroTexto()
        {
            EconomicosFiltrados.Clear();

            if (_economicosCargadosUbicacion == null || !_economicosCargadosUbicacion.Any())
            {
                return;
            }

            IEnumerable<CatalogoEconomico> resultado = _economicosCargadosUbicacion;

            if (!string.IsNullOrWhiteSpace(TextoBusquedaId))
            {
                resultado = resultado.Where(e => e.IdEconomico != null &&
                                                 e.IdEconomico.Contains(TextoBusquedaId, StringComparison.OrdinalIgnoreCase));
            }

            foreach (var item in resultado)
            {
                EconomicosFiltrados.Add(item);
            }
        }

        private void EjecutarSeleccionarArchivo()
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Archivos PDF (*.pdf)|*.pdf|Todos los archivos (*.*)|*.*",
                Title = "Seleccionar Carta Porte / Archivo"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                RutaArchivoAdjunto = openFileDialog.FileName;
            }
        }

        private void EjecutarSeleccionarArchivo2()
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Archivos PDF (*.pdf)|*.pdf|Todos los archivos (*.*)|*.*",
                Title = "Seleccionar Carta Porte / Archivo"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                RutaArchivoAdjunto2 = openFileDialog.FileName;
            }
        }

        public void EjecutarRegistrarMovimiento()
        {
            if (IdUbicacionFinSeleccionado == 0 || IdUbicacionFiltroSeleccionado == 0)
            {
                return;
            }

            var economicosSeleccionados = _economicosCargadosUbicacion.Where(e => e.IsSelected).ToList();

            if (!economicosSeleccionados.Any())
            {
                return;
            }

            bool resultadoBatch = _realizarMovimientosService.RegistrarMovimientosMultiples(
                App.Session.IdUsuario,
                economicosSeleccionados.Select(e => e.IdEconomico).ToList(),
                this.IdUbicacionFinSeleccionado,
                this.IdUbicacionFiltroSeleccionado,
                this.FechaSalida,
                this.RutaArchivoAdjunto,
                this.RutaArchivoAdjunto2
            );

            if (resultadoBatch)
            {
                CargarEconomicosPorUbicacion();
                RutaArchivoAdjunto = string.Empty;
                RutaArchivoAdjunto2 = string.Empty;
            }
        }
    }
}