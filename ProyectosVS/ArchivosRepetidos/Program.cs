// See https://aka.ms/new-console-template for more information
using System.Collections.Generic;
using System.Runtime.InteropServices;

Console.WriteLine("Ejecutando búsqueda de archivos repetidos");

bool ejecutarBusqueda = true;
bool ejecutarCopia = false ;

var repeatFilesManager = new RepeatFilesManager();

string directorioBase = @"\\MYCLOUDEX2ULTRA\Public\Fotos&Videos"; // Directorio base
var carpetasIncluidas = new HashSet<string> { @"\\MYCLOUDEX2ULTRA\Copias seguridad\Xiaomi Redmi Note 8 Pro Camera Backup" }; // Carpetas fuera del directorio base a incluir
var carpetasExcluidas = new HashSet<string> { @"\\MYCLOUDEX2ULTRA\Public\Fotos&Videos\_gsdata_" }; // Carpetas a excluir del directorio base
string rutaArchivoCarpetas = @"E:\\ComprobarRepetidos\rutas_a_comprobar.txt";
string carpetaRepository = @"E:\\ComprobarRepetidos\\Repository";

if (ejecutarBusqueda)
{
    var carpetasDesdeArchivo = repeatFilesManager.CargarCarpetasDesdeArchivo(rutaArchivoCarpetas); // Opcional: Cargar desde un archivo
                                                                                                   //directorioBase = string.Empty;
    carpetasIncluidas = new HashSet<string>();
    var duplicados = repeatFilesManager.EncontrarArchivosDuplicados(directorioBase,
                                                                    carpetasIncluidas,
                                                                    carpetasExcluidas,
                                                                    carpetasDesdeArchivo,
                                                                    rutaArchivoCarpetas);

    Console.WriteLine("Proceso completado. Revisa los archivos de salida.");
    Console.WriteLine("Archivos duplicados encontrados:");

    foreach (var grupo in duplicados)
    {
        Console.WriteLine("Grupo de archivos duplicados:");
        foreach (var archivo in grupo)
        {
            Console.WriteLine(archivo);
        }
        Console.WriteLine();
    }
}


if (ejecutarCopia)
{
    Console.WriteLine("Copiando archivos");

    repeatFilesManager.CopiarArchivosManteniendoEstructura(carpetaRepository, directorioBase);

    Console.WriteLine("Proceso completado.");
}




Console.ReadLine();
