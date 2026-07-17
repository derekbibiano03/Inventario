using Inventario.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using BCryptNet = BCrypt.Net.BCrypt;
using System.Linq;

namespace Inventario.Core.Services.Auth
{
    public class AutenticacionService
    {
        private readonly InventarioContext _context;

        public AutenticacionService(InventarioContext context)
        {
            _context = context;
        }

        public Usuario? ValidarCredenciales(string nombreUsuario, string contrasenaPlana)
        {
            if (string.IsNullOrWhiteSpace(nombreUsuario) || string.IsNullOrWhiteSpace(contrasenaPlana))
            {
                return null;
            }

            // Busca al usuario en PostgreSQL por su nombre único
            var usuarioDb = _context.Usuarios.FirstOrDefault(u => u.NombreUsuario == nombreUsuario.Trim());

            if (usuarioDb == null)
            {
                return null; // El usuario no existe
            }

            try
            {
                // Verifica si la contraseña en texto plano coincide con el hash guardado
                if (BCryptNet.Verify(contrasenaPlana, usuarioDb.Password))
                {
                    return usuarioDb; // Retorna el objeto completo con su IdUsuario real
                }

                return null; // Contraseña incorrecta
            }
            catch (Exception)
            {
                return null;
            }
        }


        public bool RegistrarUsuario(string nombreUsuario, string contrasenaPlana, int idRol)
        {
            // Genera un hash irreversible agregando un salt aleatorio automáticamente
            string contrasenaHasheada = BCrypt.Net.BCrypt.HashPassword(contrasenaPlana);

            var nuevoUsuario = new Usuario
            {
                NombreUsuario = nombreUsuario,
                Password = contrasenaHasheada,
                IdRol = idRol
            };

            _context.Usuarios.Add(nuevoUsuario);
            _context.SaveChanges();
            return true;
        }

        public List<UsuariosRole> ObtenerRoles()
        {
            // Retorna la lista de roles directamente desde la base de datos de PostgreSQL
            var resultado = _context.UsuariosRoles
                .Select(e => new UsuariosRole
                {
                    IdRol = e.IdRol,
                    DescripcionRol = e.DescripcionRol
                })
                .ToList();
            return resultado;
        }

    }
}
