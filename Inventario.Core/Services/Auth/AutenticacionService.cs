using Inventario.Core.Services.Logs;
using Inventario.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using BCryptNet = BCrypt.Net.BCrypt;

namespace Inventario.Core.Services.Auth
{
    public class AutenticacionService
    {
        private readonly InventarioContext _context;
        private readonly LogsService _logsService;

        public AutenticacionService(InventarioContext context, LogsService logsService)
        {
            _context = context;
            _logsService = logsService;
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


        public bool RegistrarUsuario(int idUsuarioOperativo, string nombreUsuario, string contrasenaPlana, int idRol)
        {
            string contrasenaHasheada = BCrypt.Net.BCrypt.HashPassword(contrasenaPlana);

            var nuevoUsuario = new Usuario
            {
                NombreUsuario = nombreUsuario,
                Password = contrasenaHasheada,
                IdRol = idRol
            };

            _context.Usuarios.Add(nuevoUsuario);
            _context.SaveChanges();

            string idGenerado = nuevoUsuario.IdUsuario.ToString();

            _logsService.RegistrarAltaNuevoUsuarioExitoso(idUsuarioOperativo, idGenerado);
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
