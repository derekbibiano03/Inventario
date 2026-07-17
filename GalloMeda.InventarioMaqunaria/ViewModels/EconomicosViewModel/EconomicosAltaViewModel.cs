using GalloMeda.InventarioMaqunaria;
using Inventario.Core.DTOs;
using Inventario.Core.Services.Catalogos;
using Inventario.Core.Services.Economicos;
using Inventario.Core.Services.Logs;
using Inventario.Core.Services.Personal;
using Inventario.Core.Services.UbicacionProyecto;
using Inventario.Data.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace Inventario.Desktop.ViewModels.EconomicosViewModel
{
    public class EconomicosAltaViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // COMANDO PARA EL BOTÓN
        public ICommand AltaEconomicoCommand { get; }

        // SERVICIOS INYECTADOS
        private readonly CatalogoMarcasService _marcasService;
        private readonly CatalogoTiposEquipoService _tipoEquipoService;
        private readonly CatalogoGruposService _gruposService;
        private readonly CatalogoCombustiblesService _combustiblesService;
        private readonly ProAdminService _pyaService;
        private readonly UbicacionProyeectoService _ubicacionService;
        private readonly CatalogoOperadoresService _operadorService;
        private readonly CatalogoEncargadoMaquinariaService _responsableService;
        private readonly CatalogoEconomicosService _economicosService;

        // COLECCIONES PARA COMBOBOX
        public ObservableCollection<CatalogoMarca> Marcas { get; set; }
        public ObservableCollection<CatalogoTiposEquipo> TipoEquipo { get; set; }
        public ObservableCollection<CatalogoGrupo> Grupos { get; set; }
        public ObservableCollection<CatalogoTiposCombustible> Combustibles { get; set; }
        public ObservableCollection<CatalogoPya> PYA { get; set; }
        public ObservableCollection<CatalogoUbicacionesProyecto> Ubicaciones { get; set; }
        public ObservableCollection<CatalogoOperadore> Operadores { get; set; }
        public ObservableCollection<CatalogoResponsableMaquinarium> Responsables { get; set; }

        // VARIABLES DE SELECCIÓN DE COMBOBOX
        private string _idTipoEquipoSeleccionado;
        public string IdTipoEquipoSeleccionado { get => _idTipoEquipoSeleccionado; set { _idTipoEquipoSeleccionado = value; OnPropertyChanged(); } }

        private string _idGrupoSeleccionado;
        public string IdGrupoSeleccionado { get => _idGrupoSeleccionado; set { _idGrupoSeleccionado = value; OnPropertyChanged(); } }

        private int? _idCombustibleSeleccionado;
        public int? IdCombustibleSeleccionado { get => _idCombustibleSeleccionado; set { _idCombustibleSeleccionado = value; OnPropertyChanged(); } }
        
        private int? _idMarcaSeleccionado;
        public int? IdMarcaSeleccionado { get => _idMarcaSeleccionado; set { _idMarcaSeleccionado = value; OnPropertyChanged(); } }

        private int? _marcaMotorSeleccionado;
        public int? MarcaMotorSeleccionado { get => _marcaMotorSeleccionado; set { _marcaMotorSeleccionado = value; OnPropertyChanged(); } }

        private int? _idPropietarioSeleccionado;
        public int? IdPropietarioSeleccionado { get => _idPropietarioSeleccionado; set { _idPropietarioSeleccionado = value; OnPropertyChanged(); } }

        private int? _idAdministradorSeleccionado;
        public int? IdAdministradorSeleccionado { get => _idAdministradorSeleccionado; set { _idAdministradorSeleccionado = value; OnPropertyChanged(); } }

        private int? _idUbicacionSeleccionado;
        public int? IdUbicacionSeleccionado { get => _idUbicacionSeleccionado; set { _idUbicacionSeleccionado = value; OnPropertyChanged(); } }

        private int? _idOperadorSeleccionado;
        public int? IdOperadorSeleccionado { get => _idOperadorSeleccionado; set { _idOperadorSeleccionado = value; OnPropertyChanged(); } }

        private int? _idResponsableSeleccionado;
        public int? IdResponsableSeleccionado { get => _idResponsableSeleccionado; set { _idResponsableSeleccionado = value; OnPropertyChanged(); } }

        private string _gradoPropiedadSeleccionado;
        public string GradoPropiedadSeleccionado { get => _gradoPropiedadSeleccionado; set { _gradoPropiedadSeleccionado = value; OnPropertyChanged(); } }

        private string _observaciones;
        public string Observaciones { get => _observaciones; set { _observaciones = value; OnPropertyChanged(); } }

        private string _modelo;
        public string Modelo { get => _modelo; set { _modelo = value; OnPropertyChanged(); } }

        private string _serie;
        public string Serie { get => _serie; set { _serie = value; OnPropertyChanged(); } }

        private int? _periodoFab;
        public int? PeriodoFab { get => _periodoFab; set { _periodoFab = value; OnPropertyChanged(); } }

        private int _horometro;
        public int Horometro { get => _horometro; set { _horometro = value; OnPropertyChanged(); } }

        private int _marcaMotor;
        public int MarcaMotor { get => _marcaMotor; set { _marcaMotor = value; OnPropertyChanged(); } }

        private string _modeloMotor;
        public string ModeloMotor { get => _modeloMotor; set { _modeloMotor = value; OnPropertyChanged(); } }

        private string _serieMotor;
        public string SerieMotor { get => _serieMotor; set { _serieMotor = value; OnPropertyChanged(); } }

        private string _familiaMotor;
        public string FamiliaMotor { get => _familiaMotor; set { _familiaMotor = value; OnPropertyChanged(); } }

        private string _placas;
        public string Placas { get => _placas; set { _placas = value; OnPropertyChanged(); } }

        private string _polizaAdj;
        public string PolizaAdj { get => _polizaAdj; set { _polizaAdj = value; OnPropertyChanged(); } }

        private string _dimensiones;
        public string Dimensiones { get => _dimensiones; set { _dimensiones = value; OnPropertyChanged(); } }

        private string _thk;
        public string THK { get => _thk; set { _thk = value; OnPropertyChanged(); } }

        private bool _estatusSeguro;
        public bool EstatusSeguro { get => _estatusSeguro; set { _estatusSeguro = value; OnPropertyChanged(); } }



        public EconomicosAltaViewModel(CatalogoMarcasService marcasService,
                                       CatalogoTiposEquipoService tipoEquipoService,
                                       CatalogoGruposService grupoService,
                                       CatalogoCombustiblesService combustibleService,
                                       ProAdminService pyaService,
                                       UbicacionProyeectoService ubicacionService,
                                       CatalogoOperadoresService operadorService,
                                       CatalogoEncargadoMaquinariaService responsableService,
                                       CatalogoEconomicosService economicosService)
        {
            // Asigna un valor por defecto predeterminado al identificador de la marca del motor
            MarcaMotorSeleccionado = 60;

            // Inicializa el servicio de marcas y prepara su colección enlazada a la UI
            _marcasService = marcasService;
            Marcas = new ObservableCollection<CatalogoMarca>();

            // Inicializa el servicio de categorías de equipo y prepara su colección enlazada a la UI
            _tipoEquipoService = tipoEquipoService;
            TipoEquipo = new ObservableCollection<CatalogoTiposEquipo>();

            // Inicializa el servicio de grupos de activos y prepara su colección enlazada a la UI
            _gruposService = grupoService;
            Grupos = new ObservableCollection<CatalogoGrupo>();

            // Inicializa el servicio de combustibles y prepara su colección enlazada a la UI
            _combustiblesService = combustibleService;
            Combustibles = new ObservableCollection<CatalogoTiposCombustible>();

            // Inicializa el servicio de propietarios y administradores, preparando su colección para la UI
            _pyaService = pyaService;
            PYA = new ObservableCollection<CatalogoPya>();

            // Inicializa el servicio de frentes de obra/proyectos junto a su colección para la UI
            _ubicacionService = ubicacionService;
            Ubicaciones = new ObservableCollection<CatalogoUbicacionesProyecto>();

            // Inicializa el servicio de operadores asignados y prepara su colección para la UI
            _operadorService = operadorService;
            Operadores = new ObservableCollection<CatalogoOperadore>();

            // Inicializa el servicio de encargados de maquinaria junto a su colección para la UI
            _responsableService = responsableService;
            Responsables = new ObservableCollection<CatalogoResponsableMaquinarium>();

            // CORRECCIÓN: Asigna el comando apuntando al método real de tu clase: 'EjecutarAltaEconomico'
            AltaEconomicoCommand = new RelayCommand(EjecutarAltaEconomico);

            // Abre una conexión aislada a PostgreSQL exclusivamente para operaciones de auditoría interna
            var contexto = new InventarioContext();

            // Instancia el gestor de bitácoras asociándole el contexto de datos recién abierto
            var logsService = new LogsService(contexto);

            // Sobreescribe la variable del servicio asignándole la instancia parametrizada de manera correcta
            _economicosService = new CatalogoEconomicosService(contexto, logsService);

            // Llama a la función interna encargada de descargar y rellenar todos los listados de los ComboBoxes
            CargarTipos();
        }

        private void CargarTipos()
        {
            var datosmarcas = _marcasService.ObtenerMarcas();
            foreach (var marca in datosmarcas) { Marcas.Add(marca); }

            var datos = _tipoEquipoService.ObtenerTiposEq();
            foreach (var item in datos) { TipoEquipo.Add(item); }

            var datoGrupo = _gruposService.ObtenerGrupos();
            foreach (var grupo in datoGrupo) { Grupos.Add(grupo); }

            var datoCombus = _combustiblesService.ObtenerCombustibles();
            foreach (var combus in datoCombus) { Combustibles.Add(combus); }

            var datospya = _pyaService.ObtenerPYA();
            foreach (var datopya in datospya) { PYA.Add(datopya); }

            var datosUbi = _ubicacionService.ObtenerUbicaciones();
            foreach (var datoU in datosUbi) { Ubicaciones.Add(datoU); }

            var datosOper = _operadorService.ObtenerOperadores();
            foreach (var datoOp in datosOper) { Operadores.Add(datoOp); }

            var datosRes = _responsableService.ObtenerResponsables();
            foreach (var dator in datosRes) { Responsables.Add(dator); }
        }

        private void EjecutarAltaEconomico()
        {
            try
            {
                // GENERADO: Mapeo completo de absolutamente todos los campos del formulario al DTO
                var dto = new EconomicoAltaDto
                {
                    // Identificadores y selecciones de ComboBoxes
                    IdTipoEquipo = this.IdTipoEquipoSeleccionado,
                    IdGrupo = this.IdGrupoSeleccionado,
                    IdCombustible = this.IdCombustibleSeleccionado,
                    IdPropietario = this.IdPropietarioSeleccionado,
                    IdAdministrador = this.IdAdministradorSeleccionado,
                    IdUbicacion = this.IdUbicacionSeleccionado,
                    IdOperador = this.IdOperadorSeleccionado,
                    IdResponsable = this.IdResponsableSeleccionado,
                    GradoPropiedad = this.GradoPropiedadSeleccionado,
                    IdMarca = this.IdMarcaSeleccionado,
                    // Campos de texto generales del equipo

                    Modelo = this.Modelo,
                    Serie = this.Serie,
                    PeriodoFab = this.PeriodoFab,
                    Placas = this.Placas,

                    // Especificaciones del motor
                    MarcaMotor = this.MarcaMotorSeleccionado ?? 60,
                    ModeloMotor = this.ModeloMotor,
                    SerieMotor = this.SerieMotor,
                    FamiliaMotor = this.FamiliaMotor,

                    // Documentos, observaciones y estados lógicos
                    Observaciones = this.Observaciones,
                    PolizaAdj = this.PolizaAdj,
                    EstatusSeguro = this.EstatusSeguro,
                    Horometro = this.Horometro,
                    Dimensiones = this.Dimensiones,
                    THK = this.THK,
                };

                // Llamada directa al método de la clase en el .Core
                _economicosService.RegistrarEconomico(App.Session.IdUsuario, dto);

                MessageBox.Show("¡Guardado exitosamente en PostgreSQL!", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                LimpiarFormulario();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error de Validación", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LimpiarFormulario()
        {
            // Restablecimiento de campos de texto e información general
            Serie = string.Empty;
            Observaciones = string.Empty;
            Modelo = string.Empty;
            PeriodoFab = null;
            MarcaMotor = 0;
            ModeloMotor = string.Empty;
            SerieMotor = string.Empty;
            FamiliaMotor = string.Empty;
            Placas = string.Empty;
            PolizaAdj = string.Empty;
            EstatusSeguro = false;
            Horometro = 0;
            Dimensiones = string.Empty;

            // GENERADO: Limpieza y reinicio explícito de los ComboBoxes para dejar el formulario completamente vacío
            IdMarcaSeleccionado = null;
            MarcaMotor = 60;
            IdTipoEquipoSeleccionado = null;
            IdGrupoSeleccionado = null;
            IdCombustibleSeleccionado = null;
            IdPropietarioSeleccionado = null;
            IdAdministradorSeleccionado = null;
            IdUbicacionSeleccionado = null;
            IdOperadorSeleccionado = null;
            IdResponsableSeleccionado = null;
            GradoPropiedadSeleccionado = null;
        }
    }
}