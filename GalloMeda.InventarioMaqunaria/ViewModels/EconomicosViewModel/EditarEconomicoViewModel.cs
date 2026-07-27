using Inventario.Core.Services.Logs;
using Inventario.Data;
using Inventario.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Inventario.Desktop.ViewModels
{
    public class EditarEconomicoViewModel : INotifyPropertyChanged
    {
        public System.Action<bool>? CloseAction { get; set; }

        public ObservableCollection<CatalogoTiposEquipo> TipoEquipo { get; set; }

        public ObservableCollection<CatalogoGrupo> Grupos { get; set; }

        public ObservableCollection<CatalogoMarca> Marcas { get; set; }

        public ObservableCollection<CatalogoEstatus> Estatus { get; set; }

        public ObservableCollection<CatalogoTiposCombustible> Combustibles { get; set; }

        public ObservableCollection<CatalogoPya> PYA { get; set; }

        public ObservableCollection<CatalogoOperadore> Operadores { get; set; }

        public ObservableCollection<CatalogoResponsableMaquinarium> Responsables { get; set; }

        public ObservableCollection<CatalogoUbicacionesProyecto> Ubicaciones { get; set; }

        private CatalogoEconomico? _economicoEdicion;

        public CatalogoEconomico? EconomicoEdicion
        {
            get => _economicoEdicion;
            set { _economicoEdicion = value; OnPropertyChanged(); }
        }

        public ICommand GuardarCambiosCommand { get; private set; }

        public ICommand CancelarCommand { get; private set; }

        public EditarEconomicoViewModel()
        {
            TipoEquipo = new ObservableCollection<CatalogoTiposEquipo>();

            Grupos = new ObservableCollection<CatalogoGrupo>();

            Marcas = new ObservableCollection<CatalogoMarca>();

            Estatus = new ObservableCollection<CatalogoEstatus>();

            Combustibles = new ObservableCollection<CatalogoTiposCombustible>();

            PYA = new ObservableCollection<CatalogoPya>();

            Operadores = new ObservableCollection<CatalogoOperadore>();

            Responsables = new ObservableCollection<CatalogoResponsableMaquinarium>();

            Ubicaciones = new ObservableCollection<CatalogoUbicacionesProyecto>();

            GuardarCambiosCommand = new RelayCommand(EjecutarGuardarCambios);

            CancelarCommand = new RelayCommand(EjecutarCancelar);

            CargarCatalogos();
        }

        private void CargarCatalogos()
        {
            using (var contexto = new InventarioContext())
            {
                var tiposDb = contexto.CatalogoTiposEquipos.ToList();
                foreach (var item in tiposDb) TipoEquipo.Add(item);

                var gruposDb = contexto.CatalogoGrupos.ToList();
                foreach (var item in gruposDb) Grupos.Add(item);

                var marcasDb = contexto.CatalogoMarcas.ToList();
                foreach (var item in marcasDb) Marcas.Add(item);

                var estatusDb = contexto.CatalogoEstatuses.ToList();
                foreach (var item in estatusDb) Estatus.Add(item);

                var combustiblesDb = contexto.CatalogoTiposCombustibles.ToList();
                foreach (var item in combustiblesDb) Combustibles.Add(item);

                var pyaDb = contexto.CatalogoPyas.ToList();
                foreach (var item in pyaDb) PYA.Add(item);

                var operadoresDb = contexto.CatalogoOperadores.ToList();
                foreach (var item in operadoresDb) Operadores.Add(item);

                var responsablesDb = contexto.CatalogoResponsableMaquinaria.ToList();
                foreach (var item in responsablesDb) Responsables.Add(item);

                var ubicacionesDb = contexto.CatalogoUbicacionesProyectos.ToList();
                foreach (var item in ubicacionesDb) Ubicaciones.Add(item);
            }
        }

        public void CargarDatosEconomico(string idEconomico)
        {
            using (var contexto = new InventarioContext())
            {
                var economico = contexto.CatalogoEconomicos.FirstOrDefault(e => e.IdEconomico == idEconomico);

                if (economico != null)
                {
                    EconomicoEdicion = economico;
                }
            }
        }

        private void EjecutarGuardarCambios()
        {
            if (EconomicoEdicion == null) return;

            try
            {
                using (var contexto = new InventarioContext())
                {
                    var logsService = new LogsService(contexto);

                    contexto.Entry(EconomicoEdicion).State = EntityState.Modified;

                    contexto.SaveChanges();

                    logsService.RegistrarModificacionEquipo(GalloMeda.InventarioMaqunaria.App.Session.IdUsuario, EconomicoEdicion.IdEconomico);
                }

                CloseAction?.Invoke(true);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Ocurrió un error al guardar los cambios: {ex.Message}", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private void EjecutarCancelar()
        {
            CloseAction?.Invoke(false);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}