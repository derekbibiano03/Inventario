using Azure.Identity;
using GalloMeda.InventarioMaqunaria;
using Inventario.Core.DTOs.Requisicion;
using Inventario.Core.Services.Adq_Serv.AdquisicionService;
using Inventario.Data.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
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

        public async Task EnviarCorreoNotificacionConArchivoAsync(List<string> correosDestino, List<string> correosCc, string asunto, string cuerpoHtml, string rutaArchivo)
        {
            // Construir el lector para appsettings.json de manera local en el método
            var configuration = new Microsoft.Extensions.Configuration.ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            string tenantId = configuration["AzureAd:TenantId"];
            string clientId = configuration["AzureAd:ClientId"];
            string clientSecret = configuration["AzureAd:ClientSecret"];
            string remitenteCorreo = configuration["AzureAd:Remitente"];

            var options = new ClientSecretCredentialOptions();

            var clientSecretCredential = new ClientSecretCredential(
                tenantId, clientId, clientSecret, options);

            var graphClient = new GraphServiceClient(clientSecretCredential, new[] { "https://graph.microsoft.com/.default" });

            byte[] fileBytes = File.ReadAllBytes(rutaArchivo);
            string fileName = Path.GetFileName(rutaArchivo);

            var toRecipientsList = correosDestino.Select(email => new Recipient
            {
                EmailAddress = new EmailAddress
                {
                    Address = email
                }
            }).ToList();

            var ccRecipientsList = correosCc.Select(email => new Recipient
            {
                EmailAddress = new EmailAddress
                {
                    Address = email
                }
            }).ToList();

            var requestBody = new Microsoft.Graph.Users.Item.SendMail.SendMailPostRequestBody
            {
                Message = new Message
                {
                    Subject = asunto,
                    Body = new ItemBody
                    {
                        ContentType = BodyType.Html,
                        Content = cuerpoHtml
                    },
                    ToRecipients = toRecipientsList,
                    CcRecipients = ccRecipientsList,
                    Attachments = new List<Attachment>
                    {
                        new FileAttachment
                        {
                            OdataType = "#microsoft.graph.fileAttachment",
                            Name = fileName,
                            ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            ContentBytes = fileBytes
                        }
                    }
                },
                SaveToSentItems = true
            };

            await graphClient.Users[remitenteCorreo].SendMail.PostAsync(requestBody);
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

                // Invocación correcta del método actualizado del servicio
                string rutaTemporalExcelFirmado = _reqService.AutorizarYDescargarArchivo(
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
                    if (File.Exists(rutaTemporalExcelFirmado))
                    {
                        File.Copy(rutaTemporalExcelFirmado, saveFileDialog.FileName, true);
                    }

                    var destinatarios = new List<string> { RequisicionSeleccionada.CorreoAtencion };
                    var copias = new List<string> { "rrodriguez@enlaceferroviario.com", "egarcia@enlaceferroviario.com" };

                    // Envío del archivo firmado por correo
                    await EnviarCorreoNotificacionConArchivoAsync(
                        correosDestino: destinatarios,
                        correosCc: copias,
                        asunto: $"Requisición Autorizada - {RequisicionSeleccionada.IdRequisicion}",
                        cuerpoHtml: $"<p>Hola Malcom,</p><p>La requisición <b>{RequisicionSeleccionada.IdRequisicion}</b> ha sido autorizada. Se adjunta el formato de Excel firmado.</p>",
                        rutaArchivo: saveFileDialog.FileName
                    );
                }

                await CargarRequisicionesAsync();

                MessageBox.Show("Requisición autorizada, estatus cambiado a 'AUTORIZADA' y archivo enviado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                string mensajeReal = ex.InnerException?.Message ?? ex.Message;
                MessageBox.Show($"No se pudo autorizar el archivo: {mensajeReal}", "Error Crítico", MessageBoxButton.OK, MessageBoxImage.Error);
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