using Inventario.Core.Services;

namespace InventarioMaquinaria.Services
{
  
    public class SessionService : ISessionService
    {
        public string Username { get; set; } = string.Empty;
        public int IdRol { get; set; }
        public int IdUsuario { get; set; }

        public void CerrarSesion()
        {
            Username = string.Empty; 
            IdRol = 0;      
        }
    }
}