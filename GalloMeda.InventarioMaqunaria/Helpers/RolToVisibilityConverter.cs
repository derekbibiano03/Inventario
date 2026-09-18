using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Inventario.Desktop.Helpers
{
    public class RolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // value = ID del rol del usuario (int)
            // parameter = ID(es) de rol permitido(s) separados por guion (ej. "1-3")

            if (value is int idRolUsuario && parameter is string rolesPermitidosStr)
            {
                var listaRoles = rolesPermitidosStr.Split('-'); // Cambiado de ',' a '-'
                foreach (var rolStr in listaRoles)
                {
                    if (int.TryParse(rolStr.Trim(), out int idPermitido) && idPermitido == idRolUsuario)
                    {
                        return Visibility.Visible;
                    }
                }
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}