using System.Text.Json;
using Microsoft.Maui.ApplicationModel.DataTransfer;
using Microsoft.Maui.Storage;

namespace NotatkiApp.Services;

public class ExportService(INoteService noteService)
{
    public async Task<string> ExportToJsonAsync()
    {
        var notes = noteService.GetAll();
        var json = JsonSerializer.Serialize(notes, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        var filePath = Path.Combine(
            FileSystem.AppDataDirectory,
            "notes-export.json");

        await File.WriteAllTextAsync(filePath, json);
        return filePath;
    }

    public async Task<string> ShareExportAsync()
    {
        var filePath = await ExportToJsonAsync();

        await Share.Default.RequestAsync(new ShareFileRequest
        {
            Title = "Eksport notatek",
            File = new ShareFile(filePath)
        });

        return filePath;
    }
}
