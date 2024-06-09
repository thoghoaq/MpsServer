using Mps.Application.Abstractions.Excel;
using OfficeOpenXml;
using System.Reflection;

namespace Mps.Infrastructure.Dependencies.Excel
{
    public class ExcelService : IExcelService
    {
        public MemoryStream ExportToExcel<T>(List<T> data)
        {
            var stream = new MemoryStream();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");

                var properties = typeof(T).GetProperties();
                for (int i = 0; i < properties.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = properties[i].Name;
                    worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                }

                for (int row = 0; row < data.Count; row++)
                {
                    var item = data[row];
                    for (int col = 0; col < properties.Length; col++)
                    {
                        var value = properties[col].GetValue(item);
                        worksheet.Cells[row + 2, col + 1].Value = value;
                    }
                }

                package.SaveAs(stream);
            }

            stream.Position = 0; // Reset the stream position to the beginning
            return stream;
        }

        public List<T> ReadExcelFile<T>(Stream excelStream) where T : new ()
        {
            var result = new List<T>();

            // Ensure the EPPlus license is accepted
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage(excelStream))
            {
                var worksheet = package.Workbook.Worksheets[0];
                int rowCount = worksheet.Dimension.Rows;
                int colCount = worksheet.Dimension.Columns;

                // Get the header row values to map to properties
                var headers = new List<string>();
                for (int col = 1; col <= colCount; col++)
                {
                    headers.Add(worksheet.Cells[1, col].Text);
                }

                // Iterate through rows
                for (int row = 2; row <= rowCount; row++)
                {
                    var item = new T();
                    var itemType = typeof(T);

                    for (int col = 1; col <= colCount; col++)
                    {
                        var propertyName = headers[col - 1];
                        var property = itemType.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                        if (property != null && property.CanWrite)
                        {
                            var cellValue = worksheet.Cells[row, col].Text;
                            var convertedValue = Convert.ChangeType(cellValue, property.PropertyType);
                            property.SetValue(item, convertedValue);
                        }
                    }

                    result.Add(item);
                }
            }

            return result;
        }
    }
}
