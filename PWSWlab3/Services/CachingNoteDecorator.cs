using PWSWlab3.Models;

namespace PWSWlab3.Services;

public class CachingNoteDecorator(
    INoteService inner,
    DecoratorDiagnostics diagnostics) : INoteService
{
    private List<Note>? _cache;

    public IReadOnlyList<Note> GetAll()
    {
        if (_cache is not null)
        {
            diagnostics.SetCacheHit();
            return _cache;
        }

        diagnostics.SetCacheMiss();
        _cache = inner.GetAll().ToList();
        return _cache;
    }

    public Note? GetById(Guid id) => inner.GetById(id);

    public IEnumerable<Note> Search(string query)
    {
        return GetAll().Where(note =>
            note.Title.Contains(query, StringComparison.OrdinalIgnoreCase) ||
            note.Content.Contains(query, StringComparison.OrdinalIgnoreCase));
    }

    public void Add(Note note)
    {
        _cache = null;
        diagnostics.SetCacheInvalidated();
        inner.Add(note);
    }

    public bool Update(Note note)
    {
        _cache = null;
        diagnostics.SetCacheInvalidated();
        return inner.Update(note);
    }

    public bool Delete(Guid id)
    {
        _cache = null;
        diagnostics.SetCacheInvalidated();
        return inner.Delete(id);
    }
}
