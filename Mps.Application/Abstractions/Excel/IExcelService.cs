namespace Mps.Application.Abstractions.Excel
{
    public interface IExcelService
    {
       List<T> ReadExcelFile<T>(Stream excelStream) where T : new();
       MemoryStream ExportToExcel<T>(List<T> data);
    }
}
