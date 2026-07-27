using GalloMeda.InventarioMaqunaria;
using Inventario.Desktop.Views.UserControllers;
using Inventario.Desktop.Views.UserControllers.Adquisiciones_Servicios.Requisiciones;
using Inventario.Desktop.Views.UserControllers.Auth;
using Inventario.Desktop.Views.UserControllers.Catalogos;
using Inventario.Desktop.Views.UserControllers.Economicos;
using Inventario.Desktop.Views.UserControllers.Personal;
using Inventario.Desktop.Views.UserControllers.Proveedores;
using Inventario.Desktop.Views.UserControllers.UbicacionProyectos;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace Inventario.Desktop.Views
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private string _usuarioLogueado = string.Empty;

        public string UsuarioLogueado
        {
            get => _usuarioLogueado;
            set
            {
                _usuarioLogueado = value;
                OnPropertyChanged();
            }
        }

        public MainWindow(string username)
        {
            InitializeComponent();
            ContenedorPrincipal.Content = new EconomicosView();
            string textoPanel = "INVENTARIO DE MAQUINARIA";

            this.DataContext = this;
            this.UsuarioLogueado = username;
            txtPanel.Text = textoPanel;
        }

        private void BtnColapsar_Click(object sender, RoutedEventArgs e)
        {
            ColumnaMenu.Width = new GridLength(0);
            BtnMostrarMenu.Visibility = Visibility.Visible;
        }

        private void BtnMostrar_Click(object sender, RoutedEventArgs e)
        {
            ColumnaMenu.Width = new GridLength(220);
            BtnMostrarMenu.Visibility = Visibility.Collapsed;
        }

        private void BtnProductosMenu_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button boton && boton.ContextMenu != null)
            {
                boton.ContextMenu.PlacementTarget = boton;
                boton.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                boton.ContextMenu.IsOpen = true;
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            string textoPanel = "";
            if (sender is MenuItem itemPresionado)
            {
                string? ID_Interno = itemPresionado.Tag?.ToString();

                switch (ID_Interno)
                {
                    case "AddUser":
                        ContenedorPrincipal.Content = new AgregarUsuario();
                        textoPanel = "AGREGAR A UN NUEVO USUARIO";
                        break;

                    // Seccion Economicos
                    case "VerInventario":
                        ContenedorPrincipal.Content = new EconomicosView();
                        textoPanel = "INVENTARIO DE MAQUINARIA";
                        break;
                    case "MoverEconomico":
                        ContenedorPrincipal.Content = new RealizarMovimientoView();
                        textoPanel = "REALIZAR UN MOVIMIENTO DE MAQUINARIA";
                        break;
                    case "AgregarEconomico":
                        ContenedorPrincipal.Content = new EconomicosAltaView();
                        textoPanel = "ALTA DE NUEVO ECONOMICO";
                        break;
                    case "AgregarArchivo":
                        ContenedorPrincipal.Content = new RegistrarArchivoView();
                        textoPanel = "ADJUNTAR DOCUMENTO O IMAGEN";
                        break;
                    case "VerServicios":
                        ContenedorPrincipal.Content = new HistorialServiciosView();
                        textoPanel = "HISTORIAL DE SERVICIOS";
                        break;
                    case "HisMoverEconomico":
                        ContenedorPrincipal.Content = new HistorialMovimientosView();
                        textoPanel = "HISTORIAL DE MOVIMIENTOS";
                        break;

                    // Seccion Personal
                    case "VerOperadores":
                        ContenedorPrincipal.Content = new OperadoresView();
                        textoPanel = "CATALOGO DE OPERADORES DE MAQUINARIA";
                        break;
                    case "VerEnMaq":
                        ContenedorPrincipal.Content = new EncargadosView();
                        textoPanel = "CATALOGO DE ENCARGADOS DE MAQUIINARIA";
                        break;
                    case "ProAdmin":
                        ContenedorPrincipal.Content = new ProAdminView();
                        textoPanel = "CATALOGO DE PROPIETARIOS Y ADMINISTRADORES DE MAQUINARIA";
                        break;

                    // Seccion Proveedores
                    case "VerBrokers":
                        ContenedorPrincipal.Content = new BrokersView();
                        textoPanel = "CATALOGO DE BROKERS";
                        break;

                    // Seccion Catalogos
                    case "CatCom":
                        ContenedorPrincipal.Content = new CombustiblesView();
                        textoPanel = "CATALOGO DE COMBUSTIBLES";
                        break;
                    case "CatTipEq":
                        ContenedorPrincipal.Content = new TiposEquipoView();
                        textoPanel = "CATALOGO DE TIPOS DE EQUIPOS";
                        break;
                    case "CatSer":
                        ContenedorPrincipal.Content = new ServiciosView();
                        textoPanel = "CATALOGO DE SERVICIOS";
                        break;
                    case "CatGrp":
                        ContenedorPrincipal.Content = new GruposView();
                        textoPanel = "CATALOGO DE GRUPOS";
                        break;
                    case "CatTiEs":
                        ContenedorPrincipal.Content = new EstatusView();
                        textoPanel = "CATALOGO DE ESTATUS";
                        break;
                    case "CatMarc":
                        ContenedorPrincipal.Content = new MarcasView();
                        textoPanel = "CATALOGO DE MARCAS";
                        break;

                    // Seccion Ubicacion Proyetos
                    case "VerUbicaciones":
                        ContenedorPrincipal.Content = new UbicacionProyectoView();
                        textoPanel = "CATALOGO DE UBICACIONES";
                        break;
                    case "VerTramos":
                        ContenedorPrincipal.Content = new TramosView();
                        textoPanel = "CATALOGO DE TRAMOS";
                        break;
                    case "VerFrentes":
                        ContenedorPrincipal.Content = new FrentesView();
                        textoPanel = "CATALOGO DE FRENTES";
                        break;

                    // Seccion Adquisiciones y servicios
                    case "AddReq":
                        ContenedorPrincipal.Content = new AgregarRequisicionView();
                        textoPanel = "AÑADIR NUEVA REQUISICION";
                        break;
                }
                txtPanel.Text = textoPanel;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}