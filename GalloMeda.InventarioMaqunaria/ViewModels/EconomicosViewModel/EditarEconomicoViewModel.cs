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
using Inventario.Core.Services.Logs; // OBLIGATORIO: Permite el acceso al servicio de registro de logs de auditoría

namespace Inventario.Desktop.ViewModels
{
    public class EditarEconomicoViewModel : INotifyPropertyChanged
    {
        // Define un delegado que se utiliza para cerrar la vista asociada y retornar el estado de éxito o cancelación
        public System.Action<bool> CloseAction { get; set; }

        // Colección observable que almacena los tipos de equipos disponibles para enlazar al ComboBox de la interfaz
        public ObservableCollection<CatalogoTiposEquipo> TipoEquipo { get; set; }

        // Colección observable que almacena los grupos de maquinaria disponibles para enlazar al ComboBox de la interfaz
        public ObservableCollection<CatalogoGrupo> Grupos { get; set; }

        // Colección observable que almacena las marcas disponibles para enlazar al ComboBox de la interfaz
        public ObservableCollection<CatalogoMarca> Marcas { get; set; }

        // Colección observable que almacena los estados lógicos de los equipos para enlazar al ComboBox de la interfaz
        public ObservableCollection<CatalogoEstatus> Estatus { get; set; }

        // Colección observable que almacena los tipos de combustible disponibles para enlazar al ComboBox de la interfaz
        public ObservableCollection<CatalogoTiposCombustible> Combustibles { get; set; }

        // Colección observable que almacena los propietarios y administradores para enlazar al ComboBox de la interfaz
        public ObservableCollection<CatalogoPya> PYA { get; set; }

        // Colección observable que almacena los operadores disponibles para enlazar al ComboBox de la interfaz
        public ObservableCollection<CatalogoOperadore> Operadores { get; set; }

        // Colección observable que almacena los ingenieros o encargados responsables para enlazar al ComboBox de la interfaz
        public ObservableCollection<CatalogoResponsableMaquinarium> Responsables { get; set; }

        // Colección observable que almacena los frentes de obra o proyectos para enlazar al ComboBox de la interfaz
        public ObservableCollection<CatalogoUbicacionesProyecto> Ubicaciones { get; set; }

        // Variable de almacenamiento interna para la entidad del económico que se está editando actualmente
        private CatalogoEconomico _economicoEdicion;

        // Propiedad pública que expone el económico en edición y notifica a la UI cuando es reemplazada por completo
        public CatalogoEconomico EconomicoEdicion
        {
            get => _economicoEdicion;
            set { _economicoEdicion = value; OnPropertyChanged(); }
        }

        // Comando público encargado de disparar la acción de guardado de modificaciones desde la interfaz de usuario
        public ICommand GuardarCambiosCommand { get; private set; }

        // Comando público encargado de disparar la acción de abortar la edición y cerrar el formulario
        public ICommand CancelarCommand { get; private set; }

        // Constructor principal de la clase que inicializa colecciones, comandos y catálogos de datos
        public EditarEconomicoViewModel()
        {
            // Inicializa la lista observable de tipos de equipo para evitar referencias nulas en el binding del XAML
            TipoEquipo = new ObservableCollection<CatalogoTiposEquipo>();

            // Inicializa la lista observable de grupos para evitar referencias nulas en el binding del XAML
            Grupos = new ObservableCollection<CatalogoGrupo>();

            // Inicializa la lista observable de marcas para evitar referencias nulas en el binding del XAML
            Marcas = new ObservableCollection<CatalogoMarca>();

            // Inicializa la lista observable de estatus para evitar referencias nulas en el binding del XAML
            Estatus = new ObservableCollection<CatalogoEstatus>();

            // Inicializa la lista observable de tipos de combustible para evitar referencias nulas en el binding del XAML
            Combustibles = new ObservableCollection<CatalogoTiposCombustible>();

            // Inicializa la lista observable de propietarios/administradores para evitar referencias nulas en el binding del XAML
            PYA = new ObservableCollection<CatalogoPya>();

            // Inicializa la lista observable de operadores técnicos para evitar referencias nulas en el binding del XAML
            Operadores = new ObservableCollection<CatalogoOperadore>();

            // Inicializa la lista observable de encargados de maquinaria para evitar referencias nulas en el binding del XAML
            Responsables = new ObservableCollection<CatalogoResponsableMaquinarium>();

            // Inicializa la lista observable de frentes de obra para evitar referencias nulas en el binding del XAML
            Ubicaciones = new ObservableCollection<CatalogoUbicacionesProyecto>();

            // Instancia el comando de guardado de cambios asociándolo al método que procesa la persistencia y los logs
            GuardarCambiosCommand = new RelayCommand(EjecutarGuardarCambios);

            // Instancia el comando de cancelación de cambios asociándolo al método que aborta el flujo de pantalla
            CancelarCommand = new RelayCommand(EjecutarCancelar);

            // Carga los catálogos de base de datos para popular todos los ComboBoxes de la pantalla de edición
            CargarCatalogos();
        }

        // Recupera de manera síncrona todos los catálogos necesarios para llenar la información de selección
        private void CargarCatalogos()
        {
            // Abre una conexión transitoria a PostgreSQL mediante el DbContext de Entity Framework
            using (var contexto = new InventarioContext())
            {
                // Consulta todos los tipos de equipos registrados en la base de datos y los almacena en una variable local
                var tiposDb = contexto.CatalogoTiposEquipos.ToList();
                // Recorre y agrega cada uno de los tipos recuperados a la colección enlazada a la UI
                foreach (var item in tiposDb) TipoEquipo.Add(item);

                // Consulta todos los grupos de maquinaria registrados en la base de datos y los almacena en una lista
                var gruposDb = contexto.CatalogoGrupos.ToList();
                // Recorre y agrega cada uno de los grupos recuperados a la colección enlazada a la UI
                foreach (var item in gruposDb) Grupos.Add(item);

                // Consulta todas las marcas registradas en la base de datos y las almacena en una lista
                var marcasDb = contexto.CatalogoMarcas.ToList();
                // Recorre y agrega cada una de las marcas recuperadas a la colección enlazada a la UI
                foreach (var item in marcasDb) Marcas.Add(item);

                // Consulta todos los estados lógicos posibles para los activos y los almacena en una lista
                var estatusDb = contexto.CatalogoEstatuses.ToList();
                // Recorre y agrega cada uno de los estados recuperados a la colección enlazada a la UI
                foreach (var item in estatusDb) Estatus.Add(item);

                // Consulta todos los tipos de combustible utilizables en el sistema y los almacena en una lista
                var combustiblesDb = contexto.CatalogoTiposCombustibles.ToList();
                // Recorre y agrega cada uno de los combustibles recuperados a la colección enlazada a la UI
                foreach (var item in combustiblesDb) Combustibles.Add(item);

                // Consulta todos los propietarios y administradores corporativos y los almacena en una lista
                var pyaDb = contexto.CatalogoPyas.ToList();
                // Recorre y agrega cada uno de los registros recuperados a la colección enlazada a la UI
                foreach (var item in pyaDb) PYA.Add(item);

                // Consulta todos los operadores asignables de la plantilla técnica y los almacena en una lista
                var operadoresDb = contexto.CatalogoOperadores.ToList();
                // Recorre y agrega cada uno de los operadores recuperados a la colección enlazada a la UI
                foreach (var item in operadoresDb) Operadores.Add(item);

                // Consulta todos los ingenieros encargados de maquinaria activos y los almacena en una lista
                var responsablesDb = contexto.CatalogoResponsableMaquinaria.ToList();
                // Recorre y agrega cada uno de los encargados recuperados a la colección enlazada a la UI
                foreach (var item in responsablesDb) Responsables.Add(item);

                // Consulta todas las ubicaciones y proyectos geográficos activos y los almacena en una lista
                var ubicacionesDb = contexto.CatalogoUbicacionesProyectos.ToList();
                // Recorre y agrega cada una de las ubicaciones recuperadas a la colección enlazada a la UI
                foreach (var item in ubicacionesDb) Ubicaciones.Add(item);
            }
        }

        // Descarga el registro específico del económico seleccionado utilizando su identificador único
        public void CargarDatosEconomico(string idEconomico)
        {
            // Abre una conexión local hacia el motor PostgreSQL para efectuar la búsqueda
            using (var contexto = new InventarioContext())
            {
                // Busca el primer registro que coincida exactamente con el identificador del económico solicitado
                var economico = contexto.CatalogoEconomicos.FirstOrDefault(e => e.IdEconomico == idEconomico);

                // Valida si el objeto recuperado existe en la base de datos
                if (economico != null)
                {
                    // Asigna el objeto de base de datos a la propiedad enlazada para reflejar la información en los controles del formulario
                    EconomicoEdicion = economico;
                }
            }
        }

        // Ejecuta la lógica correspondiente al botón "Guardar Cambios" aplicando la actualización y registrando el log de auditoría
        private void EjecutarGuardarCambios()
        {
            // Detiene de forma preventiva la ejecución si no hay ninguna entidad cargada en el objeto de edición
            if (EconomicoEdicion == null) return;

            try
            {
                // Inicializa el contexto de base de datos que se usará tanto para actualizar el registro como para insertar el log
                using (var contexto = new InventarioContext())
                {
                    // Instancia el servicio de logs asociándolo al contexto actual para garantizar que se ejecute en la misma transacción física
                    var logsService = new LogsService(contexto);

                    // Adjunta y marca el objeto 'EconomicoEdicion' como modificado en el rastreador de cambios del DbContext
                    contexto.Entry(EconomicoEdicion).State = EntityState.Modified;

                    // Guarda los cambios aplicados en el equipo dentro de PostgreSQL de forma persistente
                    contexto.SaveChanges();

                    // CORRECCIÓN: Registra el evento de actualización de manera exitosa en la bitácora utilizando el ID del usuario en sesión actual
                    logsService.RegistrarModificacionEquipo(GalloMeda.InventarioMaqunaria.App.Session.IdUsuario,EconomicoEdicion.IdEconomico);
                }

                // Cierra la ventana actual y retorna una respuesta afirmativa (true) para refrescar el listado principal de la aplicación
                CloseAction?.Invoke(true);
            }
            catch (Exception ex)
            {
                // Despliega un cuadro de diálogo con el mensaje técnico del error en caso de fallo en el proceso de actualización o guardado
                System.Windows.MessageBox.Show($"Ocurrió un error al guardar los cambios: {ex.Message}", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        // Cierra el formulario restableciendo el flujo sin guardar ningún cambio en la base de datos
        private void EjecutarCancelar()
        {
            // Cierra la ventana regresando una respuesta negativa (false) para indicar que la edición fue abortada por el usuario
            CloseAction?.Invoke(false);
        }

        // Evento requerido por la interfaz INotifyPropertyChanged para permitir el enlace de datos bidireccional en la UI
        public event PropertyChangedEventHandler PropertyChanged;

        // Dispara la notificación de actualización de propiedades para repintar los elementos enlazados de la interfaz
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}