using Inventario.Core.Services.Adq_Serv.AdquisicionService;
using Inventario.Core.Services.Economicos;
using Inventario.Core.Services.UbicacionProyecto;
using Inventario.Data.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace Inventario.Desktop.ViewModels.Adq_Serv.RequisicionesViewModel
{
    public class AgregarRequisicion : INotifyPropertyChanged
    {
        private readonly AdquisicionService _adquisicionesService;
        private readonly InventarioContext _contextoBD;

        public ObservableCollection<CatalogoEconomico> ListaEconomicos { get; set; } = new ObservableCollection<CatalogoEconomico>();

        public ICollectionView EconomicosFiltrados { get; set; }

        private string _textoBusquedaId = string.Empty;
        public string TextoBusquedaId
        {
            get => _textoBusquedaId;
            set
            {
                _textoBusquedaId = value ?? string.Empty;
                OnPropertyChanged();
                EconomicosFiltrados?.Refresh();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ObservableCollection<CatalogoUbicacionesProyecto> Ubicaciones { get; set; }

        private int? _idUbicacionSeleccionado;
        public int? IdUbicacionSeleccionado
        {
            get => _idUbicacionSeleccionado;
            set
            {
                _idUbicacionSeleccionado = value;
                OnPropertyChanged();
            }
        }

        public AgregarRequisicion(AdquisicionService adquisicionesService)
        {
            _adquisicionesService = adquisicionesService;
            _contextoBD = new InventarioContext();
            Ubicaciones = new ObservableCollection<CatalogoUbicacionesProyecto>();

            CargarInformacion();
            CargarEconomicosDesdeBD();

            EconomicosFiltrados = CollectionViewSource.GetDefaultView(ListaEconomicos);
            EconomicosFiltrados.Filter = FiltroBusquedaId;
        }

        public void CargarInformacion()
        {
            var datoUbicacion = _adquisicionesService.ObtenerProyectos();
            foreach (var ubi in datoUbicacion) { Ubicaciones.Add(ubi); }
        }

        private bool FiltroBusquedaId(object obj)
        {
            if (string.IsNullOrWhiteSpace(TextoBusquedaId)) return true;

            if (obj is CatalogoEconomico economico)
            {
                if (string.IsNullOrEmpty(economico.IdEconomico)) return false;

                return economico.IdEconomico.IndexOf(TextoBusquedaId, StringComparison.OrdinalIgnoreCase) >= 0;
            }
            return false;
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

        public class OpcionComboBox
        {
            public string Texto { get; set; } = string.Empty;

            public string Valor { get; set; } = string.Empty;
        }
    }
}