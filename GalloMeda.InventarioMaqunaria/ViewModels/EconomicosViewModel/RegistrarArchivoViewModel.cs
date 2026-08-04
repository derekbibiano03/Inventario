using GalloMeda.InventarioMaqunaria;
using Inventario.Core.Services.Economicos;
using Inventario.Core.Services.Logs;
using Inventario.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Inventario.Desktop.ViewModels.EconomicosViewModel
{
    public class RegistrarArchivoViewModel : INotifyPropertyChanged
    {
        private readonly InventarioContext _contextoBD;
        private readonly LogsService _logsService;
        private readonly GestorArchivosService _gestorArchivosService;

        public ObservableCollection<CatalogoEconomico> ListaEconomicos { get; set; } = new ObservableCollection<CatalogoEconomico>();
        public ICollectionView EconomicosFiltrados { get; set; }

        private string _textoBusquedaId = string.Empty;
        public string TextoBusquedaId
        {
            get => _textoBusquedaId;
            set
            {
                _textoBusquedaId = value;
                OnPropertyChanged();
                EconomicosFiltrados?.Refresh();
            }
        }

        private string _rutaArchivoOriginal = string.Empty;
        public string RutaArchivoOriginal
        {
            get => _rutaArchivoOriginal;
            set
            {
                _rutaArchivoOriginal = value;
                OnPropertyChanged();
                NombreArchivoOriginal = string.IsNullOrEmpty(value) ? "" : Path.GetFileName(value);
                (GuardarArchivoCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        private string _nombreArchivoOriginal = string.Empty;
        public string NombreArchivoOriginal
        {
            get => _nombreArchivoOriginal;
            set { _nombreArchivoOriginal = value; OnPropertyChanged(); }
        }

        private string _fechaAltaActual = DateTime.Now.ToString("dd/MM/yyyy");
        public string FechaAltaActual
        {
            get => _fechaAltaActual;
            set
            {
                _fechaAltaActual = value;
                OnPropertyChanged();
            }
        }

        public ICommand SeleccionarArchivoCommand { get; }
        public ICommand GuardarArchivoCommand { get; }
        public int UsuarioLog { get; private set; }

        public RegistrarArchivoViewModel()
        {
            _contextoBD = new InventarioContext();
            _gestorArchivosService = new GestorArchivosService();
            _logsService = new LogsService(_contextoBD);

            SeleccionarArchivoCommand = new RelayCommand(EjecutarSeleccionarArchivo);
            GuardarArchivoCommand = new RelayCommand(EjecutarGuardarArchivo, CanGuardarArchivo);

            CargarEconomicosDesdeBD();

            EconomicosFiltrados = CollectionViewSource.GetDefaultView(ListaEconomicos)!;
            EconomicosFiltrados.Filter = FiltroBusquedaId;
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

        private bool FiltroBusquedaId(object? obj)
        {
            if (string.IsNullOrEmpty(TextoBusquedaId)) return true;

            if (obj is CatalogoEconomico economico && economico.IdEconomico != null)
            {
                return economico.IdEconomico.IndexOf(TextoBusquedaId, StringComparison.OrdinalIgnoreCase) >= 0;
            }
            return false;
        }

        private void EjecutarSeleccionarArchivo()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Archivos permitidos (*.pdf;*.png;*.jpg;*.tif;*.JPEG)|*.pdf;*.png;*.jpg;*.tif;*.JPEG",
                Title = "Seleccione el documento o imagen"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                RutaArchivoOriginal = openFileDialog.FileName;
            }
        }

        private void EjecutarGuardarArchivo()
        {
            List<string> idsSeleccionados = ListaEconomicos
                .Where(x => x.IsSelected)
                .Select(x => x.IdEconomico)
                .Where(id => id != null)
                .Select(id => id!)
                .ToList();

            if (!idsSeleccionados.Any())
            {
                MessageBox.Show("Debes seleccionar al menos un económico.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);

                return;
            }

            try
            {
                List<(CatalogoArchivo Archivo, string IdEconomico)> archivosProcesados = new List<(CatalogoArchivo, string)>();
                foreach (string idEconomico in idsSeleccionados)
                {
                    // Llama al método manual de GestorArchivosService que sube por SFTP y genera las entidades
                    var (nuevoArchivo, nuevaRelacion) = _gestorArchivosService.RegistrarArchivoEconomico(RutaArchivoOriginal, idEconomico);

                    _contextoBD.CatalogoArchivos.Add(nuevoArchivo);
                    _contextoBD.EconomicosArchivos.Add(nuevaRelacion);

                    archivosProcesados.Add((nuevoArchivo, idEconomico));
                }

                _contextoBD.SaveChanges();
                UsuarioLog = App.Session.IdUsuario;
                foreach (var item in archivosProcesados)
                {
                    _logsService.RegistrarDocumentoAdjuntoExitoso(UsuarioLog, item.Archivo.Archivo, item.IdEconomico);
                }

                MessageBox.Show($"Archivo adjuntado con éxito a {idsSeleccionados.Count} económico(s).", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                LimpiarFormulario();
            }
            catch (DbUpdateException ex)
            {
                string mensajeBD = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                MessageBox.Show($"Error de PostgreSQL al insertar registro:\n{mensajeBD}", "Error de Base de Datos", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                string mensajeReal = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                MessageBox.Show($"Error crítico al procesar archivo:\n{mensajeReal}", "Error Crítico", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanGuardarArchivo()
        {
            return !string.IsNullOrEmpty(RutaArchivoOriginal) && ListaEconomicos != null && ListaEconomicos.Any(x => x.IsSelected);
        }

        private void LimpiarFormulario()
        {
            RutaArchivoOriginal = string.Empty;
            TextoBusquedaId = string.Empty;

            foreach (var economico in ListaEconomicos)
            {
                economico.IsSelected = false;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}