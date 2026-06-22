using PWSWlab3.Models;

namespace PWSWlab3.Services;

public class InMemoryNoteRepository :
    ISearchable,
    INoteRepositoryFactory<InMemoryNoteRepository>
{
    private readonly Dictionary<Guid, Note> _notes = [];

    public InMemoryNoteRepository()
    {
        Add(new Note
        {
            Title = "Plan laboratorium",
            Content = "Przygotowac cwiczenia 1.1 oraz 2.1.",
            Category = "Praca",
            IsPinned = true
        });

        Add(new Note
        {
            Title = "Pomysl na aplikacje",
            Content = "Lista funkcji do rozbudowy aplikacji notatek.",
            Category = "Pomysly"
        });
    }

    public static InMemoryNoteRepository Create() => new();

    public IReadOnlyList<Note> GetAll()
    {
        return _notes.Values
            .OrderByDescending(note => note.IsPinned)
            .ThenByDescending(note => note.ModifiedAt)
            .ToList();
    }

    public Note? GetById(Guid id)
    {
        return _notes.TryGetValue(id, out var note) ? note : null;
    }

    public void Add(Note note)
    {
        if (note.Id == Guid.Empty)
        {
            note.Id = Guid.NewGuid();
        }

        var now = DateTime.Now;
        note.CreatedAt = now;
        note.ModifiedAt = now;
        _notes[note.Id] = note;
    }

    public void Update(Note note)
    {
        if (!_notes.ContainsKey(note.Id))
        {
            return;
        }

        note.ModifiedAt = DateTime.Now;
        _notes[note.Id] = note;
    }

    public void Delete(Guid id)
    {
        _notes.Remove(id);
    }
}
