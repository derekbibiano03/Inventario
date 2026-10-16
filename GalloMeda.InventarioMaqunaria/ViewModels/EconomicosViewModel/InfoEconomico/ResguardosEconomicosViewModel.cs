using GalloMeda.InventarioMaqunaria;
using Inventario.Core.DTOs;
using Inventario.Core.Services.Economicos;
using Inventario.Core.Services.Logs;
using Inventario.Core.Services.Personal;
using Inventario.Data.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace Inventario.Desktop.ViewModels.EconomicosViewModel.InfoEconomico
{
    internal class ResguardosEconomicosViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private DateTime? _fechaResguardo = DateTime.Today;
        public DateTime? FechaResguardo
        {
            get => _fechaResguardo;
            set
            {
                _fechaResguardo = value;
                OnPropertyChanged();
            }
        }

        private string? _idEconomico;
        public string? IdEconomico
        {
            get => _idEconomico;
            set { _idEconomico = value; OnPropertyChanged(); }
        }

        private string? _observaciones;
        public string? Observaciones
        {
            get => _observaciones;
            set { _observaciones = value; OnPropertyChanged(); }
        }

        private int? _idOperador;
        public int? IdOperador
        {
            get => _idOperador;
            set { _idOperador = value; OnPropertyChanged(); }
        }

        private readonly InventarioContext _context;
        private readonly LogsService _logsService;
        private readonly EmpleadoService _empleadosService;
        private readonly CatalogoEconomicosService _economicosService;
        private readonly ResguardosService _resguardosService;
        private readonly GestorArchivosService _gestorArchivosService;

        private int _idResguardo;
        public int IdResguardo
        {
            get => _idResguardo;
            set { _idResguardo = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Empleado> Empleados { get; set; }
        public ObservableCollection<EconomicoMinimoDto> Economicos { get; set; }
        public ObservableCollection<string> ListaArchivosSeleccionados { get; set; } = new ObservableCollection<string>();

        public ICommand SeleccionarArchivosCommand { get; }
        public ICommand EliminarArchivoCommand { get; }
        public ICommand GuardarArchivoCommand { get; }

        public ResguardosEconomicosViewModel(
            InventarioContext context,
            EmpleadoService empleadoService,
            CatalogoEconomicosService economicosService,
            ResguardosService resguardosService,
            LogsService logsService)
        {
            _context = context;
            _empleadosService = empleadoService;
            _economicosService = economicosService;
            _resguardosService = resguardosService;
            _logsService = logsService;
            _gestorArchivosService = new GestorArchivosService(_context);

            Empleados = new ObservableCollection<Empleado>();
            Economicos = new ObservableCollection<EconomicoMinimoDto>();

            SeleccionarArchivosCommand = new RelayCommand(EjecutarSeleccionarArchivos);
            EliminarArchivoCommand = new RelayCommand<string>(EjecutarEliminarArchivo);
            GuardarArchivoCommand = new RelayCommand(AltaResguardo);

            CargarTipos();
        }

        public void CargarTipos()
        {
            var datosRes = _empleadosService.ObtenerOperadores();
            foreach (var responsable in datosRes) { Empleados.Add(responsable); }

            var datosEco = _economicosService.ObtenerIDs();
            foreach (var economico in datosEco) { Economicos.Add(economico); }
        }

        private void EjecutarSeleccionarArchivos()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Archivos permitidos (*.pdf;*.png;*.jpg;*.tif;*.JPEG)|*.pdf;*.png;*.jpg;*.tif;*.JPEG",
                Title = "Seleccione los documentos o imágenes del resguardo",
                Multiselect = true
            };

            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string ruta in openFileDialog.FileNames)
                {
                    if (!ListaArchivosSeleccionados.Contains(ruta))
                    {
                        ListaArchivosSeleccionados.Add(ruta);
                    }
                }
            }
        }

        private void EjecutarEliminarArchivo(string? ruta)
        {
            if (!string.IsNullOrEmpty(ruta) && ListaArchivosSeleccionados.Contains(ruta))
            {
                ListaArchivosSeleccionados.Remove(ruta);
            }
        }

        public void AltaResguardo()
        {
            try
            {
                // 1. Validaciones
                if (!FechaResguardo.HasValue)
                {
                    MessageBox.Show("Debe seleccionar una fecha válida.", "Error de Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(IdEconomico))
                {
                    MessageBox.Show("Debe seleccionar un número económico.", "Error de Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // 2. Construcción del DTO de HistorialResguardo
                var dto = new HistorialResguardo
                {
                    IdEconomico = this.IdEconomico,
                    IdOperador = this.IdOperador,
                    FechaResguardo = FechaResguardo.HasValue ? DateOnly.FromDateTime(FechaResguardo.Value) : null,
                    Observaciones = this.Observaciones
                };

                int usuarioLog = App.Session?.IdUsuario ?? 0;

                // 3. Registro del Resguardo en Base de Datos usando tu servicio
                HistorialResguardo resguardoCreado = _resguardosService.registrarResguardo(usuarioLog, dto);

                // 4. Procesamiento de Archivos Adjuntos (si los hay)
                if (ListaArchivosSeleccionados.Any())
                {
                    var registrosProcesados = _gestorArchivosService.RegistrarArchivosResguardos(
                        ListaArchivosSeleccionados.ToList(),
                        resguardoCreado.IdResguardo
                    );

                    foreach (var item in registrosProcesados)
                    {
                        _logsService.RegistrarResguardoEquipo(usuarioLog, resguardoCreado.IdResguardo);
                    }
                }

                MessageBox.Show("Se registró el resguardo y sus archivos de manera exitosa.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                // 5. Limpieza de Formulario
                ListaArchivosSeleccionados.Clear();
                Observaciones = string.Empty;
                IdEconomico = null;
                IdOperador = null;
            }
            catch (Exception ex)
            {
                string mensajeError = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                MessageBox.Show(mensajeError, "Error de Base de Datos / FTP", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}