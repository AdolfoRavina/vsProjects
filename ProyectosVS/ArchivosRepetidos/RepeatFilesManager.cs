using System;
using System.Security.Cryptography;
using System.Drawing;

public class EncontrarArchivosDuplicadosRequest
{
    public bool EjecutarBusqueda { set; get; } = false;
    public bool EjecutarCopia { set; get; } = false;
    public bool EjecutarBorrarVacías { set; get; } = false;
    public bool MoverArchivos { set; get; } = false;
    public bool BorrarArchivosConExtensionesDeProyectos { set; get; } = false;
       

}

public class RepeatFilesManager
{

    private readonly EncontrarArchivosDuplicadosRequest _request;
    public RepeatFilesManager(EncontrarArchivosDuplicadosRequest request)
	{
        _request = request;
    }


    public HashSet<string> CarpetasAProcesar(string directorioBase, HashSet<string> carpetasIncluidas)
    {
        var carpetasAProcesar = new HashSet<string> { directorioBase };
        carpetasAProcesar.UnionWith(carpetasIncluidas);
        return carpetasAProcesar;
    }

    //public List<List<string>> EncontrarArchivosDuplicados(string directorioBase,
    //                                                  HashSet<string> carpetasAProcesar,
    //                                                  HashSet<string> carpetasExcluidas,
    //                                                  string carpetaRepository,
    //                                                  string carpetaDelete,
    //                                                  string rutaArchivoCarpetas,
    //                                                  string archivoSalida)
    //{
    //    var archivosPorTamano = new Dictionary<long, List<string>>();
    //    var duplicados = new List<List<string>>();
    //    var archivosProcesados = new List<string>();
    //    var resultadosDuplicados = new List<string>();

    //    Directory.CreateDirectory(carpetaRepository);
    //    Directory.CreateDirectory(carpetaDelete);

    //    try
    //    {
    //        foreach (var carpeta in carpetasAProcesar.Where(x => !string.IsNullOrEmpty(x)))
    //        {
    //            foreach (var archivo in Directory.EnumerateFiles(carpeta, "*", SearchOption.AllDirectories))
    //            {
    //                if (carpetasExcluidas.Any(excluir => archivo.StartsWith(excluir, StringComparison.OrdinalIgnoreCase)))
    //                    continue;

    //                long tamaño = new FileInfo(archivo).Length;
    //                if (!archivosPorTamano.ContainsKey(tamaño))
    //                    archivosPorTamano[tamaño] = new List<string>();
    //                archivosPorTamano[tamaño].Add(archivo);
    //            }
    //        }

    //        foreach (var grupo in archivosPorTamano.Values.Where(g => g.Count > 1))
    //        {
    //            var archivosPorContenido = new Dictionary<string, List<string>>();
    //            foreach (var archivo in grupo)
    //            {
    //                try
    //                {
    //                    string contenido = ObtenerContenidoArchivo(archivo);
    //                    if (!archivosPorContenido.ContainsKey(contenido))
    //                        archivosPorContenido[contenido] = new List<string>();
    //                    archivosPorContenido[contenido].Add(archivo);
    //                }
    //                catch (Exception ex)
    //                {
    //                    Console.WriteLine($"Error al procesar {archivo}: {ex.Message}");
    //                }
    //            }

    //            foreach (var subgrupo in archivosPorContenido.Values.Where(g => g.Count > 1))
    //            {
    //                duplicados.Add(subgrupo);
    //                resultadosDuplicados.Add("Grupo de archivos duplicados:");
    //                resultadosDuplicados.AddRange(subgrupo);
    //                resultadosDuplicados.Add("");

    //                if (!_request.MoverArchivos)
    //                    continue;
    //                string archivoPrincipal = subgrupo.First();
    //                string rutaRelativa = Path.GetRelativePath(directorioBase, archivoPrincipal);
    //                string destinoRepository = Path.Combine(carpetaRepository, rutaRelativa);
    //                Directory.CreateDirectory(Path.GetDirectoryName(destinoRepository));
    //                File.Move(archivoPrincipal, destinoRepository, true);

    //                foreach (var archivoDuplicado in subgrupo.Skip(1))
    //                {
    //                    string rutaRelativaDuplicado = Path.GetRelativePath(directorioBase, archivoDuplicado);
    //                    string destinoDelete = Path.Combine(carpetaDelete, rutaRelativaDuplicado);
    //                    Directory.CreateDirectory(Path.GetDirectoryName(destinoDelete));
    //                    File.Move(archivoDuplicado, destinoDelete, true);
    //                }
    //            }
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        Console.WriteLine($"Error general: {ex.Message}");
    //    }
    //    finally
    //    {
    //        GuardarArchivosProcesados(archivoSalida, archivosProcesados);
    //        File.AppendAllLines(archivoSalida, resultadosDuplicados);
    //    }

    //    return duplicados;
    //}

    private string ObtenerContenidoArchivo(string rutaArchivo)
    {
        using (var stream = File.OpenRead(rutaArchivo))
        using (var reader = new StreamReader(stream))
        {
            return reader.ReadToEnd();
        }
    }

    public List<List<string>> EncontrarArchivosDuplicados(string directorioBase,
                                                          HashSet<string> carpetasAProcesar,
                                                          HashSet<string> carpetasExcluidas,
                                                          string carpetaRepository,
                                                          string carpetaDelete,
                                                          string rutaArchivoCarpetas,
                                                          string archivoSalida)
    {
        var archivosPorTamano = new Dictionary<long, List<string>>();
        var duplicados = new List<List<string>>();
        //var carpetasAProcesar = new HashSet<string> { directorioBase };
        //carpetasAProcesar.UnionWith(carpetasIncluidas);

        var archivosProcesados = new List<string>();
        var resultadosDuplicados = new List<string>();

        Directory.CreateDirectory(carpetaRepository);
        Directory.CreateDirectory(carpetaDelete);

        try
        {
            foreach (var carpeta in carpetasAProcesar.Where(x => !string.IsNullOrEmpty(x)))
            {
                foreach (var archivo in Directory.EnumerateFiles(carpeta, "*", SearchOption.AllDirectories))
                {
                    if (carpetasExcluidas.Any(excluir => archivo.StartsWith(excluir, StringComparison.OrdinalIgnoreCase)))
                        continue;

                    long tamaño = new FileInfo(archivo).Length;
                    if (!archivosPorTamano.ContainsKey(tamaño))
                        archivosPorTamano[tamaño] = new List<string>();
                    archivosPorTamano[tamaño].Add(archivo);
                }
            }

            foreach (var grupo in archivosPorTamano.Values.Where(g => g.Count > 1))
            {
                var archivosPorHash = new Dictionary<string, List<string>>();
                Parallel.ForEach(grupo, archivo =>
                {
                    try
                    {
                        string hash = ObtenerHashArchivo(archivo);
                        lock (archivosPorHash)
                        {
                            if (!archivosPorHash.ContainsKey(hash))
                                archivosPorHash[hash] = new List<string>();
                            archivosPorHash[hash].Add(archivo);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error al procesar {archivo}: {ex.Message}");
                    }
                });

                foreach (var subgrupo in archivosPorHash.Values.Where(g => g.Count > 1))
                {
                    duplicados.Add(subgrupo);
                    resultadosDuplicados.Add("Grupo de archivos duplicados:");
                    resultadosDuplicados.AddRange(subgrupo);
                    resultadosDuplicados.Add("");


                    if (!_request.MoverArchivos)
                        continue;

                    // Determinar la ruta más corta
                    string rutaMasCorta = subgrupo.OrderBy(r => r.Length).First();
                    string carpetaRepetidos = Path.Combine(Path.GetDirectoryName(rutaMasCorta), "repetidos");

                    // Crear la carpeta "repetidos" si no existe
                    Directory.CreateDirectory(carpetaRepetidos);

                    // Copiar uno de los archivos duplicados a la carpeta "repetidos"
                    string archivoDestino = Path.Combine(carpetaRepetidos, Path.GetFileName(rutaMasCorta));
                    File.Copy(rutaMasCorta, archivoDestino, true);

                    // Borrar ambos archivos duplicados
                    foreach (var archivoDuplicado in subgrupo)
                    {
                        File.Delete(archivoDuplicado);
                    }

                    //string archivoPrincipal = subgrupo.First();
                    //string rutaRelativa = Path.GetRelativePath(directorioBase, archivoPrincipal);
                    //string destinoRepository = Path.Combine(carpetaRepository, rutaRelativa);
                    //Directory.CreateDirectory(Path.GetDirectoryName(destinoRepository));
                    //File.Move(archivoPrincipal, destinoRepository, true);

                    //foreach (var archivoDuplicado in subgrupo.Skip(1))
                    //{
                    //    string rutaRelativaDuplicado = Path.GetRelativePath(directorioBase, archivoDuplicado);
                    //    string destinoDelete = Path.Combine(carpetaDelete, rutaRelativaDuplicado);
                    //    Directory.CreateDirectory(Path.GetDirectoryName(destinoDelete));
                    //    File.Move(archivoDuplicado, destinoDelete, true);
                    //}


                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error general: {ex.Message}");
        }
        finally
        {
            GuardarArchivosProcesados(archivoSalida, archivosProcesados);
            File.AppendAllLines(archivoSalida, resultadosDuplicados);
        }

        return duplicados;
    }

    public void CopiarArchivosManteniendoEstructura(string carpetaOrigen, string carpetaDestinoBase)
    {
        foreach (string archivoOrigen in Directory.GetFiles(carpetaOrigen, "*", SearchOption.AllDirectories))
        {
            try
            {
                if (Path.GetExtension(archivoOrigen).Equals(".db", StringComparison.OrdinalIgnoreCase))
                    continue; // Ignorar archivos .db

                string rutaRelativa = Path.GetRelativePath(carpetaOrigen, archivoOrigen);
                string destinoFinal = Path.Combine(carpetaDestinoBase, rutaRelativa);
                string carpetaDestino = Path.GetDirectoryName(destinoFinal);

                if (!Directory.Exists(carpetaDestino))
                {
                    Directory.CreateDirectory(carpetaDestino);
                }

                File.Copy(archivoOrigen, destinoFinal, true);
                Console.WriteLine($"Copiado: {archivoOrigen} -> {destinoFinal}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al copiar {archivoOrigen}: {ex.Message}");
            }
        }
    }

    public string ObtenerHashArchivo(string rutaArchivo)
    {
        string extension = Path.GetExtension(rutaArchivo).ToLowerInvariant();
        if (extension == ".jpg" || extension == ".jpeg" || extension == ".png")
        {
            using (var sha256 = SHA256.Create())
            {
                // Leer el contenido del archivo en un array de bytes
                byte[] fileBytes = File.ReadAllBytes(rutaArchivo);

                // Normalizar el contenido del archivo eliminando metadatos EXIF
                byte[] normalizedBytes = EliminarMetadatosEXIF(fileBytes);

                // Calcular el hash del contenido normalizado
                byte[] hashBytes = sha256.ComputeHash(normalizedBytes);
                return BitConverter.ToString(hashBytes).Replace("-", "");
            }
        }
        else if (extension == ".avi" || extension == ".mp4")
        {
            using (var sha256 = SHA256.Create())
            {
                // Leer el contenido del archivo en un array de bytes
                byte[] fileBytes = File.ReadAllBytes(rutaArchivo);

                // Calcular el hash del contenido del archivo
                byte[] hashBytes = sha256.ComputeHash(fileBytes);
                return BitConverter.ToString(hashBytes).Replace("-", "");
            }
        }
        else
        {
            using (var sha256 = SHA256.Create())
            {
                // Leer el contenido del archivo en un array de bytes
                byte[] fileBytes = File.ReadAllBytes(rutaArchivo);

                // Calcular el hash del contenido del archivo
                byte[] hashBytes = sha256.ComputeHash(fileBytes);
                return BitConverter.ToString(hashBytes).Replace("-", "");
            }
        }
    }

    private byte[] EliminarMetadatosEXIF(byte[] fileBytes)
    {
        using (var ms = new MemoryStream(fileBytes))
        using (var image = Image.FromStream(ms))
        {
            // Crear una nueva imagen sin metadatos EXIF
            using (var newImage = new Bitmap(image))
            {
                using (var newMs = new MemoryStream())
                {
                    newImage.Save(newMs, image.RawFormat);
                    return newMs.ToArray();
                }
            }
        }
    }

    public void GuardarArchivosProcesados(string rutaSalida, List<string> archivosProcesados)
    {
        try
        {
            File.AppendAllLines(rutaSalida, archivosProcesados);
            Console.WriteLine($"Lista de archivos procesados guardada en: {rutaSalida}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al guardar archivos procesados: {ex.Message}");
        }
    }

    public void DeleteEmptyFolders(HashSet<string> carpetasAProcesar, HashSet<string> carpetasExcluidas, string rutaArchivoSalida)
    {
        using (StreamWriter logFile = new StreamWriter(rutaArchivoSalida, append: true))
        {
            int deletedCount = DeleteEmptyFoldersRecursive(carpetasAProcesar, carpetasExcluidas, logFile);
            logFile.WriteLine($"\nTotal de carpetas eliminadas: {deletedCount}");
        }
    }

    private int DeleteEmptyFoldersRecursive(HashSet<string> carpetasAProcesar, HashSet<string> carpetasExcluidas, StreamWriter logFile)
    {
        int deletedCount = 0;

        foreach (string dir in carpetasAProcesar.ToList()) // Usar ToList() para evitar modificaciones durante la iteración
        {
            // Excluir carpetas según la lista de exclusión
            if (carpetasExcluidas.Contains(dir))
            {
                continue; // Si la carpeta está en las carpetas excluidas, la saltamos
            }

            foreach (var subDir in Directory.GetDirectories(dir)) // Recorrer las subcarpetas
            {
                deletedCount += DeleteEmptyFoldersRecursive(new HashSet<string> { subDir }, carpetasExcluidas, logFile);
            }

            // Si la carpeta está vacía, la eliminamos
            if (Directory.GetFiles(dir).Length == 0 && Directory.GetDirectories(dir).Length == 0)
            {
                try
                {
                    Directory.Delete(dir);
                    logFile.WriteLine($"Carpeta eliminada: {dir}");
                    deletedCount++;
                }
                catch (Exception ex)
                {
                    logFile.WriteLine($"Error eliminando {dir}: {ex.Message}");
                }
            }
        }

        return deletedCount;
    }

    public List<string> CargarCarpetasDesdeArchivo(string rutaArchivo)
    {
        return File.Exists(rutaArchivo) ? File.ReadAllLines(rutaArchivo).ToList() : new List<string>();
    }

    public string GetArchivoDeSalida(string rutaArchivo)
    {
        string directorioSalida = Path.GetDirectoryName(rutaArchivo);
        return Path.Combine(directorioSalida, $"Duplicados_{DateTime.Now:yyyyMMdd_HHmmss}.txt");
    }

    public void EliminarArchivosConExtensiones(string directorioBase, 
                                               HashSet<string> carpetasAProcesar, 
                                               HashSet<string> carpetasExcluidas)
    {
        var extensionesAEliminar = new HashSet<string> { ".modd", ".moff", ".thm", ".db" };

        try
        {
            foreach (var carpeta in carpetasAProcesar.Where(x => !string.IsNullOrEmpty(x)))
            {
                foreach (var archivo in Directory.EnumerateFiles(carpeta, "*", SearchOption.AllDirectories))
                {
                    if (carpetasExcluidas.Any(excluir => archivo.StartsWith(excluir, StringComparison.OrdinalIgnoreCase)))
                        continue;

                    string extension = Path.GetExtension(archivo).ToLowerInvariant();
                    if (extensionesAEliminar.Contains(extension))
                    {
                        try
                        {
                            File.Delete(archivo);
                            Console.WriteLine($"Archivo eliminado: {archivo}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error al eliminar {archivo}: {ex.Message}");
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error general al eliminar archivos: {ex.Message}");
        }
    }


}
