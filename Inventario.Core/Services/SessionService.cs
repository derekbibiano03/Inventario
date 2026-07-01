using Inventario.Core.Services;

namespace InventarioMaquinaria.Services
{
  
    public class SessionService : ISessionService
    {
  
        public string Username { get; set; }

        public int IdRol { get; set; }

        public void CerrarSesion()
        {
            Username = string.Empty; 
            IdRol = 0;      
        }
    }
}