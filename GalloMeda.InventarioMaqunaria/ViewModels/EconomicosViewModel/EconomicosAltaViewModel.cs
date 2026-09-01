using GalloMeda.InventarioMaqunaria;
using Inventario.Core.DTOs;
using Inventario.Core.Services.Catalogos;
using Inventario.Core.Services.Economicos;
using Inventario.Core.Services.Logs;
using Inventario.Core.Services.Personal;
using Inventario.Core.Services.UbicacionProyecto;
using Inventario.Data.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace Inventario.Desktop.ViewModels.EconomicosViewModel
{
    // Declaración de la clase ViewModel que implementa la interfaz para notificar cambios a la vista de WPF.
    public class EconomicosAltaViewModel : INotifyPropertyChanged
    {
        // Evento requerido por INotifyPropertyChanged para avisar a la interfaz cuando un dato cambia.
        public event PropertyChangedEventHandler? PropertyChanged;

        // Método auxiliar que dispara el evento PropertyChanged usando el nombre de la propiedad que lo llamó.
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            // Invoca el evento de notificación si tiene suscriptores vinculados.
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Declaración del comando que se enlaza al botón de guardar en la interfaz XAML.
        public ICommand AltaEconomicoCommand { get; }

        // Declaración de variables privadas para los servicios de negocio inyectados.
        private readonly CatalogoMarcasService _marcasService;
        private readonly CatalogoTiposEquipoService _tipoEquipoService;
        private readonly CatalogoGruposService _gruposService;
        private readonly CatalogoCombustiblesService _combustiblesService;
        private readonly ProAdminService _pyaService;
        private readonly UbicacionProyeectoService _ubicacionService;
        private readonly EmpleadoService _empleadosService;
        private readonly CatalogoEconomicosService _economicosService;

        // Propiedades ObservableCollection que alimentan los ComboBoxes en la vista.
        public ObservableCollection<CatalogoMarca> Marcas { get; set; }
        public ObservableCollection<CatalogoTiposEquipo> TipoEquipo { get; set; }
        public ObservableCollection<CatalogoGrupo> Grupos { get; set; }
        public ObservableCollection<CatalogoTiposCombustible> Combustibles { get; set; }
        public ObservableCollection<CatalogoPya> PYA { get; set; }
        public ObservableCollection<CatalogoUbicacionesProyecto> Ubicaciones { get; set; }
        public ObservableCollection<Empleado> Empleados { get; set; }
        public ObservableCollection<Empleado> Responsables { get; set; }
        public ObservableCollection<Empleado> Operadores { get; set; }

        // Campo privado y propiedad pública para almacenar y notificar la selección del Tipo de Equipo.
        private string _idTipoEquipoSeleccionado = "F";
        public string IdTipoEquipoSeleccionado { get => _idTipoEquipoSeleccionado; set { _idTipoEquipoSeleccionado = value; OnPropertyChanged(); } }

        // Campo privado y propiedad pública para almacenar y notificar la selección del Grupo.
        private string _idGrupoSeleccionado = "xxx";
        public string IdGrupoSeleccionado { get => _idGrupoSeleccionado; set { _idGrupoSeleccionado = value; OnPropertyChanged(); } }

        // Campo privado y propiedad pública para almacenar y notificar la selección del Combustible.
        private int _idCombustibleSeleccionado = 7;
        public int IdCombustibleSeleccionado { get => _idCombustibleSeleccionado; set { _idCombustibleSeleccionado = value; OnPropertyChanged(); } }

        // Campo privado y propiedad pública para almacenar y notificar la selección de la Marca del equipo.
        private int _idMarcaSeleccionado = 12;
        public int IdMarcaSeleccionado { get => _idMarcaSeleccionado; set { _idMarcaSeleccionado = value; OnPropertyChanged(); } }

        // Campo privado y propiedad pública para almacenar y notificar la selección de la Marca del Motor.
        private int _marcaMotorSeleccionado = 12;
        public int MarcaMotorSeleccionado { get => _marcaMotorSeleccionado; set { _marcaMotorSeleccionado = value; OnPropertyChanged(); } }

        // Campo privado y propiedad pública para almacenar y notificar la selección del Propietario.
        private int _idPropietarioSeleccionado = 5;
        public int IdPropietarioSeleccionado { get => _idPropietarioSeleccionado; set { _idPropietarioSeleccionado = value; OnPropertyChanged(); } }

        // Campo privado y propiedad pública para almacenar y notificar la selección del Administrador.
        private int _idAdministradorSeleccionado = 5;
        public int IdAdministradorSeleccionado { get => _idAdministradorSeleccionado; set { _idAdministradorSeleccionado = value; OnPropertyChanged(); } }

        // Campo privado y propiedad pública para almacenar y notificar la selección de la Ubicación.
        private int _idUbicacionSeleccionado = 12;
        public int IdUbicacionSeleccionado { get => _idUbicacionSeleccionado; set { _idUbicacionSeleccionado = value; OnPropertyChanged(); } }

        // Campo privado y propiedad pública para almacenar y notificar la selección del Operador.
        private int _idOperadorSeleccionado = 1;
        public int IdOperadorSeleccionado { get => _idOperadorSeleccionado; set { _idOperadorSeleccionado = value; OnPropertyChanged(); } }

        // Campo privado y propiedad pública para almacenar y notificar la selección del Responsable.
        private int _idResponsableSeleccionado = 12;
        public int IdResponsableSeleccionado { get => _idResponsableSeleccionado; set { _idResponsableSeleccionado = value; OnPropertyChanged(); } }

        // Campo privado y propiedad pública para el grado de propiedad del equipo.
        private string _gradoPropiedadSeleccionado = "SIN IDENTIFICAR";
        public string GradoPropiedadSeleccionado { get => _gradoPropiedadSeleccionado; set { _gradoPropiedadSeleccionado = value; OnPropertyChanged(); } }

        // Campo privado y propiedad pública para las observaciones del registro.
        private string _observaciones = "SIN OBSERVACIONES";
        public string Observaciones { get => _observaciones; set { _observaciones = value; OnPropertyChanged(); } }

        // Campo privado y propiedad pública para el modelo del equipo.
        private string _modelo = "MODELO NO IDENTIFICADO";
        public string Modelo { get => _modelo; set { _modelo = value; OnPropertyChanged(); } }

        // Campo privado y propiedad pública para el número de serie.
        private string _serie = "SERIE SIN IDENTIFICAR";
        public string Serie { get => _serie; set { _serie = value; OnPropertyChanged(); } }

        // Campo privado y propiedad pública (nullable) para el año o periodo de fabricación.
        private int? _periodoFab = 0;
        public int? PeriodoFab { get => _periodoFab; set { _periodoFab = value; OnPropertyChanged(); } }

        // Campo privado y propiedad pública (nullable) para el horómetro del equipo.
        private int? _horometro = 0;
        public int? Horometro { get => _horometro; set { _horometro = value; OnPropertyChanged(); } }

        // Campo privado y propiedad pública para el modelo del motor.
        private string _modeloMotor = "MODELO DE MOTOR SIN IDENTIFICAR O NO TIENE MOTOR";
        public string ModeloMotor { get => _modeloMotor; set { _modeloMotor = value; OnPropertyChanged(); } }

        // Campo privado y propiedad pública para la serie del motor.
        private string _serieMotor = "SERIE DE MOTOR SIN IDENTIFICAR O NO TIENE MOTOR";
        public string SerieMotor { get => _serieMotor; set { _serieMotor = value; OnPropertyChanged(); } }

        // Campo privado y propiedad pública para la familia del motor.
        private string _familiaMotor = "FAMILIA DE MOTOR SIN IDENTIFICAR O NO TIENE MOTOR";
        public string FamiliaMotor { get => _familiaMotor; set { _familiaMotor = value; OnPropertyChanged(); } }

        // Campo privado y propiedad pública para las placas del equipo.
        private string _placas = "PLACAS SIN IDENTIFICAR O NO TIENE PLACAS";
        public string Placas { get => _placas; set { _placas = value; OnPropertyChanged(); } }

        // Campo privado y propiedad pública para el número de póliza adjunta.
        private string _polizaAdj = "SIN POLIZA VER EN DOCUMENTOS ADJUNTOS";
        public string PolizaAdj { get => _polizaAdj; set { _polizaAdj = value; OnPropertyChanged(); } }

        // Campo privado y propiedad pública para las dimensiones del equipo.
        private string _dimensiones = "DIMENSIONES AUN SIN MEDIR";
        public string Dimensiones { get => _dimensiones; set { _dimensiones = value; OnPropertyChanged(); } }

        // Campo privado y propiedad pública para el indicador THK (Tiempo/Horas/Kilometraje).
        private string _thk = "SIN IDENTIFICAR HOROMETRO O KILOMETRAJE";
        public string THK { get => _thk; set { _thk = value; OnPropertyChanged(); } }

        // Campo privado y propiedad pública para el tipo de seguro.
        private string _tipoSeguro = "SIN SEGURO";
        public string TipoSeguro { get => _tipoSeguro; set { _tipoSeguro = value; OnPropertyChanged(); } }

        // Campo privado y propiedad pública para el estado booleano del seguro.
        private bool _estatusSeguro = false;
        public bool EstatusSeguro { get => _estatusSeguro; set { _estatusSeguro = value; OnPropertyChanged(); } }

        // Constructor del ViewModel que recibe los servicios mediante inyección de dependencias.
        public EconomicosAltaViewModel(CatalogoMarcasService marcasService,
                                       CatalogoTiposEquipoService tipoEquipoService,
                                       CatalogoGruposService grupoService,
                                       CatalogoCombustiblesService combustibleService,
                                       ProAdminService pyaService,
                                       UbicacionProyeectoService ubicacionService,
                                       EmpleadoService empleadoService,
                                       CatalogoEconomicosService economicosService)
        {
            // Asigna el servicio de marcas recibido por parámetro al campo local.
            _marcasService = marcasService;
            // Inicializa la colección observable para contener la lista de marcas.
            Marcas = new ObservableCollection<CatalogoMarca>();

            // Asigna el servicio de tipos de equipo recibido por parámetro al campo local.
            _tipoEquipoService = tipoEquipoService;
            // Inicializa la colección observable para contener la lista de tipos de equipo.
            TipoEquipo = new ObservableCollection<CatalogoTiposEquipo>();

            // Asigna el servicio de grupos recibido por parámetro al campo local.
            _gruposService = grupoService;
            // Inicializa la colección observable para contener la lista de grupos.
            Grupos = new ObservableCollection<CatalogoGrupo>();

            // Asigna el servicio de combustibles recibido por parámetro al campo local.
            _combustiblesService = combustibleService;
            // Inicializa la colección observable para contener la lista de tipos de combustible.
            Combustibles = new ObservableCollection<CatalogoTiposCombustible>();

            // Asigna el servicio de PYA recibido por parámetro al campo local.
            _pyaService = pyaService;
            // Inicializa la colección observable para contener la lista de PYA.
            PYA = new ObservableCollection<CatalogoPya>();

            // Asigna el servicio de ubicaciones recibido por parámetro al campo local.
            _ubicacionService = ubicacionService;
            // Inicializa la colección observable para contener la lista de ubicaciones de proyecto.
            Ubicaciones = new ObservableCollection<CatalogoUbicacionesProyecto>();

            // Asigna el servicio de empleados recibido por parámetro al campo local.
            _empleadosService = empleadoService;
            // Inicializa la colección observable genérica de empleados.
            Empleados = new ObservableCollection<Empleado>();

            // Inicializa la colección observable para los empleados responsables.
            Responsables = new ObservableCollection<Empleado>();
            // Inicializa la colección observable para los empleados operadores.
            Operadores = new ObservableCollection<Empleado>();

            // Instancia el comando vinculando la acción de guardado con el método EjecutarAltaEconomico.
            AltaEconomicoCommand = new RelayCommand(EjecutarAltaEconomico);

            // Asigna el servicio de económicos recibido desde la inyección de dependencias.
            _economicosService = economicosService;

            // Llama al método que consulta las bases de datos y puebla las ObservableCollections.
            CargarTipos();
        }

        // Método que se encarga de consultar los catálogos e insertar los datos en las colecciones de la UI.
        private void CargarTipos()
        {
            // Obtiene la lista de marcas desde el servicio.
            var datosmarcas = _marcasService.ObtenerMarcas();
            // Itera cada marca obtenida y la agrega a la lista vinculada a la interfaz.
            foreach (var marca in datosmarcas) { Marcas.Add(marca); }

            // Obtiene la lista de tipos de equipo desde el servicio.
            var datos = _tipoEquipoService.ObtenerTiposEq();
            // Itera cada tipo de equipo obtenido y lo agrega a la colección.
            foreach (var item in datos) { TipoEquipo.Add(item); }

            // Obtiene la lista de grupos desde el servicio.
            var datoGrupo = _gruposService.ObtenerGrupos();
            // Itera cada grupo obtenido y lo agrega a la colección.
            foreach (var grupo in datoGrupo) { Grupos.Add(grupo); }

            // Obtiene la lista de combustibles desde el servicio.
            var datoCombus = _combustiblesService.ObtenerCombustibles();
            // Itera cada combustible obtenido y lo agrega a la colección.
            foreach (var combus in datoCombus) { Combustibles.Add(combus); }

            // Obtiene la lista de datos PYA desde el servicio.
            var datospya = _pyaService.ObtenerPYA();
            // Itera cada registro PYA obtenido y lo agrega a la colección.
            foreach (var datopya in datospya) { PYA.Add(datopya); }

            // Obtiene la lista de ubicaciones de proyectos desde el servicio.
            var datosUbi = _ubicacionService.ObtenerUbicaciones();
            // Itera cada ubicación obtenida y la agrega a la colección.
            foreach (var datoU in datosUbi) { Ubicaciones.Add(datoU); }

            // Define los IDs de roles que corresponden a empleados autorizados para asumir responsabilidades.
            var rolesPermitidos = new List<int> { 2, 3, 4, 5, 6, 7 };
            // Consulta los empleados que poseen los roles especificados.
            var datosRes = _empleadosService.ObtenerResponsables(rolesPermitidos);
            // Itera la lista de responsables y los inserta en la colección de la interfaz.
            foreach (var responsable in datosRes) { Responsables.Add(responsable); }

            // Consulta la lista de operadores usando los mismos roles desde el servicio de empleados.
            var datosOpera = _empleadosService.ObtenerResponsables(rolesPermitidos);
            // Itera la lista de operadores y los agrega a la colección de la interfaz.
            foreach (var operador in datosOpera) { Operadores.Add(operador); }
        }

        // Método ejecutado al presionar el botón de dar de alta en la interfaz.
        private void EjecutarAltaEconomico()
        {
            // Bloque de control de excepciones para capturar fallos durante el guardado.
            try
            {
                // Instancia el objeto de transferencia de datos (DTO) recopilando la información de las propiedades del ViewModel.
                var dto = new EconomicoAltaDto
                {
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
                    Modelo = this.Modelo,
                    Serie = this.Serie,
                    PeriodoFab = this.PeriodoFab ?? 0,
                    Placas = this.Placas,
                    MarcaMotor = this.MarcaMotorSeleccionado,
                    ModeloMotor = this.ModeloMotor,
                    SerieMotor = this.SerieMotor,
                    FamiliaMotor = this.FamiliaMotor,
                    Observaciones = this.Observaciones,
                    PolizaAdj = this.PolizaAdj,
                    EstatusSeguro = this.EstatusSeguro,
                    Horometro = this.Horometro ?? 0,
                    Dimensiones = this.Dimensiones,
                    THK = this.THK,
                    TipoSeguro = this.TipoSeguro,
                };

                // Llama al servicio de negocio para insertar la entidad enviando el ID del usuario y el DTO cargado.
                _economicosService.RegistrarEconomico(App.Session.IdUsuario, dto);

                // Muestra un diálogo al usuario informando que la transacción fue exitosa.
                MessageBox.Show("¡Guardado exitosamente en PostgreSQL!", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                // Llama al método de limpieza para resetear la interfaz a sus valores por defecto.
                LimpiarFormulario();
            }
            // Captura los errores lanzados durante la validación o persistencia.
            catch (Exception ex)
            {
                // Muestra una alerta con el detalle de la excepción capturada.
                MessageBox.Show(ex.Message, "Error de Validación", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Método encargado de reiniciar los campos de la pantalla a sus valores originales tras una inserción.
        private void LimpiarFormulario()
        {
            // Restablece la serie a su valor string por defecto usando la propiedad pública.
            Serie = "SERIE SIN IDENTIFICAR";
            // Restablece las observaciones a su valor string por defecto usando la propiedad pública.
            Observaciones = "SIN OBSERVACIONES";
            // Restablece el modelo a su valor string por defecto usando la propiedad pública.
            Modelo = "MODELO NO IDENTIFICADO";
            // Restablece el año de fabricación a 0 usando la propiedad pública.
            PeriodoFab = 0;
            // Restablece el modelo del motor a su valor string por defecto.
            ModeloMotor = "MODELO DE MOTOR SIN IDENTIFICAR O NO TIENE MOTOR";
            // Restablece la serie del motor a su valor string por defecto.
            SerieMotor = "SERIE DE MOTOR SIN IDENTIFICAR O NO TIENE MOTOR";
            // Restablece la familia del motor a su valor string por defecto.
            FamiliaMotor = "FAMILIA DE MOTOR SIN IDENTIFICAR O NO TIENE MOTOR";
            // Restablece las placas a su valor string por defecto.
            Placas = "PLACAS SIN IDENTIFICAR O NO TIENE PLACAS";
            // Restablece el campo póliza a su valor string por defecto.
            PolizaAdj = "SIN POLIZA VER EN DOCUMENTOS ADJUNTOS";
            // Restablece las dimensiones a su valor string por defecto.
            Dimensiones = "DIMENSIONES AUN SIN MEDIR";
            // Restablece el indicador THK a su valor string por defecto.
            THK = "SIN IDENTIFICAR HOROMETRO O KILOMETRAJE";
            // Restablece el tipo de seguro a su valor string por defecto.
            TipoSeguro = "SIN SEGURO";
            // Restablece el valor de la checkbox o switch de seguro a falso.
            EstatusSeguro = false;
            // Restablece el horómetro a 0 usando la propiedad pública.
            Horometro = 0;

            // Restablece el ComboBox de marcas asignando un ID entero válido existente en la lista.
            IdMarcaSeleccionado = 12;
            // Restablece el ComboBox de marcas de motor asignando un ID entero válido.
            MarcaMotorSeleccionado = 12;
            // Restablece la selección del tipo de equipo asignando una clave existente.
            IdTipoEquipoSeleccionado = "F";
            // Restablece la selección del grupo asignando una clave existente.
            IdGrupoSeleccionado = "xxx";
            // Restablece la selección de combustible al ID por defecto.
            IdCombustibleSeleccionado = 7;
            // Restablece la selección de propietario al ID por defecto.
            IdPropietarioSeleccionado = 5;
            // Restablece la selección de administrador al ID por defecto.
            IdAdministradorSeleccionado = 5;
            // Restablece la selección de ubicación al ID por defecto.
            IdUbicacionSeleccionado = 12;
            // Restablece la selección de operador al ID por defecto.
            IdOperadorSeleccionado = 1;
            // Restablece la selección de responsable al ID por defecto.
            IdResponsableSeleccionado = 12;
            // Restablece el grado de propiedad al valor string por defecto.
            GradoPropiedadSeleccionado = "SIN IDENTIFICAR";
        }
    }
}