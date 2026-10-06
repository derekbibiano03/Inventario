using Inventario.Data.Models; // Asegúrate de apuntar al namespace correcto de tus modelos
using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;

namespace Inventario.Core
{
    public class ExcelExportService
    {

        public byte[] GenerarExcelHistorialServicios(List<HistorialServicio> listaHistorial)
        {
            ExcelPackage.License.SetNonCommercialOrganization("GalloMeda");

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Historial de Servicios");

                // Cabeceras omitiendo los documentos adjuntos
                string[] cabeceras = {
            "ID SERVICIO", "ECONOMICO", "FECHA DE MANTENIMIENTO",
            "TIPO DE MANTENIMIENTO", "ANOTACIONES", "ODOMETRO REAL"
        };

                for (int i = 0; i < cabeceras.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = cabeceras[i];
                    worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                }

                int fila = 2;

                foreach (var item in listaHistorial)
                {
                    worksheet.Cells[fila, 1].Value = item.IdServicio;
                    worksheet.Cells[fila, 2].Value = item.NoEconomico;

                    // Formatear la fecha correctamente para Excel
                    if (item.FechaMantenimiento != default)
                    {
                        worksheet.Cells[fila, 3].Value = item.FechaMantenimiento.ToDateTime(TimeOnly.MinValue);
                        worksheet.Cells[fila, 3].Style.Numberformat.Format = "dd/MM/yyyy";
                    }
                    else
                    {
                        worksheet.Cells[fila, 3].Value = "N/A";
                    }

                    worksheet.Cells[fila, 4].Value = item.TipoMantenimiento ?? "N/A";
                    worksheet.Cells[fila, 5].Value = item.Anotaciones ?? "N/A";
                    worksheet.Cells[fila, 6].Value = item.Horaskilometrosreales ?? "N/A";

                    fila++;
                }

                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                return package.GetAsByteArray();
            }
        }


        public byte[] GenerarExcelEconomicos(List<CatalogoEconomico> listaFiltrada)
        {
            ExcelPackage.License.SetNonCommercialOrganization("GalloMeda");

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Inventario Completo");

                // Cabeceras con los nombres reales y lógicos de los datos
                string[] cabeceras = {
                    "Económico ID", "Descripción", "Marca",
                    "Modelo", "Serie", "Periodo de Fabricacion", "Informacion de Motor",
                    "Marca de motor", "Modelo del Motor", "No. Serie del motor",
                    "Familia del motor", "Horometro", "Grado de Propiedad",
                    "Observaciones", "Estatus Seguro", "Tipo de Equipo", "Grupo", "Ubicación",
                    "Combustible", "Propietario", "Administrador", "Estatus",
                    "Operador", "Responsable", "Horómetro", "Placas"
                };

                for (int i = 0; i < cabeceras.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = cabeceras[i];
                    worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                }

                int fila = 2;

                foreach (var item in listaFiltrada)
                {
                    // Propiedades directas e indirectas resolviendo las relaciones nulas con el operador '?.'
                    worksheet.Cells[fila, 1].Value = item.IdEconomico.ToUpper();
                    worksheet.Cells[fila, 2].Value = item.Descripcion;
                    worksheet.Cells[fila, 3].Value = item.IdMarcaNavigation?.NombreMarca ?? "SIN INFORMACION";
                    worksheet.Cells[fila, 4].Value = item.Modelo;
                    worksheet.Cells[fila, 5].Value = item.Serie;
                    worksheet.Cells[fila, 6].Value = item.PeriodoFabricacion;
                    worksheet.Cells[fila, 7].Value = item.Motor;
                    worksheet.Cells[fila, 8].Value = item.MarcaMotorNavigation?.NombreMarca ?? "SNI IDENTIFICAR";
                    worksheet.Cells[fila, 9].Value = item.ModeloMotor;
                    worksheet.Cells[fila, 10].Value = item.SerieMotor;
                    worksheet.Cells[fila, 11].Value = item.FamiliaMotor;
                    worksheet.Cells[fila, 12].Value = item.Horometro;
                    worksheet.Cells[fila, 13].Value = item.GradoPropiedad;
                    worksheet.Cells[fila, 14].Value = item.ObservacionesAsignaciones;
                    worksheet.Cells[fila, 15].Value = item.EstatusSeguro;
                    worksheet.Cells[fila, 16].Value = item.IdTipoEquipoNavigation?.DescripcionTipoEquipo ?? "N/A";
                    worksheet.Cells[fila, 17].Value = item.IdGrupoNavigation?.DescripcionGrupo ?? "N/A";
                    worksheet.Cells[fila, 18].Value = item.IdUbicacionNavigation?.NombreProyecto ?? "N/A";
                    worksheet.Cells[fila, 19].Value = item.IdCombustibleNavigation?.DescripcionCombustible ?? "N/A";
                    worksheet.Cells[fila, 20].Value = item.IdPropietarioNavigation?.Nombre ?? "N/A";
                    worksheet.Cells[fila, 21].Value = item.IdAdministradorNavigation?.Nombre ?? "N/A";
                    worksheet.Cells[fila, 22].Value = item.IdEstatusNavigation?.DescripcionEstatus ?? "N/A";
                    worksheet.Cells[fila, 23].Value = item.IdOperadorNavigation?.NombreEmpleado ?? "N/A";
                    worksheet.Cells[fila, 24].Value = item.IdResponsableNavigation?.NombreEmpleado ?? "N/A";
                    worksheet.Cells[fila, 25].Value = item.Horometro;
                    worksheet.Cells[fila, 26].Value = item.Placas;


                    fila++;
                }

                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                return package.GetAsByteArray();
            }
        }
    }
}