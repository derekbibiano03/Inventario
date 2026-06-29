using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace Inventario.Desktop.Helpers
{
    public class UpperCaseConverter : IValueConverter
    {
        public object Convert(object value, Type targeetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                return value.ToString().ToUpper();
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Al igual que al mostrar, si el usuario escribe algo, nos aseguramos de guardarlo en mayúsculas.
            if (value != null)
            {
                // Retorna el texto modificado en mayúsculas hacia tu propiedad de C#.
                return value.ToString().ToUpper();
            }
            // Si está vacío, retorna una cadena vacía.
            return string.Empty;
        }
    }
}
