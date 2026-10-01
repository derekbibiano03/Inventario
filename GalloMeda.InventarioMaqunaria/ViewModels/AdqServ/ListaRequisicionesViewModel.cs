using GalloMeda.InventarioMaqunaria;
using Inventario.Core.DTOs.Requisicion;
using Inventario.Core.Services.Adq_Serv.AdquisicionService;
using Inventario.Core.Services.Catalogos;
using Inventario.Data.Models;
using Microsoft.Win32; // <--- Usamos el SaveFileDialog correcto de WPF
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Inventario.Desktop.ViewModels.AdqServ
{
    public class ListaRequisicionesViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public ObservableCollection<Usuario> Usuarios { get; } = new();

        private ObservableCollection<RequisicionListDto> _requisiciones = new();
        public ObservableCollection<RequisicionListDto> Requisiciones
        {
            get => _requisiciones;
            set { _requisiciones = value; OnPropertyChanged(); }
        }

        private RequisicionListDto? _requisicionSeleccionada;
        public RequisicionListDto? RequisicionSeleccionada
        {
            get => _requisicionSeleccionada;
            set { _requisicionSeleccionada = value; OnPropertyChanged(); }
        }

        public ICommand VerExcelCommand { get; }

        private readonly InventarioContext _contexto;
        private readonly AdquisicionService _reqService;
        public ICommand AutorizarCommand { get; }


        public ListaRequisicionesViewModel()
        {
            var contexto = new InventarioContext();
            _reqService = new AdquisicionService(contexto);
            Usuarios = new ObservableCollection<Usuario>();
            VerExcelCommand = new RelayCommand(_ => AbrirArchivoExcel());
            AutorizarCommand = new RelayCommand(_ => AutorizarArchivo());

            _ = CargarRequisicionesAsync();
        }

        public async Task CargarRequisicionesAsync()
        {
            try
            {
                if (App.Session == null)
                {
                    MessageBox.Show("App.Session es nulo.", "Depuración");
                    return;
                }

                var datosReq = await _reqService.ObtenerRequisicionesAsync(App.Session.IdUsuario);

                Requisiciones.Clear();
                foreach (var dato in datosReq)
                {
                    Requisiciones.Add(dato);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"StackTrace: {ex.StackTrace}\n\nMensaje: {ex.Message}", "Depuración de Error Nulo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        public async Task CargarRequisicionesSinRevisionAsync()
        {
            try
            {
                if (App.Session == null)
                {
                    MessageBox.Show("App.Session es nulo.", "Depuración");
                    return;
                }

                var datosReq = await _reqService.ObtenerRequisicionesAsync(App.Session.IdUsuario);

                Requisiciones.Clear();
                foreach (var dato in datosReq)
                {
                    Requisiciones.Add(dato);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"StackTrace: {ex.StackTrace}\n\nMensaje: {ex.Message}", "Depuración de Error Nulo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private async void AutorizarArchivo()
        {
            if (RequisicionSeleccionada == null)
            {
                MessageBox.Show("Por favor, seleccione una requisición de la lista.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(RequisicionSeleccionada.ArchivoReq))
            {
                MessageBox.Show("La requisición seleccionada no cuenta con un archivo adjunto.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                string firmaPathUsuario = App.Session?.FirmaPath ?? string.Empty;

                if (string.IsNullOrWhiteSpace(firmaPathUsuario))
                {
                    MessageBox.Show("El usuario actual no cuenta con una ruta de firma registrada en la sesión.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string rutaTemporalFirmada = _reqService.AutorizarYDescargarArchivo(
                    RequisicionSeleccionada.IdRequisicion,
                    RequisicionSeleccionada.ArchivoReq,
                    firmaPathUsuario
                );

                var saveFileDialog = new SaveFileDialog
                {
                    Filter = "Excel Files|*.xlsx",
                    FileName = RequisicionSeleccionada.ArchivoReq
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    File.Copy(rutaTemporalFirmada, saveFileDialog.FileName, true);
                }

                await CargarRequisicionesAsync();

                MessageBox.Show("Requisición autorizada, estatus cambiado a 'AUTORIZADA' y archivo exportado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"No se pudo autorizar el archivo: {ex.Message}", "Error Crítico", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AbrirArchivoExcel()
        {
            try
            {
                if (RequisicionSeleccionada == null)
                {
                    MessageBox.Show("Por favor, seleccione una requisición de la lista.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrEmpty(RequisicionSeleccionada.ArchivoReq))
                {
                    MessageBox.Show("La requisición seleccionada no cuenta con un archivo adjunto.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                _reqService.DescargarYAbrirArchivoExcel(RequisicionSeleccionada.ArchivoReq);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"DETALLE DEL ERROR:\n\nMensaje: {ex.Message}\n\nStackTrace:\n{ex.StackTrace}",
                                "Error en AbrirArchivoExcel", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}