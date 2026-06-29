using Inventario.Desktop.Views.UserControllers;
using Inventario.Desktop.Views.UserControllers.Economicos;
using Inventario.Desktop.Views.UserControllers.Personal;
using Inventario.Desktop.Views.UserControllers.Catalogos;
using Inventario.Desktop.Views.UserControllers.Proveedores;
using Inventario.Desktop.Views.UserControllers.UbicacionProyectos;
using System.Windows;
using System.Windows.Controls;
using Inventario.Desktop.Views.UserControllers.Auth;

namespace Inventario.Desktop.Views
{
    public partial class MainWindow : Window
    {
        
        public MainWindow()
        {
            InitializeComponent();
            string textoPanel = "Panel de Control de Maquinaria";
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
            if (sender is Button boton)
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
                string ID_Interno = itemPresionado.Tag.ToString();

                switch (ID_Interno)
                {

                    case "AddUser":
                        ContenedorPrincipal.Content = new AgregarUsuario();
                        textoPanel = "Agregar un nuevo usuario";
                        break;

                    //Seccion Economicos

                    case "VerInventario":
                        ContenedorPrincipal.Content = new EconomicosView();
                        textoPanel = "Inventario Maquinaria";
                        break;
                    case "AgregarEconomico":
                        ContenedorPrincipal.Content = new EconomicosAltaView();
                        textoPanel = "Alta de nuevo Economico";
                        break;
                    case "AgregarArchivo":
                        ContenedorPrincipal.Content = new RegistrarArchivoView();
                        textoPanel = "Adjuntar Documento o Imagen";
                        break;
                    case "VerServicios":
                        ContenedorPrincipal.Content = new HistorialServiciosView();
                        textoPanel = "Historial de Servicios";
                        break;

                    //Seccion Personal

                    case "VerOperadores":
                        ContenedorPrincipal.Content = new OperadoresView();
                        textoPanel = "Catalogo de Operadores";
                        break;
                    case "VerEnMaq":
                        ContenedorPrincipal.Content = new EncargadosView();
                        textoPanel = "Catalogo Encargados de Maquinaria";
                        break;
                    case "ProAdmin":
                        ContenedorPrincipal.Content = new ProAdminView();
                        textoPanel = "Catalogo de Propietarios y Administradores";
                        break;

                    //Seccion Proveedores

                    case "VerBrokers":
                        ContenedorPrincipal.Content = new BrokersView();
                        textoPanel = "Catalogo de Brokers";
                        break;

                    // Seccion Catalogos

                    case "CatCom":
                        ContenedorPrincipal.Content = new CombustiblesView();
                        textoPanel = "Catalogo de Combustibles";
                        break;
                    case "CatTipEq":
                        ContenedorPrincipal.Content = new TiposEquipoView();
                        textoPanel = "Catalogo de Tipos de Equipos";
                        break;
                    case "CatSer":
                        ContenedorPrincipal.Content = new ServiciosView();
                        textoPanel = "Catalogo de Servicios";
                        break;
                    case "CatGrp":
                        ContenedorPrincipal.Content = new GruposView();
                        textoPanel = "Catalogo de Grupos";
                        break;
                    case "CatTiEs":
                        ContenedorPrincipal.Content = new EstatusView();
                        textoPanel = "Catalogo de Estatus";
                        break;
                    case "CatMarc":
                        ContenedorPrincipal.Content = new MarcasView();
                        textoPanel = "Catalogo de Marcas";
                        break;


                    // Seccion Ubicacion Proyetos

                    case "VerUbicaciones":
                        ContenedorPrincipal.Content = new UbicacionProyectoView();
                        textoPanel = "Catalogo de Ubicaciones";
                        break;
                    case "VerTramos":
                        ContenedorPrincipal.Content = new TramosView();
                        textoPanel = "Catalogo de Tramos";
                        break;
                    case "VerFrentes":
                        ContenedorPrincipal.Content = new FrentesView();
                        textoPanel = "Catalogo de Frentes";
                        break;

                    


                }
                txtPanel.Text = textoPanel;
            }
        }

    }
}