using System;
using System.Data;
using Spectre.Console;
using DotNetEnv;
using System.IO;

namespace ExportarDatosAExcelClasesUserPassworMySQL
{
    class Program
    {
        static void Main(string[] args)
        {
            // CONFIGURACIÓN DE VARIABLE DE ENTORNO
            string envFilePath = Path.Combine(Directory.GetCurrentDirectory(), ".env");
            Env.Load(envFilePath);

            while (true) // Bucle para reiniciar el programa en caso de error
            {
                try
                {
                    // LEER VARIABLE DE ENTORNO
                    string? connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");

                    // IMPRIMIR ESTADO DE LA VARIABLE DE ENTORNO
                    Console.WriteLine($"Valor Inicial de DB_CONNECTION_STRING: {connectionString ?? "NULL"}");

                    // VERIFICAR SI ES NECESARIO PEDIR LOS DATOS DE CONEXIÓN
                    if (string.IsNullOrWhiteSpace(connectionString))
                    {
                        var dbConnection = new DatabaseConnection();
                        connectionString = dbConnection.GetConnectionString() ?? throw new InvalidOperationException("No se pudo obtener una cadena de conexión válida.");
                    }

                    // SOLICITAR TABLA A EXPORTAR
                    Console.Write("Ingrese Nombre de Tabla a Exportar: ");
                    string? table = Console.ReadLine();

                    if (string.IsNullOrWhiteSpace(table))
                    {
                        throw new ArgumentException("El nombre de la tabla no puede estar vacío.");
                    }

                    // CONSTRUIR CONSULTA Y ARCHIVO DE SALIDA
                    string query = $"SELECT * FROM `{table}`"; // Usar backticks para proteger nombres de tablas
                    string filePath = $"{table}_Export.xlsx";

                    // CREAR INSTANCIAS DE LAS CLASES NECESARIAS
                    var dbHelper = new DatabaseHelper(connectionString);
                    var excelExporter = new ExcelExporter();
                    var dataTable = dbHelper.ExecuteQuery(query);

                    // VALIDAR RESULTADOS DE LA CONSULTA
                    if (dataTable.Rows.Count > 0)
                    {
                        excelExporter.ExportToExcel(dataTable, filePath, table);
                        AnsiConsole.MarkupLine($"[bold white on blue]Datos exportados exitosamente a {filePath}[/]");
                    }
                    else
                    {
                        AnsiConsole.MarkupLine("[bold yellow on red]No se encontraron datos para exportar.[/]");
                    }

                    break; // SALIR DEL BUCLE SI TODO VA BIEN
                }
                catch (Exception ex)
                {
                    AnsiConsole.MarkupLine($"[bold yellow on red]Error: {ex.Message}[/]");
                    Console.WriteLine("Presiona Enter para intentarlo de nuevo...");
                    Console.ReadLine();
                }
                finally
                {
                    // LIMPIEZAS O LOGS FINALES
                    Console.WriteLine("Finalizando intento de exportación...");
                }
            }
        }
    }
}
    