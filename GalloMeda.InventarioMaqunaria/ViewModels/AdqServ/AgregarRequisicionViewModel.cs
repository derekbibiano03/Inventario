using ClosedXML.Excel;
using GalloMeda.InventarioMaqunaria;
using Inventario.Core.Services.Auth;
using Inventario.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

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
        public ObservableCollection<CatalogoUbicacionesProyecto> Ubicaciones { get; }
        public ObservableCollection<CatalogoEconomico> Economicos { get; }

        // CORREGIDO: Se inicializa la colección para evitar excepciones de referencia nula
        public ObservableCollection<Usuario> Usuarios { get; } = new();

        private readonly InventarioContext _contexto;
        private readonly UsuariosService _usuariosService;

        public ICommand AñadirConceptoCommand { get; }
        public ICommand EliminarConceptoCommand { get; }
        public ICommand GenerarExcelCommand { get; }

        // --- Propiedades Automatizadas de Usuarios ---
        private Usuario? _usuarioAtencionSeleccionado;
        public Usuario? UsuarioAtencionSeleccionado
        {
            get => _usuarioAtencionSeleccionado;
            set
            {
                _usuarioAtencionSeleccionado = value;
                OnPropertyChanged();
                AtencionNombre = value?.NombreCompleto;
                AtencionDepto = value?.Area;
            }
        }

        private Usuario? _usuarioAutorizanteSeleccionado;
        public Usuario? UsuarioAutorizanteSeleccionado
        {
            get => _usuarioAutorizanteSeleccionado;
            set
            {
                _usuarioAutorizanteSeleccionado = value;
                OnPropertyChanged();
                FirmaAutorizante = value?.NombreCompleto;
            }
        }
        // ---------------------------------------------

        private string? _ubicacionSeleccionada;
        public string? UbicacionSeleccionada
        {
            get => _ubicacionSeleccionada;
            set { _ubicacionSeleccionada = value; OnPropertyChanged(); }
        }

        private string? _empresaSeleccionada;
        public string? EmpresaSeleccionada
        {
            get => _empresaSeleccionada;
            set { _empresaSeleccionada = value; OnPropertyChanged(); }
        }

        private DateTime _fechaActual = DateTime.Now;
        public DateTime FechaActual
        {
            get => _fechaActual;
            set { _fechaActual = value; OnPropertyChanged(); }
        }

        private string? _atencionNombre;
        public string? AtencionNombre
        {
            get => _atencionNombre;
            set { _atencionNombre = value; OnPropertyChanged(); }
        }

        private string? _atencionDepto;
        public string? AtencionDepto
        {
            get => _atencionDepto;
            set { _atencionDepto = value; OnPropertyChanged(); }
        }

        private string? _solicitanteNombre;
        public string? SolicitanteNombre
        {
            get => _solicitanteNombre;
            set { _solicitanteNombre = value; OnPropertyChanged(); }
        }

        private string? _solicitanteDepto;
        public string? SolicitanteDepto
        {
            get => _solicitanteDepto;
            set { _solicitanteDepto = value; OnPropertyChanged(); }
        }

        private string? _observaciones;
        public string? Observaciones
        {
            get => _observaciones;
            set { _observaciones = value; OnPropertyChanged(); }
        }

        private string? _firmaSolicitante;
        public string? FirmaSolicitante
        {
            get => _firmaSolicitante;
            set { _firmaSolicitante = value; OnPropertyChanged(); }
        }

        private string? _firmaAutorizante;
        public string? FirmaAutorizante
        {
            get => _firmaAutorizante;
            set { _firmaAutorizante = value; OnPropertyChanged(); }
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
        private string? _serieUnidad;
        public string? SerieUnidad
        {
            get => _serieUnidad;
            set { _serieUnidad = value; OnPropertyChanged(); }
        }

        public ObservableCollection<CatalogoMarca> ListaMarcas { get; set; } = new();
        public ObservableCollection<CatalogoTiposCombustible> ListaMotores { get; set; } = new();

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

        public AgregarRequisicionViewModel(InventarioContext contexto, UsuariosService usuariosService)
        {
            _usuariosService = usuariosService;
            _contexto = contexto;
            Ubicaciones = new ObservableCollection<CatalogoUbicacionesProyecto>();
            Economicos = new ObservableCollection<CatalogoEconomico>();
            ListaMarcas = new ObservableCollection<CatalogoMarca>();
            ListaMotores = new ObservableCollection<CatalogoTiposCombustible>();

            AñadirConceptoCommand = new RelayCommand(EjecutarAñadirConcepto);
            EliminarConceptoCommand = new RelayCommand(EjecutarEliminarConcepto);
            GenerarExcelCommand = new RelayCommand(EjecutarGenerarExcel);

            CargarUsuarios();
            CargarCatalogos();
        }

        public void CargarUsuarios()
        {
            Usuarios.Clear();
            var datosusuarios = _usuariosService.ObtenerUsuarios();
            foreach (var usuario in datosusuarios)
            {
                Usuarios.Add(usuario);
            }
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

        private void EjecutarGenerarExcel(object? obj)
        {
            try
            {
                using (var workbook = new XLWorkbook())
                {
                    var ws = workbook.Worksheets.Add("Requisición");
                    ws.ShowGridLines = true;

                    ws.Column(1).Width = 8;
                    ws.Column(2).Width = 8.78;
                    ws.Column(3).Width = 8.67;
                    ws.Column(4).Width = 46.44;
                    ws.Column(5).Width = 17.56;
                    ws.Column(6).Width = 14;
                    ws.Column(7).Width = 14;
                    ws.Column(8).Width = 13;

                    var rangoIcono = ws.Range("A1:C1");
                    rangoIcono.Merge();
                    if (EmpresaSeleccionada == "CGM")
                    {
                        var icono = "../../../Resources/gallo_meda_icon.png";
                        ws.AddPicture(icono)
                      .MoveTo(ws.Cell("A1"))
                      .WithSize(150, 50);
                    } else if (EmpresaSeleccionada == "OX")
                    {
                        var icono = "../../../Resources/grupo_ox.jpeg";
                        ws.AddPicture(icono)
                      .MoveTo(ws.Cell("A1"))
                      .WithSize(150, 50);
                    }
                        

                    var rangoTitulo = ws.Range("E1:G1");
                    rangoTitulo.Merge();
                    rangoTitulo.FirstCell().Value = "REQUISICIÓN DE COMPRA";
                    rangoTitulo.FirstCell().Style.Font.Bold = true;
                    rangoTitulo.FirstCell().Style.Font.FontSize = 16;
                    rangoTitulo.FirstCell().Style.Font.FontColor = XLColor.FromHtml("#1F4E78");

                    var rangoSub = ws.Range("E2:H2");
                    rangoSub.Merge();
                    rangoSub.FirstCell().Value = $"{EmpresaSeleccionada} - RC - 000 - {UbicacionSeleccionada} - {FechaActual:yyyy} ";
                    rangoSub.FirstCell().Style.Font.Italic = true;

                    var rangoUbi = ws.Range("E3:H3");
                    rangoUbi.Merge();
                    rangoUbi.FirstCell().Value = $"{UbicacionSeleccionada} - {FechaActual:yyyy} ";
                    rangoUbi.FirstCell().Style.Font.Italic = true;

                    var rangoInfo = ws.Range("A4:C4");
                    rangoInfo.Merge();
                    rangoInfo.FirstCell().Value = "INFORMACIÓN GENERAL";
                    rangoInfo.FirstCell().Style.Font.Bold = true;
                    rangoInfo.FirstCell().Style.Font.FontColor = XLColor.FromHtml("#1F4E78");

                    var r5_ab = ws.Range("A5:B5"); r5_ab.Merge(); r5_ab.FirstCell().Value = "Atención A:";
                    var r5_cd = ws.Range("C5:D5"); r5_cd.Merge(); r5_cd.FirstCell().Value = AtencionNombre;

                    ws.Cell("E5").Value = "Depto. Atención:";
                    var r5_fgh = ws.Range("F5:H5"); r5_fgh.Merge(); r5_fgh.FirstCell().Value = AtencionDepto;

                    var r6_ab = ws.Range("A6:B6"); r6_ab.Merge(); r6_ab.FirstCell().Value = "Solicitante:";
                    var r6_cd = ws.Range("C6:D6"); r6_cd.Merge(); r6_cd.FirstCell().Value = App.Session.NombreCompleto;

                    ws.Cell("E6").Value = "Depto. Solicitante:";
                    var r6_fgh = ws.Range("F6:H6"); r6_fgh.Merge(); r6_fgh.FirstCell().Value = App.Session.Area;

                    var rangoEquipo = ws.Range("A9:C9");
                    rangoEquipo.Merge();
                    rangoEquipo.FirstCell().Value = "DATOS DEL EQUIPO O UNIDAD";
                    rangoEquipo.FirstCell().Style.Font.Bold = true;
                    rangoEquipo.FirstCell().Style.Font.FontColor = XLColor.FromHtml("#1F4E78");

                    var r10_ab = ws.Range("A10:B10"); r10_ab.Merge(); r10_ab.FirstCell().Value = "No. Económico:";
                    var r10_cd = ws.Range("C10:D10"); r10_cd.Merge(); r10_cd.FirstCell().Value = EconomicoSeleccionado?.IdEconomico;

                    var r11_ab = ws.Range("A11:B11"); r11_ab.Merge(); r11_ab.FirstCell().Value = "Descripcion:";
                    var r11_cd = ws.Range("C11:D11"); r11_cd.Merge(); r11_cd.FirstCell().Value = DescripcionUnidad;

                    var r12_ab = ws.Range("A12:B12"); r12_ab.Merge(); r12_ab.FirstCell().Value = "Marca:";
                    var r12_cd = ws.Range("C12:D12"); r12_cd.Merge(); r12_cd.FirstCell().Value = MarcaUnidad;

                    var r13_ab = ws.Range("A13:B13"); r13_ab.Merge(); r13_ab.FirstCell().Value = "Modelo:";
                    var r13_cd = ws.Range("C13:D13"); r13_cd.Merge(); r13_cd.FirstCell().Value = ModeloUnidad;

                    var r14_ab = ws.Range("A14:B14"); r14_ab.Merge(); r14_ab.FirstCell().Value = "No. Serie:";
                    var r14_cd = ws.Range("C14:D14"); r14_cd.Merge(); r14_cd.FirstCell().Value = SerieUnidad;

                    ws.Cell("E10").Value = "Tipo de Motor:";
                    var r10_fgh = ws.Range("F10:H10"); r10_fgh.Merge(); r10_fgh.FirstCell().Value = MotorUnidad;

                    ws.Cell("E11").Value = "Marca de Motor:";
                    var r11_fgh = ws.Range("F11:H11"); r11_fgh.Merge(); r11_fgh.FirstCell().Value = MotorMarca;

                    ws.Cell("E12").Value = "Modelo de Motor:";
                    var r12_fgh = ws.Range("F12:H12"); r12_fgh.Merge(); r12_fgh.FirstCell().Value = MotorModelo;

                    ws.Cell("E13").Value = "Serie de Motor:";
                    var r13_fgh = ws.Range("F13:H13"); r13_fgh.Merge(); r13_fgh.FirstCell().Value = MotorSerie;

                    var rangoConceptos = ws.Range("A16:C16");
                    rangoConceptos.Merge();
                    rangoConceptos.FirstCell().Value = "CONCEPTOS / PARTIDAS";
                    rangoConceptos.FirstCell().Style.Font.Bold = true;
                    rangoConceptos.FirstCell().Style.Font.FontColor = XLColor.FromHtml("#1F4E78");

                    string[] headers = { "Partida", "Cantidad", "Unidad", "Descripción", "No. Parte", "No. Equivalente", "Conj", "SubConj" };
                    int headerRow = 17;

                    for (int i = 0; i < headers.Length; i++)
                    {
                        var cell = ws.Cell(headerRow, i + 1);
                        cell.Value = headers[i];
                        cell.Style.Font.Bold = true;
                        cell.Style.Font.FontColor = XLColor.White;
                        cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#1F4E78");
                        cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    }

                    int rowIdx = 18;
                    foreach (var det in Detalles)
                    {
                        ws.Cell(rowIdx, 1).Value = det.Partida;
                        ws.Cell(rowIdx, 2).Value = det.Cantidad;
                        ws.Cell(rowIdx, 3).Value = det.Unidad;
                        ws.Cell(rowIdx, 4).Value = det.Descripcion;
                        ws.Cell(rowIdx, 5).Value = det.NoPartida;
                        ws.Cell(rowIdx, 6).Value = det.NoEquivalente;
                        ws.Cell(rowIdx, 7).Value = det.Catalogo;
                        ws.Cell(rowIdx, 8).Value = det.Pagina;

                        for (int c = 1; c <= headers.Length; c++)
                        {
                            ws.Cell(rowIdx, c).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            ws.Cell(rowIdx, c).Style.Border.BottomBorderColor = XLColor.LightGray;
                        }
                        rowIdx++;
                    }

                    int filaObservaciones = 60;
                    int filaObservacionesBaja = 61;
                    ws.Cell(filaObservaciones, 1).Value = "Observaciones:";
                    ws.Cell(filaObservaciones, 1).Style.Font.Bold = true;

                    var rangoObservaciones = ws.Range(filaObservacionesBaja, 1, filaObservacionesBaja + 2, 8);
                    rangoObservaciones.Merge();
                    rangoObservaciones.FirstCell().Value = Observaciones;
                    rangoObservaciones.Style.Alignment.WrapText = true;
                    rangoObservaciones.Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;

                    int filaFirmas = filaObservaciones + 5;
                    ws.Cell(filaFirmas, 2).Value = "___________________________________";
                    ws.Cell(filaFirmas, 5).Value = "___________________________________";

                    filaFirmas++;
                    ws.Cell(filaFirmas, 2).Value = $"Firma Solicitante: {App.Session.NombreCompleto}";
                    ws.Cell(filaFirmas, 5).Value = $"Firma Autorizador: {FirmaAutorizante}";
                    ws.Cell(filaFirmas, 2).Style.Font.Bold = true;
                    ws.Cell(filaFirmas, 5).Style.Font.Bold = true;

                    ws.PageSetup.PrintAreas.Add("A1:H80");
                    ws.PageSetup.FitToPages(1, 0);
                    ws.PageSetup.Footer.Center.AddText("Constructora Gallo Meda S.A. de C.V. Detroit 16. Col. Ferrocarril. Guadalajara. Jalisco. Mexico. C.P. 44440 Tel. (3339423080)");

                    var saveFileDialog = new SaveFileDialog
                    {
                        Filter = "Excel Files|*.xlsx",
                        FileName = $"Requisicion_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
                    };

                    if (saveFileDialog.ShowDialog() == true)
                    {
                        workbook.SaveAs(saveFileDialog.FileName);
                        MessageBox.Show("Archivo Excel generado exitosamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ocurrió un error al generar el Excel: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CargarCatalogos()
        {
            var ubicacionesDb = _contexto.CatalogoUbicacionesProyectos
                .Where(e => e.Siglas != "")
                .AsNoTracking()
                .ToList();

            Ubicaciones.Clear();
            foreach (var item in ubicacionesDb) { Ubicaciones.Add(item); }

            var economicosDb = _contexto.CatalogoEconomicos.AsNoTracking().ToList();
            Economicos.Clear();
            foreach (var item in economicosDb) { Economicos.Add(item); }

            var marcasDb = _contexto.Set<CatalogoMarca>().AsNoTracking().ToList();
            ListaMarcas.Clear();
            foreach (var marca in marcasDb) { ListaMarcas.Add(marca); }

            var combustiblesDb = _contexto.Set<CatalogoTiposCombustible>().AsNoTracking().ToList();
            ListaMotores.Clear();
            foreach (var combustible in combustiblesDb) { ListaMotores.Add(combustible); }
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action<object?> _ejecutar;
        private readonly Func<object?, bool>? _puedeEjecutar;

        public RelayCommand(Action<object?> ejecutar, Func<object?, bool>? puedeEjecutar = null)
        {
            _ejecutar = ejecutar;
            _puedeEjecutar = _puedeEjecutar;
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