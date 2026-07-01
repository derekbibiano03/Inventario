using System;
using System.Collections.Generic;
using System.Text;

namespace Inventario.Core.Services
{
    public interface ISessionService
    {
        string Username { get; set; }
        int IdRol{ get; set; }
        void CerrarSesion();
    }
}
