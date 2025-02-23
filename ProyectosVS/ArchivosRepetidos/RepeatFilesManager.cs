using System;
using System.Security.Cryptography;

public class RepeatFilesManager
{
	public RepeatFilesManager()
	{
	}

    public List<List<string>> EncontrarArchivosDuplicados(string directorioBase, 
                                                          HashSet<string> carpetasIncluidas, 
                                                          HashSet<string> carpetasExcluidas, 
                                                          List<string> carpetasDesdeArchivo, 
                                                          string rutaArchivoCarpetas)
    {
        var archivosPorTamano = new Dictionary<long, List<string>>();
        var duplicados = new List<List<string>>();
        var carpetasAProcesar = new HashSet<string> { directorioBase };
        carpetasAProcesar.UnionWith(carpetasIncluidas);
        carpetasAProcesar.UnionWith(carpetasDesdeArchivo);

        string directorioSalida = Path.GetDirectoryName(rutaArchivoCarpetas);
        string archivoSalida = Path.Combine(directorioSalida, $"Duplicados_{DateTime.Now:yyyyMMdd_HHmmss}.txt");
        var archivosProcesados = new List<string>();
        var resultadosDuplicados = new List<string>();

        string carpetaRepository = @"E:\\ComprobarRepetidos\\Repository";
        string carpetaDelete = @"E:\\ComprobarRepetidos\\Delete";
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

                    string archivoPrincipal = subgrupo.First();
                    string rutaRelativa = Path.GetRelativePath(directorioBase, archivoPrincipal);
                    string destinoRepository = Path.Combine(carpetaRepository, rutaRelativa);
                    Directory.CreateDirectory(Path.GetDirectoryName(destinoRepository));
                    File.Move(archivoPrincipal, destinoRepository, true);

                    foreach (var archivoDuplicado in subgrupo.Skip(1))
                    {
                        string rutaRelativaDuplicado = Path.GetRelativePath(directorioBase, archivoDuplicado);
                        string destinoDelete = Path.Combine(carpetaDelete, rutaRelativaDuplicado);
                        Directory.CreateDirectory(Path.GetDirectoryName(destinoDelete));
                        File.Move(archivoDuplicado, destinoDelete, true);
                    }
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

    static string ObtenerHashArchivo(string rutaArchivo)
    {
        using (var sha256 = SHA256.Create())
        using (var stream = File.OpenRead(rutaArchivo))
        {
            byte[] hashBytes = sha256.ComputeHash(stream);
            return BitConverter.ToString(hashBytes).Replace("-", "");
        }
    }

    static void GuardarArchivosProcesados(string rutaSalida, List<string> archivosProcesados)
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

    public List<string> CargarCarpetasDesdeArchivo(string rutaArchivo)
    {
        return File.Exists(rutaArchivo) ? File.ReadAllLines(rutaArchivo).ToList() : new List<string>();
    }
}
