using Inventario.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq;

namespace Inventario.Desktop.ViewModels.AdqServ
{
    public class AgregarRequisicionViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public ObservableCollection<DetalleRequisicion> Detalles { get; } = new();

        public ICommand AñadirConceptoCommand { get; }
        public ICommand EliminarConceptoCommand { get; }
        private readonly InventarioContext _contexto;
        public ObservableCollection<CatalogoUbicacionesProyecto> Ubicaciones { get; }
        public ObservableCollection<CatalogoEconomico> Economicos { get; }
        private string? _ubicacionSeleccionada;
        public string? UbicacionSeleccionada
        {
            get => _ubicacionSeleccionada;
            set { _ubicacionSeleccionada = value; OnPropertyChanged(); }
        }
        private DateTime _fechaActual = DateTime.Now;
        public DateTime FechaActual
        {
            get => _fechaActual;
            set { _fechaActual = value; OnPropertyChanged(); }
        }
        private string? _descripcionUnidad;
        public string? DescripcionUnidad
        {
            get => _descripcionUnidad;
            set { _descripcionUnidad = value; OnPropertyChanged(); }
        }
        private string? _modeloUnidad;
        public string? ModeloUnidad
        {
            get => _modeloUnidad;
            set { _modeloUnidad = value; OnPropertyChanged(); }
        }
        private string? _motorUnidad;
        public string? MotorUnidad
        {
            get => _motorUnidad;
            set { _motorUnidad = value; OnPropertyChanged(); }
        }

        private string? _motorModelo;
        public string? MotorModelo
        {
            get => _motorModelo;
            set { _motorModelo = value; OnPropertyChanged(); }
        }

        private string? _motorMarca;
        public string? MotorMarca
        {
            get => _motorMarca;
            set { _motorMarca = value; OnPropertyChanged(); }
        }

        private string? _motorSerie;
        public string? MotorSerie
        {
            get => _motorSerie;
            set { _motorSerie = value; OnPropertyChanged(); }
        }

        private string? _marcaUnidad;
        public string? MarcaUnidad
        {
            get => _marcaUnidad;
            set { _marcaUnidad = value; OnPropertyChanged(); }
        }
        public ObservableCollection<CatalogoMarca> ListaMarcas { get; set; } = new();
        public ObservableCollection<CatalogoTiposCombustible> ListaMotores { get; set; } = new();
        private string? _serieUnidad;
        public string? SerieUnidad
        {
            get => _serieUnidad;
            set { _serieUnidad = value; OnPropertyChanged(); }
        }

        // Cambiamos el tipo de dato a CatalogoEconomico para gestionar la selección completa
        private CatalogoEconomico? _economicoSeleccionado;
        public CatalogoEconomico? EconomicoSeleccionado
        {
            get => _economicoSeleccionado;
            set
            {
                _economicoSeleccionado = value;
                OnPropertyChanged();
                ActualizarDatosEconomico();
            }
        }

        private void ActualizarDatosEconomico()
        {
            if (_economicoSeleccionado != null)
            {
                var marcaEncontrada = ListaMarcas?.FirstOrDefault(m => m.IdMarca == _economicoSeleccionado.IdMarca);
                var motorMarcaEncontrado = ListaMarcas?.FirstOrDefault(m => m.IdMarca == _economicoSeleccionado.MarcaMotor);
                var tipoMotor = ListaMotores?.FirstOrDefault(m => m.IdCombustible == _economicoSeleccionado.IdCombustible);
                DescripcionUnidad = _economicoSeleccionado.Descripcion;
                ModeloUnidad = _economicoSeleccionado.Modelo;
                MotorUnidad = tipoMotor?.DescripcionCombustible ?? string.Empty;
                MotorMarca = motorMarcaEncontrado?.NombreMarca ?? string.Empty;
                MarcaUnidad = marcaEncontrada?.NombreMarca ?? string.Empty;
                SerieUnidad = _economicoSeleccionado.Serie;
                MotorModelo = _economicoSeleccionado.ModeloMotor;
                MotorSerie = _economicoSeleccionado.SerieMotor;
            }
            else
            {
                DescripcionUnidad = string.Empty;
                ModeloUnidad = string.Empty;
                MotorUnidad = string.Empty;
                MarcaUnidad = string.Empty;
                SerieUnidad = string.Empty;
                MotorMarca = string.Empty;
                MotorModelo = string.Empty;
                MotorSerie = string.Empty;
            }
        }

        public AgregarRequisicionViewModel(InventarioContext contexto) 
        {
            _contexto = contexto;
            Ubicaciones = new ObservableCollection<CatalogoUbicacionesProyecto>();
            Economicos = new ObservableCollection<CatalogoEconomico>();
            ListaMarcas = new ObservableCollection<CatalogoMarca>();
            ListaMotores = new ObservableCollection<CatalogoTiposCombustible>();

            AñadirConceptoCommand = new RelayCommand(EjecutarAñadirConcepto);

            // (Dentro del constructor, inicializa el comando)
            EliminarConceptoCommand = new RelayCommand(EjecutarEliminarConcepto);

            CargarCatalogos();
        }


        private void EjecutarEliminarConcepto(object? parametro)
        {
            if (parametro is DetalleRequisicion detalle)
            {
                Detalles.Remove(detalle);
                ReordenarPartidas();
            }
        }

        private void ReordenarPartidas()
        {
            int index = 1;
            foreach (var item in Detalles)
            {
                item.Partida = index++;
            }
        }

        private void EjecutarAñadirConcepto(object? obj)
        {
            int siguientePartida = Detalles.Count + 1;
            Detalles.Add(new DetalleRequisicion { Partida = siguientePartida, Cantidad = 1 });
        }

        private void CargarCatalogos()
        {
            var ubicacionesDb = _contexto.CatalogoUbicacionesProyectos
                .Where(e => e.Siglas != "")
                .AsNoTracking()
                .ToList();

            Ubicaciones.Clear();
            foreach (var item in ubicacionesDb) 
            {
                Ubicaciones.Add(item);
            }

            var economicosDb = _contexto.CatalogoEconomicos
                .AsNoTracking()
                .ToList();

            Economicos.Clear();
            foreach (var item in economicosDb)
            {
                Economicos.Add(item);
            }
            var marcasDb = _contexto.Set<CatalogoMarca>().AsNoTracking().ToList();
            ListaMarcas.Clear();
            foreach (var marca in marcasDb)
            {
                ListaMarcas.Add(marca);
            }
            var combustiblesDb = _contexto.Set<CatalogoTiposCombustible>().AsNoTracking().ToList();
            ListaMotores.Clear();
            foreach (var combustible in combustiblesDb)
            {
                ListaMotores.Add(combustible);
            }
        }
    }
    public class RelayCommand : ICommand
    {
        private readonly Action<object?> _ejecutar;
        private readonly Func<object?, bool>? _puedeEjecutar;

        public RelayCommand(Action<object?> ejecutar, Func<object?, bool>? puedeEjecutar = null)
        {
            _ejecutar = ejecutar;
            _puedeEjecutar = puedeEjecutar;
        }

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object? parameter) => _puedeEjecutar?.Invoke(parameter) ?? true;
        public void Execute(object? parameter) => _ejecutar(parameter);
    }
}