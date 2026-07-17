using Inventario.Core;
using Inventario.Core.DTOs;
using Inventario.Core.Services.Economicos;
using Inventario.Core.Services.Logs;
using Inventario.Data;
using Inventario.Data.Models;
using Inventario.Desktop.Views;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using System.Windows.Input;

namespace Inventario.Desktop.ViewModels.EconomicosViewModel.EconomicosViewModel
{
    public class OpcionFiltroCheckbox : INotifyPropertyChanged
    {
        private bool _isChecked;
        public string Nombre { get; set; }
        public int Id { get; set; }
        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                _isChecked = value;
                OnPropertyChanged();
                AlCambiarSeleccion?.Invoke();
            }
        }
        public required Action AlCambiarSeleccion { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    public class EconomicosViewModel : INotifyPropertyChanged
    {
        private bool _isResetting = false;
        private readonly CatalogoEconomicosService _economicosService;
        private readonly ExcelExportService _excelService = new ExcelExportService();
        private readonly InventarioContext _contextoCompartido;
        private string _busquedaId;
        private string _busquedaDescripcion;
        private string _busquedaMarca;
        private string _busquedaSerie;
        private string _busquedaTipoEquipo;
        private string _busquedaUbicacion;
        public ICommand ExportarExcelCommand { get; set; }
        public ObservableCollection<EconomicoMinimoDto> Economicos { get; set; }
        public ICollectionView VistaEconomicos { get; set; }
        public ICommand EditarCommand { get; }
        private Dictionary<string, bool> _estadosId = new Dictionary<string, bool>();
        private Dictionary<string, bool> _estadosDescripcion = new Dictionary<string, bool>();
        private Dictionary<int, bool> _estadosMarca = new Dictionary<int, bool>();
        private Dictionary<string, bool> _estadosSerie = new Dictionary<string, bool>();
        private Dictionary<string, bool> _estadosTipoEquipo = new Dictionary<string, bool>();
        private Dictionary<int, bool> _estadosUbicacion = new Dictionary<int, bool>();
        public ObservableCollection<OpcionFiltroCheckbox> FiltroIdOpciones { get; set; } = new ObservableCollection<OpcionFiltroCheckbox>();
        public ObservableCollection<OpcionFiltroCheckbox> FiltroDescripcionesOpciones { get; set; } = new ObservableCollection<OpcionFiltroCheckbox>();
        public ObservableCollection<OpcionFiltroCheckbox> FiltroMarcasOpciones { get; set; } = new ObservableCollection<OpcionFiltroCheckbox>();
        public ObservableCollection<OpcionFiltroCheckbox> FiltroSeriesOpciones { get; set; } = new ObservableCollection<OpcionFiltroCheckbox>();
        public ObservableCollection<OpcionFiltroCheckbox> FiltroTipoEquipoOpciones { get; set; } = new ObservableCollection<OpcionFiltroCheckbox>();
        public ObservableCollection<OpcionFiltroCheckbox> FiltroUbicacionesOpciones { get; set; } = new ObservableCollection<OpcionFiltroCheckbox>();
        public ObservableCollection<CatalogoUbicacionesProyecto> ListaUbicaciones { get; set; }
        public ICommand VerDetalleCommand { get; }
        public ICommand LimpiarFiltrosCommand { get; }
        public string BusquedaId
        {
            get => _busquedaId;
            set { _busquedaId = value; OnPropertyChanged(); RecalcularOpcionesFiltros(); }
        }
        public string BusquedaDescripcion
        {
            get => _busquedaDescripcion;
            set { _busquedaDescripcion = value; OnPropertyChanged(); RecalcularOpcionesFiltros(); }
        }
        public string BusquedaMarca
        {
            get => _busquedaMarca;
            set { _busquedaMarca = value; OnPropertyChanged(); RecalcularOpcionesFiltros(); }
        }
        public string BusquedaSerie
        {
            get => _busquedaSerie;
            set { _busquedaSerie = value; OnPropertyChanged(); RecalcularOpcionesFiltros(); }
        }
        public string BusquedaTipoEquipo
        {
            get => _busquedaTipoEquipo;
            set { _busquedaTipoEquipo = value; OnPropertyChanged(); RecalcularOpcionesFiltros(); }
        }
        public string BusquedaUbicacion
        {
            get => _busquedaUbicacion;
            set { _busquedaUbicacion = value; OnPropertyChanged(); RecalcularOpcionesFiltros(); }
        }
        public EconomicosViewModel()
        {
            VerDetalleCommand = new RelayCommand<string>(AbrirVentanaDetalle);
            LimpiarFiltrosCommand = new RelayCommand<object>(x => LimpiarFiltros());
            EditarCommand = new RelayCommand<string>(AbrirVentanaEditar);

            // Crea la conexión única con la base de datos PostgreSQL para este módulo
            _contextoCompartido = new InventarioContext();

            // CORRECCIÓN: Instancia primero el servicio de logs con el contexto compartido
            var logsService = new LogsService(_contextoCompartido);

            // CORRECCIÓN: Pasa ambos objetos requeridos al constructor del servicio para evitar el NullReferenceException
            _economicosService = new CatalogoEconomicosService(_contextoCompartido, logsService);

            Economicos = new ObservableCollection<EconomicoMinimoDto>();

            VistaEconomicos = CollectionViewSource.GetDefaultView(Economicos);
            VistaEconomicos.Filter = FiltroEjecucion;

            ExportarExcelCommand = new RelayCommand(EjecutarExportacion);

            CargarEconomicos();
        }

        private void AbrirVentanaEditar(string id)
        {
            if (string.IsNullOrEmpty(id)) return;
            EditarEconomicoWindow ventanaEditar = new EditarEconomicoWindow(id);
            bool? resultado = ventanaEditar.ShowDialog();
            if (resultado == true)
            {
                CargarEconomicos();
            }
        }

        private void EjecutarExportacion()
        {
            List<EconomicoMinimoDto> equiposVisiblesEnTabla = VistaEconomicos.Cast<EconomicoMinimoDto>().ToList();
            if (!equiposVisiblesEnTabla.Any())
            {
                System.Windows.MessageBox.Show("No hay registros seleccionados por los filtros actuales para exportar.", "Atención", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                return;
            }

            string[] idsDeEquiposFiltrados = equiposVisiblesEnTabla.Select(x => x.IdEconomico).ToArray();
            List<CatalogoEconomico> datosCompletos = _economicosService.ObtenerEconomicosPorListaDeIds(idsDeEquiposFiltrados);
            byte[] archivoExcelBytes = _excelService.GenerarExcelEconomicos(datosCompletos);

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                FileName = $"Reporte_Inventario_Completo_{DateTime.Now:yyyyMMdd}",
                DefaultExt = ".xlsx",
                Filter = "Archivos de Excel (*.xlsx)|*.xlsx"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllBytes(saveFileDialog.FileName, archivoExcelBytes);
            }
        }
        private bool FiltroEjecucion(object item)
        {
            var economico = item as EconomicoMinimoDto;
            if (economico == null) return false;

            bool resultado = true;

            // Filtro ID
            var idsSeleccionados = _estadosId.Where(x => x.Value).Select(x => x.Key).ToList();
            if (idsSeleccionados.Any())
            {
                resultado = resultado && economico.IdEconomico != null && idsSeleccionados.Contains(economico.IdEconomico);
            }

            // Filtro Descripción
            var descripcionesSeleccionadas = _estadosDescripcion.Where(x => x.Value).Select(x => x.Key).ToList();
            if (descripcionesSeleccionadas.Any())
            {
                resultado = resultado && economico.Descripcion != null && descripcionesSeleccionadas.Contains(economico.Descripcion);
            }

            // Filtro Marca
            var marcasSeleccionadas = _estadosMarca.Where(x => x.Value).Select(x => x.Key).ToList();
            if (marcasSeleccionadas.Any())
            {
                resultado = resultado && economico.IdMarca.HasValue && marcasSeleccionadas.Contains(economico.IdMarca.Value);
            }

            // Filtro Número de Serie
            var seriesSeleccionadas = _estadosSerie.Where(x => x.Value).Select(x => x.Key).ToList();
            if (seriesSeleccionadas.Any())
            {
                resultado = resultado && economico.Serie != null && seriesSeleccionadas.Contains(economico.Serie);
            }

            // Filtro Tipo de Equipo
            var tipoequiposSeleccionadas = _estadosTipoEquipo.Where(x => x.Value).Select(x => x.Key).ToList();
            if (tipoequiposSeleccionadas.Any())
            {
                resultado = resultado && economico.IdTipoEquipo != null && tipoequiposSeleccionadas.Contains(economico.IdTipoEquipo);
            }

            // Filtro Ubicación
            var ubicacionesSeleccionadas = _estadosUbicacion.Where(x => x.Value).Select(x => x.Key).ToList();
            if (ubicacionesSeleccionadas.Any())
            {
                resultado = resultado && economico.IdUbicacion.HasValue && ubicacionesSeleccionadas.Contains(economico.IdUbicacion.Value);
            }

            return resultado;
        }
        private void RecalcularOpcionesFiltros(string columnaExcluida = null)
        {
            if (_isResetting) return;
            _isResetting = true;
            var itemsVisibles = Economicos.Where(FiltroEjecucion).ToList();

            // ==========================================
            // 1. RE-POBLAR ID ECONÓMICO
            // ==========================================
            // Solo regenera la lista de IDs si esta columna no fue la que originó el cambio de selección del usuario
            if (columnaExcluida != "ID")
            {
                // Extrae el universo de cadenas de texto de IDs económicos que siguen siendo válidos en el DataGrid filtrado
                var idsVisibles = itemsVisibles.Select(e => e.IdEconomico).Where(id => !string.IsNullOrEmpty(id)).Distinct().ToList();
                // Limpia por completo la colección observable enlazada directamente al ComboBox del ID en el XAML
                FiltroIdOpciones.Clear();
                // Consolida en un conjunto único los IDs que el usuario ya marcó junto con los que siguen resultando visibles en la grilla
                var idsAMostrar = _estadosId.Where(x => x.Value).Select(x => x.Key).Union(idsVisibles).OrderBy(x => x);
                foreach (var id in idsAMostrar)
                {
                    // Valida si el ítem cumple con la cadena de caracteres ingresada en el buscador de la cabecera de ID
                    if (!string.IsNullOrWhiteSpace(BusquedaId) && !id.Contains(BusquedaId, StringComparison.OrdinalIgnoreCase)) continue;
                    // Registra preventivamente la existencia de la clave de texto en el diccionario de control de estados
                    _estadosId.TryAdd(id, false);
                    // Inserta el elemento reactivo acoplado a la acción delegada para mantener la sincronización de la base de datos
                    FiltroIdOpciones.Add(new OpcionFiltroCheckbox { Nombre = id, IsChecked = _estadosId[id], AlCambiarSeleccion = () => NotificarCheckboxCambiado("ID", id, null) });
                }
            }

            // ==========================================
            // 2. RE-POBLAR DESCRIPCIONES
            // ==========================================
            // Evalúa si la columna de descripción debe quedar exenta de la limpieza destructiva para no perder el foco del puntero
            if (columnaExcluida != "DESCRIPCION")
            {
                // Obtiene las descripciones distintas de los equipos que lograron pasar con éxito el predicado lógico del filtro
                var descVisibles = itemsVisibles.Select(e => e.Descripcion).Where(d => !string.IsNullOrEmpty(d)).Distinct().ToList();
                // Remueve la totalidad de los objetos Checkbox expuestos previamente en el desplegable de descripciones
                FiltroDescripcionesOpciones.Clear();
                // Une mediante operaciones algebraicas de conjuntos LINQ los elementos marcados con las cadenas vigentes en pantalla
                var descAMostrar = _estadosDescripcion.Where(x => x.Value).Select(x => x.Key).Union(descVisibles).OrderBy(x => x);
                foreach (var d in descAMostrar)
                {
                    // Discrimina el ítem si no contiene la subcadena tipográfica especificada en su cuadro de búsqueda
                    if (!string.IsNullOrWhiteSpace(BusquedaDescripcion) && !d.Contains(BusquedaDescripcion, StringComparison.OrdinalIgnoreCase)) continue;
                    // Asegura la inserción de la propiedad de texto dentro del mapa de bits de estados del backend
                    _estadosDescripcion.TryAdd(d, false);
                    // Inyecta el nuevo nodo en la lista observable mapeada por binding para refrescar los controles visuales WPF
                    FiltroDescripcionesOpciones.Add(new OpcionFiltroCheckbox { Nombre = d, IsChecked = _estadosDescripcion[d], AlCambiarSeleccion = () => NotificarCheckboxCambiado("DESCRIPCION", d, null) });
                }
            }

            // ==========================================
            // 3. RE-POBLAR MARCAS
            // ==========================================
            // Comprueba que el origen del evento no pertenezca a la columna Marca para preservar la continuidad de la selección
            if (columnaExcluida != "MARCA")
            {
                // Mapea los identificadores y nombres comerciales de marcas cuyos activos permanecen visibles en la tabla principal
                var marcasVisibles = itemsVisibles.Where(e => e.IdMarcaNavigation != null && !string.IsNullOrEmpty(e.IdMarcaNavigation.NombreMarca))
                    .Select(e => new { Id = e.IdMarca!.Value, Nombre = e.IdMarcaNavigation.NombreMarca }).Distinct().ToList();
                // Vacía la colección observable vinculada al ComboBox de marcas del encabezado del DataGrid
                FiltroMarcasOpciones.Clear();
                // Resuelve una secuencia única de claves enteras integrando lo seleccionado por el usuario y los datos visibles
                var marcasAMostrarIds = _estadosMarca.Where(x => x.Value).Select(x => x.Key).Union(marcasVisibles.Select(v => v.Id)).Distinct();
                foreach (var idM in marcasAMostrarIds)
                {
                    // Obtiene el nombre plano de la marca desde la lista maestra o asigna un valor comodín en caso de nulos estructurales
                    var nombreM = Economicos.FirstOrDefault(e => e.IdMarca == idM)?.IdMarcaNavigation?.NombreMarca ?? "Desconocido";
                    // Ejecuta una validación contra el cuadro de texto del buscador de marcas ignorando la cultura de mayúsculas
                    if (!string.IsNullOrWhiteSpace(BusquedaMarca) && !nombreM.Contains(BusquedaMarca, StringComparison.OrdinalIgnoreCase)) continue;
                    // Guarda el registro entero de la marca en el diccionario volátil de persistencia de estados
                    _estadosMarca.TryAdd(idM, false);
                    // Añade la opción final a la estructura observable que alimenta al árbol visual en tiempo de ejecución
                    FiltroMarcasOpciones.Add(new OpcionFiltroCheckbox { Id = idM, Nombre = nombreM, IsChecked = _estadosMarca[idM], AlCambiarSeleccion = () => NotificarCheckboxCambiado("MARCA", null, idM) });
                }
            }

            // ==========================================
            // 4. RE-POBLAR SERIES
            // ==========================================
            // Omite la reconstrucción destructiva de la columna Series si el operador está haciendo clic dentro de ella
            if (columnaExcluida != "SERIE")
            {
                // Extrae de forma diferenciada las series numéricas de las maquinarias que cumplen con los filtros globales de la grilla
                var seriesVisibles = itemsVisibles.Select(e => e.Serie).Where(s => !string.IsNullOrEmpty(s)).Distinct().ToList();
                // Vacía la lista observable asignada al submenú desplegable de los números de serie del activo
                FiltroSeriesOpciones.Clear();
                // Consolida en un orden alfabético las opciones activas guardadas y las filas supervivientes del DataGrid
                var seriesAMostrar = _estadosSerie.Where(x => x.Value).Select(x => x.Key).Union(seriesVisibles).OrderBy(x => x);
                foreach (var s in seriesAMostrar)
                {
                    // Verifica si la propiedad de la serie coincide con los caracteres ingresados en su barra de búsqueda
                    if (!string.IsNullOrWhiteSpace(BusquedaSerie) && !s.Contains(BusquedaSerie, StringComparison.OrdinalIgnoreCase)) continue;
                    // Almacena de manera segura la clave en el diccionario para prevenir anomalías de desincronización
                    _estadosSerie.TryAdd(s, false);
                    // Inserta el elemento con Checkbox a la lista observable para detonar el renderizado en la UI de WPF
                    FiltroSeriesOpciones.Add(new OpcionFiltroCheckbox { Nombre = s, IsChecked = _estadosSerie[s], AlCambiarSeleccion = () => NotificarCheckboxCambiado("SERIE", s, null) });
                }
            }

            // ==========================================
            // 5. RE-POBLAR TIPO DE EQUIPO
            // ==========================================
            // Protege el control visual de Tipo de Equipo de ser limpiado si el cambio proviene de sus propios Checkboxes
            if (columnaExcluida != "TIPO_EQUIPO")
            {
                // Genera un listado de cadenas con los tipos de equipo asociados a las filas activas y visibles en la interfaz
                var tiposVisibles = itemsVisibles.Select(e => e.IdTipoEquipo).Where(t => !string.IsNullOrEmpty(t)).Distinct().ToList();
                // Vacía los elementos previos expuestos en el control ComboBox de categorías de maquinaria
                FiltroTipoEquipoOpciones.Clear();
                // Mezcla las selecciones vigentes del operador con las filas lógicas resultantes de los filtros cruzados
                var tiposAMostrar = _estadosTipoEquipo.Where(x => x.Value).Select(x => x.Key).Union(tiposVisibles).OrderBy(x => x);
                foreach (var t in tiposAMostrar)
                {
                    // Compara la cadena de texto con el parámetro de búsqueda asignado al buscador de tipo de equipo
                    if (!string.IsNullOrWhiteSpace(BusquedaTipoEquipo) && !t.Contains(BusquedaTipoEquipo, StringComparison.OrdinalIgnoreCase)) continue;
                    // Añade el elemento al diccionario para mantener persistido si está marcado o desmarcado de forma segura
                    _estadosTipoEquipo.TryAdd(t, false);
                    // Registra el ítem final dentro de la colección visible para actualizar el dropdown de la cabecera
                    FiltroTipoEquipoOpciones.Add(new OpcionFiltroCheckbox { Nombre = t, IsChecked = _estadosTipoEquipo[t], AlCambiarSeleccion = () => NotificarCheckboxCambiado("TIPO_EQUIPO", t, null) });
                }
            }

            // ==========================================
            // 6. RE-POBLAR UBICACIONES
            // ==========================================
            // Valida que la columna Ubicación no sea el foco de interacción actual para no corromper la experiencia del clic
            if (columnaExcluida != "UBICACION")
            {
                // Genera una lista distinta conteniendo las llaves primarias de las obras o ubicaciones de los registros activos
                var ubVisibles = itemsVisibles.Where(e => e.IdUbicacion.HasValue).Select(e => e.IdUbicacion.Value).Distinct().ToList();
                // Remueve la totalidad de las opciones almacenadas en el control observable de frentes de trabajo o proyectos
                FiltroUbicacionesOpciones.Clear();
                // Interconecta mediante operaciones unificadas de LINQ las selecciones guardadas y los elementos dinámicos visibles
                var ubAMostrarIds = _estadosUbicacion.Where(x => x.Value).Select(x => x.Key).Union(ubVisibles).Distinct();
                foreach (var idU in ubAMostrarIds)
                {
                    // Extrae el nombre explícito del proyecto desde la lista maestra de base de datos cargada al inicializar el módulo
                    var nombreU = ListaUbicaciones.FirstOrDefault(u => u.IdUbicacion == idU)?.NombreProyecto ?? "Sin Asignar";
                    // Aplica el filtro tipográfico basándose en el contenido de la barra de búsqueda interna de Ubicaciones
                    if (!string.IsNullOrWhiteSpace(BusquedaUbicacion) && !nombreU.Contains(BusquedaUbicacion, StringComparison.OrdinalIgnoreCase)) continue;
                    // Asegura la existencia de la llave entera dentro del mapa de control de estados lógicos del ViewModel
                    _estadosUbicacion.TryAdd(idU, false);
                    // Inserta la opción final a la lista observable para actualizar el árbol de nodos gráficos de WPF
                    FiltroUbicacionesOpciones.Add(new OpcionFiltroCheckbox { Id = idU, Nombre = nombreU, IsChecked = _estadosUbicacion[idU], AlCambiarSeleccion = () => NotificarCheckboxCambiado("UBICACION", null, idU) });
                }
            }

            // Libera la bandera de bloqueo permitiendo que el sistema procese con normalidad las interacciones subsiguientes
            _isResetting = false;
        }

        // CAPTURA LA ACCIÓN FÍSICA DEL CHECKBOX Y SINCRONIZA LOS DICCIONARIOS ESPECIFICANDO QUÉ COLUMNA MUTÓ
        private void NotificarCheckboxCambiado(string columnaOrigen, string claveTexto, int? claveId)
        {
            // Aborta de inmediato la ejecución si el sistema se encuentra bloqueado re-poblando colecciones
            if (_isResetting) return;

            // Actualiza los diccionarios de estados de texto si la clave de texto suministrada es válida
            if (claveTexto != null)
            {
                if (columnaOrigen == "ID" && FiltroIdOpciones.FirstOrDefault(x => x.Nombre == claveTexto) is var idOpt && idOpt != null) _estadosId[claveTexto] = idOpt.IsChecked;
                if (columnaOrigen == "DESCRIPCION" && FiltroDescripcionesOpciones.FirstOrDefault(x => x.Nombre == claveTexto) is var descOpt && descOpt != null) _estadosDescripcion[claveTexto] = descOpt.IsChecked;
                if (columnaOrigen == "SERIE" && FiltroSeriesOpciones.FirstOrDefault(x => x.Nombre == claveTexto) is var serOpt && serOpt != null) _estadosSerie[claveTexto] = serOpt.IsChecked;
                if (columnaOrigen == "TIPO_EQUIPO" && FiltroTipoEquipoOpciones.FirstOrDefault(x => x.Nombre == claveTexto) is var tipoOpt && tipoOpt != null) _estadosTipoEquipo[claveTexto] = tipoOpt.IsChecked;
            }

            // Actualiza los diccionarios de estados numéricos si el identificador entero posee un valor asignado
            if (claveId.HasValue)
            {
                if (columnaOrigen == "MARCA" && FiltroMarcasOpciones.FirstOrDefault(x => x.Id == claveId.Value) is var marcaOpt && marcaOpt != null) _estadosMarca[claveId.Value] = marcaOpt.IsChecked;
                if (columnaOrigen == "UBICACION" && FiltroUbicacionesOpciones.FirstOrDefault(x => x.Id == claveId.Value) is var ubOpt && ubOpt != null) _estadosUbicacion[claveId.Value] = ubOpt.IsChecked;
            }

            // Refresca la vista predeterminada de la grilla de datos para aplicar las nuevas reglas del predicado del filtro
            VistaEconomicos.Refresh();

            // Invoca la reconstrucción del resto de los filtros de las columnas pasando el identificador de la columna de origen
            RecalcularOpcionesFiltros(columnaOrigen);
        }

        private void LimpiarFiltros()
        {
            _isResetting = true;

            _busquedaId = string.Empty;
            _busquedaDescripcion = string.Empty;
            _busquedaMarca = string.Empty;
            _busquedaSerie = string.Empty;
            _busquedaTipoEquipo = string.Empty;
            _busquedaUbicacion = string.Empty;

            OnPropertyChanged(nameof(BusquedaId));
            OnPropertyChanged(nameof(BusquedaDescripcion));
            OnPropertyChanged(nameof(BusquedaMarca));
            OnPropertyChanged(nameof(BusquedaSerie));
            OnPropertyChanged(nameof(BusquedaTipoEquipo));
            OnPropertyChanged(nameof(BusquedaUbicacion));

            // Limpiamos de raíz los diccionarios de estados seleccionados
            _estadosId.Clear();
            _estadosDescripcion.Clear();
            _estadosMarca.Clear();
            _estadosSerie.Clear();
            _estadosTipoEquipo.Clear();
            _estadosUbicacion.Clear();

            _isResetting = false;

            // Restablece la tabla completa y las opciones cruzadas
            VistaEconomicos.Refresh();
            RecalcularOpcionesFiltros();
        }

        private void AbrirVentanaDetalle(string id)
        {
            DetallesWindow ventanaDetalle = new DetallesWindow(id);
            ventanaDetalle.ShowDialog();
        }

        public void CargarEconomicos()
        {
            var ubicacionesBBDD = _contextoCompartido.CatalogoUbicacionesProyectos.ToList();
            ListaUbicaciones = new ObservableCollection<CatalogoUbicacionesProyecto>(ubicacionesBBDD);

            var datosBBDD = _economicosService.ObtenerEconomicosCortos();

            Economicos.Clear();
            foreach (var item in datosBBDD)
            {
                Economicos.Add(item);
            }

            // Construcción inicial en cascada
            RecalcularOpcionesFiltros();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}