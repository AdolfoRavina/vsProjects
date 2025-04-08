// See https://aka.ms/new-console-template for more information

Console.WriteLine("Ejecutando búsqueda de archivos repetidos");

string directorioBase = @"\\MYCLOUDEX2ULTRA\Public\Fotos&Videos"; // Directorio base
var carpetasIncluidas = new HashSet<string> { @"\\MYCLOUDEX2ULTRA\Copias seguridad\Xiaomi Redmi Note 8 Pro Camera Backup",
                                              @"\\MYCLOUDEX2ULTRA\Silvia",
                                              @"\\MYCLOUDEX2ULTRA\abril\Movil"}; // Carpetas fuera del directorio base a incluir

var carpetasExcluidas = new HashSet<string> { @"\\MYCLOUDEX2ULTRA\Public\Fotos&Videos\_gsdata_" }; // Carpetas a excluir del directorio base


string rutaArchivoCarpetas = @"E:\\ComprobarRepetidos\rutas_a_comprobar.txt";

string carpetaRepository = @"E:\\ComprobarRepetidos\\Repository";
string carpetaDelete = @"E:\\ComprobarRepetidos\\Delete";


// Configurar la solicitud
var request = new EncontrarArchivosDuplicadosRequest
{
    EjecutarBusqueda = true,
    EjecutarCopia = false,
    EjecutarBorrarVacías = false,
    MoverArchivos = true,
    BorrarArchivosConExtensionesDeProyectos = false // Establecer en true para borrar archivos con extensiones específicas
};

var repeatFilesManager = new RepeatFilesManager(request);
string archivoSalida = repeatFilesManager.GetArchivoDeSalida(rutaArchivoCarpetas);


// Llamar a EliminarArchivosConExtensiones si la propiedad está establecida en true
if (request.BorrarArchivosConExtensionesDeProyectos)
{
    var carpetasAProcesar = repeatFilesManager.CarpetasAProcesar(directorioBase, carpetasIncluidas);
    repeatFilesManager.EliminarArchivosConExtensiones(directorioBase, carpetasAProcesar, carpetasExcluidas);
}

if (request.EjecutarBusqueda)
{
    carpetasIncluidas = [];
    var carpetasAProcesar = repeatFilesManager.CarpetasAProcesar(directorioBase, carpetasIncluidas);
    //carpetasAProcesar = new HashSet<string> { @"\\MYCLOUDEX2ULTRA\Public\Fotos&Videos\2012\Varios" ,
    //@"\\MYCLOUDEX2ULTRA\Public\Fotos&Videos\2016\08 Agosto\Madrid"};

    var duplicados = repeatFilesManager.EncontrarArchivosDuplicados(directorioBase, carpetasAProcesar, carpetasExcluidas,carpetaRepository,carpetaDelete,rutaArchivoCarpetas,archivoSalida);

    Console.WriteLine("Proceso completado. Revisa los archivos de salida.");
    Console.WriteLine("Archivos duplicados encontrados:");

    foreach (var grupo in duplicados)
    {
        Console.WriteLine("Grupo de archivos duplicados:");
        foreach (var archivo in grupo)
        {
            Console.WriteLine(archivo);
        }
        Console.WriteLine("Pulsa una tecla");
        Console.WriteLine();
    }
}


if (request.EjecutarCopia)
{
    Console.WriteLine("Copiando archivos");

    repeatFilesManager.CopiarArchivosManteniendoEstructura(carpetaRepository, directorioBase);

    Console.WriteLine("Proceso completado.");
}


if (request.EjecutarBorrarVacías)
{
    Console.WriteLine("Borrando carpetas vacías");
    var carpetas = repeatFilesManager.CarpetasAProcesar(directorioBase, carpetasIncluidas);

    repeatFilesManager.DeleteEmptyFolders(repeatFilesManager.CarpetasAProcesar(directorioBase, carpetasIncluidas) , carpetasExcluidas, archivoSalida);
    Console.WriteLine("Proceso completado.");
}

Console.ReadLine();
