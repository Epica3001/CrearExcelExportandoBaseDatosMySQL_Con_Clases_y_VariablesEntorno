using System;
using System.Data;
using ClosedXML.Excel;

namespace ExportarDatosAExcelClasesUserPassworMySQL
{
    public class ExcelExporter
    {
        public void ExportToExcel(DataTable dataTable, string filePath, string table)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add(table);
                //var worksheet = workbook.Worksheets.Add("Producto");
                worksheet.Cell(1, 1).InsertTable(dataTable);
                workbook.SaveAs(filePath);
            }
        }
    }
}
