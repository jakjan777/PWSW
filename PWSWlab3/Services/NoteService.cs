using PWSWlab3.Models;

namespace PWSWlab3.Services;

public class NoteService(InMemoryNoteRepository repository) : INoteService
{
    public IReadOnlyList<Note> GetAll() => repository.GetAll();

    public Note? GetById(Guid id) => repository.GetById(id);

    public IEnumerable<Note> Search(string query) => ((ISearchable)repository).Search(query);

    public void Add(Note note) => repository.Add(note);

    public bool Update(Note note)
    {
        if (repository.GetById(note.Id) is null)
        {
            return false;
        }

        repository.Update(note);
        return true;
    }

    public bool Delete(Guid id)
    {
        if (repository.GetById(id) is null)
        {
            return false;
        }

        repository.Delete(id);
        return true;
    }
}
