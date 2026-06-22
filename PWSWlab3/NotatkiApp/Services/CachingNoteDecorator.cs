using NotatkiApp.Models;

namespace NotatkiApp.Services;

public class CachingNoteDecorator(
    INoteRepository inner,
    RepositoryCacheDiagnostics diagnostics) : INoteRepository
{
    private List<Note>? _cache;

    public async Task<IEnumerable<Note>> GetAllAsync()
    {
        if (_cache is not null)
        {
            diagnostics.Hit();
            return _cache;
        }

        diagnostics.Miss();
        var notes = await inner.GetAllAsync();
        _cache = notes.ToList();
        return _cache;
    }

    public Task<Note?> GetByIdAsync(Guid id)
    {
        return inner.GetByIdAsync(id);
    }

    public Task SaveAsync(Note note)
    {
        _cache = null;
        diagnostics.Invalidated();
        return inner.SaveAsync(note);
    }

    public Task DeleteAsync(Guid id)
    {
        _cache = null;
        diagnostics.Invalidated();
        return inner.DeleteAsync(id);
    }
}
