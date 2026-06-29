using Inventario.Core;
using Inventario.Core.DTOs;
using Inventario.Core.Services.Economicos;
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

        public Action AlCambiarSeleccion { get; set; }
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
        public ICommand ExportarExcelCommand { get; set; }
        public ObservableCollection<EconomicoMinimoDto> Economicos { get; set; }
        public ICollectionView VistaEconomicos { get; set; }
        public ObservableCollection<OpcionFiltroCheckbox> FiltroIdOpciones { get; set; } = new ObservableCollection<OpcionFiltroCheckbox>();
        public ObservableCollection<OpcionFiltroCheckbox> FiltroDescripcionesOpciones { get; set; }
        public ObservableCollection<OpcionFiltroCheckbox> FiltroMarcasOpciones { get; set; }
        public ObservableCollection<OpcionFiltroCheckbox> FiltroSeriesOpciones { get; set; }
        public ObservableCollection<OpcionFiltroCheckbox> FiltroTipoEquipoOpciones { get; set; }
        public ObservableCollection<CatalogoUbicacionesProyecto> ListaUbicaciones { get; set; }
        public ObservableCollection<OpcionFiltroCheckbox> FiltroUbicacionesOpciones { get; set; }

        public ICommand VerDetalleCommand { get; }
        public ICommand LimpiarFiltrosCommand { get; }

        public EconomicosViewModel()
        {
            VerDetalleCommand = new RelayCommand<string>(AbrirVentanaDetalle);
            LimpiarFiltrosCommand = new RelayCommand<object>(x => LimpiarFiltros());

            var contexto = new InventarioContext();
            _economicosService = new CatalogoEconomicosService(contexto);


            Economicos = new ObservableCollection<EconomicoMinimoDto>();
            FiltroDescripcionesOpciones = new ObservableCollection<OpcionFiltroCheckbox>();
            FiltroMarcasOpciones = new ObservableCollection<OpcionFiltroCheckbox>();
            FiltroSeriesOpciones = new ObservableCollection<OpcionFiltroCheckbox>();
            FiltroTipoEquipoOpciones = new ObservableCollection<OpcionFiltroCheckbox>();
            FiltroUbicacionesOpciones = new ObservableCollection<OpcionFiltroCheckbox>();

            VistaEconomicos = CollectionViewSource.GetDefaultView(Economicos);
            VistaEconomicos.Filter = FiltroEjecucion;
            ExportarExcelCommand = new RelayCommand(EjecutarExportacion);
            CargarEconomicos();
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
            
            var idsSeleccionados = FiltroIdOpciones.Where(x => x.IsChecked).Select(x => x.Nombre).ToList();


            if (idsSeleccionados.Any())
            {
                resultado = resultado && economico.IdEconomico != null && idsSeleccionados.Contains(economico.IdEconomico);
            }

            var descripcionesSeleccionadas = FiltroDescripcionesOpciones.Where(x => x.IsChecked).Select(x => x.Nombre).ToList();
            if (descripcionesSeleccionadas.Any())
            {
                resultado = resultado && economico.Descripcion != null && descripcionesSeleccionadas.Contains(economico.Descripcion);
            }

            var marcasSeleccionadas = FiltroMarcasOpciones.Where(x => x.IsChecked).Select(x => x.Id).ToList();
            if (marcasSeleccionadas.Any())
            {
                resultado = resultado && economico.IdMarca.HasValue && marcasSeleccionadas.Contains(economico.IdMarca.Value);
            }

            var seriesSeleccionadas = FiltroSeriesOpciones.Where(x => x.IsChecked).Select(x => x.Nombre).ToList();
            if (seriesSeleccionadas.Any())
            {
                resultado = resultado && economico.Serie != null && seriesSeleccionadas.Contains(economico.Serie);
            }

            var tipoequiposSeleccionadas = FiltroTipoEquipoOpciones.Where(x => x.IsChecked).Select(x => x.Nombre).ToList();
            if (tipoequiposSeleccionadas.Any())
            {
                resultado = resultado && economico.IdTipoEquipo != null && tipoequiposSeleccionadas.Contains(economico.IdTipoEquipo);
            }

            var ubicacionesSeleccionadas = FiltroUbicacionesOpciones.Where(x => x.IsChecked).Select(x => x.Id).ToList();
            if (ubicacionesSeleccionadas.Any())
            {
                resultado = resultado && economico.IdUbicacion.HasValue && ubicacionesSeleccionadas.Contains(economico.IdUbicacion.Value);
            }

            return resultado;
        }

        private void ConstruirOpcionesDeFiltros()
        {
            var idsChequeados = FiltroIdOpciones.Where(x => x.IsChecked).Select(x => x.Nombre).ToList();
            var descripcionesChequeadas = FiltroDescripcionesOpciones.Where(x => x.IsChecked).Select(x => x.Nombre).ToList();
            var marcasChequeadas = FiltroMarcasOpciones.Where(x => x.IsChecked).Select(x => x.Id).ToList();
            var seriesChequeadas = FiltroSeriesOpciones.Where(x => x.IsChecked).Select(x => x.Nombre).ToList();
            var tipoequiposChequeadas = FiltroTipoEquipoOpciones.Where(x => x.IsChecked).Select(x => x.Nombre).ToList();
            var ubicacionesChequeadas = FiltroUbicacionesOpciones.Where(x => x.IsChecked).Select(x => x.Id).ToList();

            FiltroIdOpciones.Clear();
            FiltroDescripcionesOpciones.Clear();
            FiltroMarcasOpciones.Clear();
            FiltroSeriesOpciones.Clear();
            FiltroTipoEquipoOpciones.Clear();
            FiltroUbicacionesOpciones.Clear();

            Func<EconomicoMinimoDto, bool> PasaIdEconomico = e => !idsChequeados.Any() ||
                    (e.IdEconomico != null && idsChequeados.Contains(e.IdEconomico));

            Func<EconomicoMinimoDto, bool> PasaDescripcion = e => !descripcionesChequeadas.Any() ||
                    (e.Descripcion != null && descripcionesChequeadas.Contains(e.Descripcion));

            Func<EconomicoMinimoDto, bool> PasaMarca = e => !marcasChequeadas.Any() ||
                    (e.IdMarca.HasValue && marcasChequeadas.Contains(e.IdMarca.Value));

            Func<EconomicoMinimoDto, bool> PasaSerie = e => !seriesChequeadas.Any() ||
                    (e.Serie != null && seriesChequeadas.Contains(e.Serie));

            Func<EconomicoMinimoDto, bool> PasaTipoEquipo = e => !tipoequiposChequeadas.Any() ||
                    (e.IdTipoEquipo != null && tipoequiposChequeadas.Contains(e.IdTipoEquipo));

            Func<EconomicoMinimoDto, bool> PasaUbicacion = e => !ubicacionesChequeadas.Any() ||
                    (e.IdUbicacion.HasValue && ubicacionesChequeadas.Contains(e.IdUbicacion.Value));

            var idsValidos = Economicos
            .Where(e => PasaDescripcion(e) && PasaMarca(e) && PasaSerie(e) && PasaTipoEquipo(e) && PasaUbicacion(e))
            .Select(e => e.IdEconomico)
            .Where(id => !string.IsNullOrEmpty(id))
            .Distinct().OrderBy(id => id).ToList();

            foreach (var id in idsValidos)
            {
                FiltroIdOpciones.Add(new OpcionFiltroCheckbox
                {
                    Nombre = id,
                    IsChecked = idsChequeados.Contains(id),
                    AlCambiarSeleccion = () => NotificarCambioFiltro()
                });
            }

            var descripcionesValidas = Economicos
                .Where(e => PasaIdEconomico(e) && PasaMarca(e) && PasaSerie(e) && PasaTipoEquipo(e) && PasaUbicacion(e))
        .Select(e => e.Descripcion)
        .Where(d => !string.IsNullOrEmpty(d))
        .Distinct().OrderBy(d => d).ToList();

            foreach (var desc in descripcionesValidas)
            {
                FiltroDescripcionesOpciones.Add(new OpcionFiltroCheckbox
                {
                    Nombre = desc,
                    IsChecked = descripcionesChequeadas.Contains(desc),
                    AlCambiarSeleccion = () => NotificarCambioFiltro()
                });
            }

            var marcasValidas = Economicos
                .Where(e => PasaIdEconomico(e) && PasaDescripcion(e) && PasaSerie(e) && PasaTipoEquipo(e) && PasaUbicacion(e))
                .Where(e => e.IdMarcaNavigation != null && !string.IsNullOrEmpty(e.IdMarcaNavigation.NombreMarca))
                .Select(e => new { Id = e.IdMarca!.Value, Nombre = e.IdMarcaNavigation.NombreMarca })
                .Distinct().OrderBy(m => m.Nombre).ToList();

            foreach (var m in marcasValidas)
            {
                FiltroMarcasOpciones.Add(new OpcionFiltroCheckbox
                {
                    Id = m.Id,
                    Nombre = m.Nombre,
                    IsChecked = marcasChequeadas.Contains(m.Id),
                    AlCambiarSeleccion = () => NotificarCambioFiltro()
                });
            }

            var seriesValidas = Economicos
                .Where(e => PasaIdEconomico(e) && PasaDescripcion(e) && PasaMarca(e) && PasaTipoEquipo(e) && PasaUbicacion(e))
                .Select(e => e.Serie)
                .Where(s => !string.IsNullOrEmpty(s))
                .Distinct().OrderBy(s => s).ToList();

            foreach (var serie in seriesValidas)
            {
                FiltroSeriesOpciones.Add(new OpcionFiltroCheckbox
                {
                    Nombre = serie,
                    IsChecked = seriesChequeadas.Contains(serie),
                    AlCambiarSeleccion = () => NotificarCambioFiltro()
                });
            }

            var tipoEquipoValidas = Economicos
                .Where(e => PasaIdEconomico(e) && PasaDescripcion(e) && PasaMarca(e) && PasaSerie(e) && PasaUbicacion(e))
                .Select(e => e.IdTipoEquipo)
                .Where(s => !string.IsNullOrEmpty(s))
                .Distinct().OrderBy(s => s).ToList();

            foreach (var tipoEquipo in tipoEquipoValidas)
            {
                FiltroTipoEquipoOpciones.Add(new OpcionFiltroCheckbox
                {
                    Nombre = tipoEquipo,
                    IsChecked = tipoequiposChequeadas.Contains(tipoEquipo),
                    AlCambiarSeleccion = () => NotificarCambioFiltro()
                });
            }

            var idsUbicacionesValidas = Economicos
                .Where(e => PasaIdEconomico(e) && PasaDescripcion(e) && PasaMarca(e) && PasaSerie(e) && PasaTipoEquipo(e))
                .Where(e => e.IdUbicacion.HasValue)
                .Select(e => e.IdUbicacion.Value)
                .Distinct().ToList();

            var ubicacionesValidasAagregar = ListaUbicaciones
                .Where(u => idsUbicacionesValidas.Contains(u.IdUbicacion))
                .OrderBy(u => u.NombreProyecto).ToList();

            foreach (var ub in ubicacionesValidasAagregar)
            {
                FiltroUbicacionesOpciones.Add(new OpcionFiltroCheckbox
                {
                    Id = ub.IdUbicacion,
                    Nombre = ub.NombreProyecto,
                    IsChecked = ubicacionesChequeadas.Contains(ub.IdUbicacion),
                    AlCambiarSeleccion = () => NotificarCambioFiltro()
                });
            }
        }

        private void NotificarCambioFiltro()
        {
            if (_isResetting) return;

            VistaEconomicos.Refresh();
            ConstruirOpcionesDeFiltros();
        }

        private void LimpiarFiltros()
        {
            _isResetting = true;

            foreach (var opc in FiltroIdOpciones) { opc.IsChecked = false; }
            foreach (var opc in FiltroDescripcionesOpciones) { opc.IsChecked = false; }
            foreach (var opc in FiltroMarcasOpciones) { opc.IsChecked = false; }
            foreach (var opc in FiltroSeriesOpciones) { opc.IsChecked = false; }
            foreach (var opc in FiltroTipoEquipoOpciones) { opc.IsChecked = false; }
            foreach (var opc in FiltroUbicacionesOpciones) { opc.IsChecked = false; }

            _isResetting = false;

            NotificarCambioFiltro();
        }

        private void AbrirVentanaDetalle(string id)
        {
            DetallesWindow ventanaDetalle = new DetallesWindow(id);
            ventanaDetalle.ShowDialog();
        }

        public void CargarEconomicos()
        {
            var contexto = new InventarioContext();
            var ubicacionesBBDD = contexto.CatalogoUbicacionesProyectos.ToList();

            ListaUbicaciones = new ObservableCollection<CatalogoUbicacionesProyecto>(ubicacionesBBDD);

            var datosBBDD = _economicosService.ObtenerEconomicosCortos();

            Economicos.Clear();

            foreach (var item in datosBBDD)
            {
                Economicos.Add(item);
            }

            ConstruirOpcionesDeFiltros();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}