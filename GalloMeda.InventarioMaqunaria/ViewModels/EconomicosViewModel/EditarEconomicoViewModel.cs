using Inventario.Data;
using Inventario.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input; // OBLIGATORIO: Permite usar la interfaz ICommand

namespace Inventario.Desktop.ViewModels
{
    public class EditarEconomicoViewModel : INotifyPropertyChanged
    {
        public System.Action<bool> CloseAction { get; set; }

        // Colecciones observables para los ComboBoxes de la UI.
        public ObservableCollection<CatalogoTiposEquipo> TipoEquipo { get; set; }
        public ObservableCollection<CatalogoGrupo> Grupos { get; set; }
        public ObservableCollection<CatalogoMarca> Marcas { get; set; }
        public ObservableCollection<CatalogoEstatus> Estatus { get; set; }
        public ObservableCollection<CatalogoTiposCombustible> Combustibles { get; set; }
        public ObservableCollection<CatalogoPya> PYA { get; set; }
        public ObservableCollection<CatalogoOperadore> Operadores { get; set; }
        public ObservableCollection<CatalogoResponsableMaquinarium> Responsables { get; set; }
        public ObservableCollection<CatalogoUbicacionesProyecto> Ubicaciones { get; set; }

        private CatalogoEconomico _economicoEdicion;
        public CatalogoEconomico EconomicoEdicion
        {
            get => _economicoEdicion;
            set { _economicoEdicion = value; OnPropertyChanged(); }
        }

        // CORRECCIÓN: Declaración de las propiedades Command requeridas obligatoriamente por el XAML.
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

            // CORRECCIÓN: Inicialización de los comandos vinculándolos a sus respectivos métodos internos.
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

        // CORRECCIÓN: Método que se dispara de forma automática cuando el usuario pulsa el botón "Guardar Cambios".
        private void EjecutarGuardarCambios()
        {
            // Verifica que el objeto a editar no se encuentre vacío antes de intentar operar con la base de datos.
            if (EconomicoEdicion == null) return;

            try
            {
                // Abre el contexto de Entity Framework para impactar PostgreSQL.
                using (var contexto = new InventarioContext())
                {
                    contexto.CatalogoEconomicos.Update(EconomicoEdicion);

                    // Sincroniza y guarda los cambios de forma persistente en la base de datos.
                    contexto.SaveChanges();
                }

                // Cierra la ventana actual notificando un resultado exitoso (true) a través del delegado CloseAction.
                CloseAction?.Invoke(true);
            }
            catch (Exception ex)
            {
                // Manejo básico de excepciones en caso de fallo de red, restricciones de BD o errores inesperados.
                System.Windows.MessageBox.Show($"Ocurrió un error al guardar los cambios: {ex.Message}", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        // CORRECCIÓN: Método que se dispara de forma automática cuando el usuario pulsa el botón "Cancelar".
        private void EjecutarCancelar()
        {
            // Cierra la ventana de inmediato retornando un valor falso (false) indicando que la operación fue abortada.
            CloseAction?.Invoke(false);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}