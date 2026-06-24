using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace PWSW5;

public class FilePlugin : IAgentPlugin
{
    public string Nazwa => "FilePlugin";
    public string OpisFunkcji => "Operacje na systemie plikow";

    [KernelFunction, Description("Listuje pliki w podanym katalogu")]
    public string[] ListFiles(
        [Description("Sciezka do katalogu")] string path,
        [Description("Filtr rozszerzenia, np. '*.txt'")] string filter = "*.*")
    {
        if (!Directory.Exists(path))
            return [$"Katalog nie istnieje: {path}"];
        return Directory.GetFiles(path, filter)
            .Select(Path.GetFileName)
            .ToArray()!;
    }

    [KernelFunction, Description("Czyta zawartosc pliku tekstowego")]
    public async Task<string> ReadFile(
        [Description("Pelna sciezka do pliku")] string filePath)
    {
        if (!File.Exists(filePath))
            return $"Plik nie istnieje: {filePath}";
        return await File.ReadAllTextAsync(filePath);
    }

    [KernelFunction, Description("Zwraca informacje o pliku: rozmiar i daty")]
    public string GetFileInfo(
        [Description("Pelna sciezka do pliku")] string filePath)
    {
        if (!File.Exists(filePath))
            return $"Plik nie istnieje: {filePath}";
        var info = new FileInfo(filePath);
        return $"Nazwa: {info.Name}, Rozmiar: {info.Length} B, " +
               $"Zmodyfikowany: {info.LastWriteTime:dd.MM.yyyy HH:mm}";
    }
}
