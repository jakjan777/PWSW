using NotatkiApp.Models;

namespace NotatkiApp.Services;

public class InMemoryNoteRepository : INoteRepository
{
    private readonly Dictionary<Guid, Note> _store = [];

    public InMemoryNoteRepository()
    {
        SaveSeed(new Note
        {
            Title = "MAUI Blazor Hybrid",
            Content = "Model Note i NoteService sa zarejestrowane w DI.",
            Category = "Praca",
            IsPinned = true
        });

        SaveSeed(new Note
        {
            Title = "BlazorWebView",
            Content = "MainPage.xaml osadza komponent Razor w natywnej aplikacji MAUI.",
            Category = "Pomysly"
        });

        SaveSeed(new Note
        {
            Title = "Oddac sprawozdanie",
            Content = "Dzisiaj.",
            Category = "Pilne"
        });

        SaveSeed(new Note
        {
            Title = "Stare zalozenia",
            Content = "Notatka zostawiona tylko jako material archiwalny.",
            Category = "Archiwum"
        });

        SaveSeed(new Note
        {
            Title = "Dlugie rozpoznanie",
            Content = new string('x', 360),
            Category = "Pomysly"
        });
    }

    public Task<IEnumerable<Note>> GetAllAsync()
    {
        var notes = _store.Values
            .OrderByDescending(note => note.IsPinned)
            .ThenByDescending(note => note.ModifiedAt)
            .ToList()
            .AsEnumerable();

        return Task.FromResult(notes);
    }

    public Task<Note?> GetByIdAsync(Guid id)
    {
        _store.TryGetValue(id, out var note);
        return Task.FromResult(note);
    }

    public Task SaveAsync(Note note)
    {
        if (note.Id == Guid.Empty)
        {
            note.Id = Guid.NewGuid();
        }

        if (_store.TryGetValue(note.Id, out var existing))
        {
            note.CreatedAt = existing.CreatedAt;
        }
        else
        {
            note.CreatedAt = DateTime.Now;
        }

        note.ModifiedAt = DateTime.Now;
        _store[note.Id] = note;

        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid id)
    {
        _store.Remove(id);
        return Task.CompletedTask;
    }

    private void SaveSeed(Note note)
    {
        var now = DateTime.Now;
        note.CreatedAt = now;
        note.ModifiedAt = now;
        _store[note.Id] = note;
    }
}
