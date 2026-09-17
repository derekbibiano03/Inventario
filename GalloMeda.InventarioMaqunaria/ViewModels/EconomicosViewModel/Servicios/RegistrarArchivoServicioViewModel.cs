using GalloMeda.InventarioMaqunaria;
using Inventario.Core.Services.Economicos;
using Inventario.Core.Services.Logs;
using Inventario.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace Inventario.Desktop.ViewModels.EconomicosViewModel.Servicios
{
    public class RegistrarArchivoServicioViewModel : INotifyPropertyChanged
    {
        private readonly InventarioContext _contextoBD;
        private readonly LogsService _logsService;
        private readonly GestorArchivosService _gestorArchivosService;
        private readonly int _idServicio;

        public ObservableCollection<string> ListaArchivosSeleccionados { get; set; } = new ObservableCollection<string>();

        public ICommand SeleccionarArchivosCommand { get; }
        public ICommand EliminarArchivoCommand { get; }
        public ICommand GuardarArchivoCommand { get; }

        public RegistrarArchivoServicioViewModel(InventarioContext contextoBD, int idServicio)
        {
            _contextoBD = contextoBD;
            _idServicio = idServicio;
            _gestorArchivosService = new GestorArchivosService(_contextoBD);
            _logsService = new LogsService(_contextoBD);

            SeleccionarArchivosCommand = new RelayCommand(EjecutarSeleccionarArchivos);
            EliminarArchivoCommand = new RelayCommand<string>(EjecutarEliminarArchivo);
            GuardarArchivoCommand = new RelayCommand(EjecutarGuardarArchivo, CanGuardarArchivo);

            ListaArchivosSeleccionados.CollectionChanged += (s, e) =>
            {
                (GuardarArchivoCommand as RelayCommand)?.RaiseCanExecuteChanged();
            };
        }

        private void EjecutarSeleccionarArchivos()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Archivos permitidos (*.pdf;*.png;*.jpg;*.tif;*.JPEG)|*.pdf;*.png;*.jpg;*.tif;*.JPEG",
                Title = "Seleccione los documentos o imágenes del servicio",
                Multiselect = true
            };

            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string ruta in openFileDialog.FileNames)
                {
                    if (!ListaArchivosSeleccionados.Contains(ruta))
                    {
                        ListaArchivosSeleccionados.Add(ruta);
                    }
                }
            }
        }

        private void EjecutarEliminarArchivo(string? ruta)
        {
            if (!string.IsNullOrEmpty(ruta) && ListaArchivosSeleccionados.Contains(ruta))
            {
                ListaArchivosSeleccionados.Remove(ruta);
            }
        }

        private bool CanGuardarArchivo() => ListaArchivosSeleccionados.Any();

        private void EjecutarGuardarArchivo()
        {
            try
            {
                var registrosProcesados = _gestorArchivosService.RegistrarArchivosServicios(
                    ListaArchivosSeleccionados.ToList(),
                    _idServicio
                );

                _contextoBD.SaveChanges();

                int usuarioLog = App.Session.IdUsuario;
                foreach (var item in registrosProcesados)
                {
                    // Registro en bitácora idéntico al de económicos
                    _logsService.RegistrarDocumentoAdjuntoExitoso(usuarioLog, item.Archivo.Archivo, _idServicio.ToString());
                }

                MessageBox.Show("Se asociaron los archivos al servicio correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                string mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                MessageBox.Show($"Error al guardar archivos: {mensaje}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}