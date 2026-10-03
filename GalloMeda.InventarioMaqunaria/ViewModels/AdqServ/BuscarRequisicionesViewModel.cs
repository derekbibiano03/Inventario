using GalloMeda.InventarioMaqunaria;
using Inventario.Core.DTOs.Requisicion;
using Inventario.Core.Services;
using Inventario.Core.Services.Adq_Serv.AdquisicionService;
using Inventario.Data.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Inventario.Desktop.ViewModels.AdqServ
{
    public class BuscarRequisicionesViewModel : INotifyPropertyChanged
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

        private readonly AdquisicionService _reqService;

        // Inyectamos AdquisicionService directamente en el constructor
        public BuscarRequisicionesViewModel(AdquisicionService reqService)
        {
            _reqService = reqService ?? throw new ArgumentNullException(nameof(reqService));
            Usuarios = new ObservableCollection<Usuario>();
            VerExcelCommand = new RelayCommand(_ => AbrirArchivoExcel());
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