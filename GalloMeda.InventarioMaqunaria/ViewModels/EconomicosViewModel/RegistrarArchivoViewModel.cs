using Inventario.Core.DTOs;
using Inventario.Core.Services.Economicos;
using Inventario.Core.Services.Logs;
using Inventario.Data.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Inventario.Desktop.ViewModels.EconomicosViewModel
{
    public class RegistrarArchivoViewModel : INotifyPropertyChanged
    {
        private readonly RealizarMovimientosService _movimientosService;
        private readonly InventarioContext _contextoBD;

        public ObservableCollection<CatalogoEconomico> ListaEconomicos { get; set; } = new ObservableCollection<CatalogoEconomico>();
        public ICollectionView EconomicosFiltrados { get; set; }

        private string _textoBusquedaId = string.Empty;
        public string TextoBusquedaId
        {
            get => _textoBusquedaId;
            set
            {
                _textoBusquedaId = value;
                OnPropertyChanged();
                EconomicosFiltrados?.Refresh();
            }
        }

        private string _rutaArchivoOriginal = string.Empty;
        public string RutaArchivoOriginal
        {
            get => _rutaArchivoOriginal;
            set
            {
                _rutaArchivoOriginal = value;
                OnPropertyChanged();
                NombreArchivoOriginal = string.IsNullOrEmpty(value) ? "" : Path.GetFileName(value);
                (GuardarArchivoCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        private string _nombreArchivoOriginal = string.Empty;
        public string NombreArchivoOriginal
        {
            get => _nombreArchivoOriginal;
            set { _nombreArchivoOriginal = value; OnPropertyChanged(); }
        }

        private string _rutaArchivoOriginal2 = string.Empty;
        public string RutaArchivoOriginal2
        {
            get => _rutaArchivoOriginal2;
            set
            {
                _rutaArchivoOriginal2 = value;
                OnPropertyChanged();
                NombreArchivoOriginal2 = string.IsNullOrEmpty(value) ? "" : Path.GetFileName(value);
            }
        }

        private string _nombreArchivoOriginal2 = string.Empty;
        public string NombreArchivoOriginal2
        {
            get => _nombreArchivoOriginal2;
            set { _nombreArchivoOriginal2 = value; OnPropertyChanged(); }
        }

        private int _idUsuarioActual = 0;
        public int IdUsuarioActual
        {
            get => _idUsuarioActual;
            set { _idUsuarioActual = value; OnPropertyChanged(); }
        }

        private int _idUbicacionSalida;
        public int IdUbicacionSalida
        {
            get => _idUbicacionSalida;
            set { _idUbicacionSalida = value; OnPropertyChanged(); }
        }

        private int _idUbicacionLlegada;
        public int IdUbicacionLlegada
        {
            get => _idUbicacionLlegada;
            set { _idUbicacionLlegada = value; OnPropertyChanged(); }
        }

        private DateTime _fechaMovimiento = DateTime.Now;
        public DateTime FechaMovimiento
        {
            get => _fechaMovimiento;
            set { _fechaMovimiento = value; OnPropertyChanged(); }
        }

        public ICommand SeleccionarArchivoCommand { get; }
        public ICommand SeleccionarArchivo2Command { get; }
        public ICommand GuardarArchivoCommand { get; }
        public ICollectionView VistaHistorialMovimientos { get; set; }

        public ObservableCollection<CatalogoMovimientosEconomico> MovimientosEconomicos { get; set; } = new ObservableCollection<CatalogoMovimientosEconomico>();

        public RegistrarArchivoViewModel()
        {
            _contextoBD = new InventarioContext();
            var logsService = new LogsService(_contextoBD);
            _movimientosService = new RealizarMovimientosService(_contextoBD, logsService);

            SeleccionarArchivoCommand = new RelayCommand(EjecutarSeleccionarArchivo);
            SeleccionarArchivo2Command = new RelayCommand(EjecutarSeleccionarArchivo2);
            GuardarArchivoCommand = new RelayCommand(EjecutarGuardarArchivo, CanGuardarArchivo);

            VistaHistorialMovimientos = CollectionViewSource.GetDefaultView(MovimientosEconomicos);

            CargarEconomicosDesdeBD();

            EconomicosFiltrados = CollectionViewSource.GetDefaultView(ListaEconomicos)!;
            EconomicosFiltrados.Filter = FiltroBusquedaId;
        }

        private void CargarEconomicosDesdeBD()
        {
            try
            {
                var listaBD = _contextoBD.CatalogoEconomicos.ToList();
                ListaEconomicos = new ObservableCollection<CatalogoEconomico>(listaBD);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar Económicos: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                ListaEconomicos = new ObservableCollection<CatalogoEconomico>();
            }
        }

        private bool FiltroBusquedaId(object? obj)
        {
            if (string.IsNullOrEmpty(TextoBusquedaId)) return true;

            if (obj is CatalogoEconomico economico && economico.IdEconomico != null)
            {
                return economico.IdEconomico.IndexOf(TextoBusquedaId, StringComparison.OrdinalIgnoreCase) >= 0;
            }
            return false;
        }

        private void EjecutarSeleccionarArchivo()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Archivos permitidos (*.pdf;*.png;*.jpg;*.tif;*.JPEG)|*.pdf;*.png;*.jpg;*.tif;*.JPEG",
                Title = "Seleccione el documento principal"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                RutaArchivoOriginal = openFileDialog.FileName;
            }
        }

        private void EjecutarSeleccionarArchivo2()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Archivos permitidos (*.pdf;*.png;*.jpg;*.tif;*.JPEG)|*.pdf;*.png;*.jpg;*.tif;*.JPEG",
                Title = "Seleccione el segundo documento (Opcional)"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                RutaArchivoOriginal2 = openFileDialog.FileName;
            }
        }

        private void EjecutarGuardarArchivo()
        {
            try
            {
                List<string> idsSeleccionados = ListaEconomicos
                    .Where(x => x.IsSelected)
                    .Select(x => x.IdEconomico)
                    .Where(id => id != null)
                    .Select(id => id!)
                    .ToList();

                bool resultado = _movimientosService.RegistrarMovimientosMultiples(
                    IdUsuarioActual,
                    idsSeleccionados,
                    IdUbicacionLlegada,
                    IdUbicacionSalida,
                    FechaMovimiento,
                    RutaArchivoOriginal,
                    RutaArchivoOriginal2
                );

                if (resultado)
                {
                    MessageBox.Show($"Movimientos registrados exitosamente para {idsSeleccionados.Count} económicos.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    LimpiarFormulario();
                }
                else
                {
                    MessageBox.Show("No se realizaron cambios o la lista de económicos estaba vacía.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                string mensajeReal = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                MessageBox.Show($"Error al guardar: {mensajeReal}", "Error Crítico", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanGuardarArchivo()
        {
            return !string.IsNullOrEmpty(RutaArchivoOriginal) && ListaEconomicos != null && ListaEconomicos.Any(x => x.IsSelected);
        }

        private void LimpiarFormulario()
        {
            RutaArchivoOriginal = string.Empty;
            RutaArchivoOriginal2 = string.Empty;
            TextoBusquedaId = string.Empty;

            foreach (var economico in ListaEconomicos)
            {
                economico.IsSelected = false;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}