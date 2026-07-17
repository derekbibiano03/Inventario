using System;
using System.Collections.Generic;

namespace Inventario.Data.Models;

// Define la estructura lógica de la entidad Usuario que representa la tabla 'usuarios'
public partial class Usuario
{
    // Identificador único y llave primaria de la tabla usuarios
    public int IdUsuario { get; set; }

    // Almacena el nombre de inicio de sesión elegido por el usuario en el sistema
    public string NombreUsuario { get; set; } = string.Empty;

    // Almacena la contraseña (idealmente encriptada) para validar el acceso
    public string Password { get; set; } = string.Empty;

    // Identificador de la llave foránea que conecta al usuario con su rol asignado
    public int IdRol { get; set; }

    // Propiedad de navegación que permite acceder al objeto completo del rol que posee este usuario
    public virtual UsuariosRole? IdRolNavigation { get; set; }

    // Colección de registros de la tabla 'usurios_roles' vinculados de forma directa a este usuario
    public virtual ICollection<UsuriosRole> UsuriosRoles { get; set; } = new List<UsuriosRole>();

    // Colección de registros históricos de logs generados de forma automática por las acciones de este usuario
    public virtual ICollection<HistorialLogs> HistorialLogs { get; set; } = new List<HistorialLogs>();
}