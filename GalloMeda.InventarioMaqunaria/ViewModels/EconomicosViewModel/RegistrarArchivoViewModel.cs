using Inventario.Core.Services.Economicos;
using Inventario.Data.Models;
using Inventario.Data;
using Microsoft.Win32;
using System;
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
        private readonly GestorArchivosService _archivosService;
        private readonly InventarioContext _contextoBD;

        public ObservableCollection<CatalogoEconomico> ListaEconomicos { get; set; }
        public ICollectionView EconomicosFiltrados { get; set; }

        private string _textoBusquedaId;
        public string TextoBusquedaId
        {
            get => _textoBusquedaId;
            set
            {
                _textoBusquedaId = value;
                OnPropertyChanged();
                EconomicosFiltrados.Refresh();
            }
        }

        private string _rutaArchivoOriginal;
        public string RutaArchivoOriginal
        {
            get => _rutaArchivoOriginal;
            set
            {
                _rutaArchivoOriginal = value;
                OnPropertyChanged();
                NombreArchivoOriginal = string.IsNullOrEmpty(value) ? "" : Path.GetFileName(value);
            }
        }

        private string _nombreArchivoOriginal;
        public string NombreArchivoOriginal
        {
            get => _nombreArchivoOriginal;
            set { _nombreArchivoOriginal = value; OnPropertyChanged(); }
        }

        public DateTime FechaAltaActual => DateTime.Now;

        public ICommand SeleccionarArchivoCommand { get; }
        public ICommand GuardarArchivoCommand { get; }

        public RegistrarArchivoViewModel()
        {
            _archivosService = new GestorArchivosService();
            _contextoBD = new InventarioContext();

            SeleccionarArchivoCommand = new RelayCommand(EjecutarSeleccionarArchivo);
            GuardarArchivoCommand = new RelayCommand(EjecutarGuardarArchivo, CanGuardarArchivo);

            CargarEconomicosDesdeBD();

            EconomicosFiltrados = CollectionViewSource.GetDefaultView(ListaEconomicos);
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

        private bool FiltroBusquedaId(object obj)
        {
            if (string.IsNullOrEmpty(TextoBusquedaId)) return true;

            if (obj is CatalogoEconomico economico)
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
                Title = "Seleccione el documento"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                RutaArchivoOriginal = openFileDialog.FileName;
            }
        }

        private void EjecutarGuardarArchivo()
        {
            try
            {
                var economicosSeleccionados = ListaEconomicos.Where(x => x.IsSelected).ToList();

                var (archivoCatalogadoDto, _) = _archivosService.RegistrarArchivoEconomico(RutaArchivoOriginal, string.Empty);

                var nuevoArchivoBD = new CatalogoArchivo
                {
                    Archivo = archivoCatalogadoDto.Archivo,
                    NombreArchivo = archivoCatalogadoDto.NombreArchivo,
                    FechaSubida = DateTime.UtcNow 
                };

                _contextoBD.CatalogoArchivos.Add(nuevoArchivoBD);
                _contextoBD.SaveChanges(); 

                foreach (var economico in economicosSeleccionados)
                {
                    var nuevaRelacion = new EconomicosArchivo
                    {
                        IdArchivo = nuevoArchivoBD.IdArchivo, 
                        IdEconomico = economico.IdEconomico  
                    };

                    _contextoBD.EconomicosArchivos.Add(nuevaRelacion);
                }

                _contextoBD.SaveChanges();

                MessageBox.Show($"Archivo registrado exitosamente y vinculado a {economicosSeleccionados.Count} económicos.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                LimpiarFormulario();
            }
            catch (Exception ex)
            {
                string mensajeReal = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                MessageBox.Show($"Error real en la Base de Datos: {mensajeReal}", "Error Crítico", MessageBoxButton.OK, MessageBoxImage.Error);
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

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}