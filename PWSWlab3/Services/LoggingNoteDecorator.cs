using PWSWlab3.Models;

namespace PWSWlab3.Services;

public class LoggingNoteDecorator(
    INoteService inner,
    ILogger<LoggingNoteDecorator> logger) : INoteService
{
    public IReadOnlyList<Note> GetAll()
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = inner.GetAll();
        stopwatch.Stop();

        logger.LogInformation(
            "GetAll: {Count} notatek, czas: {Milliseconds} ms",
            result.Count,
            stopwatch.ElapsedMilliseconds);

        return result;
    }

    public Note? GetById(Guid id)
    {
        logger.LogInformation("GetById: {Id}", id);
        return inner.GetById(id);
    }

    public IEnumerable<Note> Search(string query)
    {
        logger.LogInformation("Search: {Query}", query);
        return inner.Search(query);
    }

    public void Add(Note note)
    {
        logger.LogInformation("Add: {Title}", note.Title);
        inner.Add(note);
    }

    public bool Update(Note note)
    {
        logger.LogInformation("Update: {Id}", note.Id);
        return inner.Update(note);
    }

    public bool Delete(Guid id)
    {
        logger.LogWarning("Delete: {Id}", id);
        return inner.Delete(id);
    }
}
