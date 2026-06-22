using PWSWlab3.Models;

namespace PWSWlab3.Services;

public interface INoteRepository
{
    IReadOnlyList<Note> GetAll();

    Note? GetById(Guid id);

    void Add(Note note);

    void Update(Note note);

    void Delete(Guid id);
}

public interface ISearchable : INoteRepository
{
    IEnumerable<Note> Search(string query)
    {
        return GetAll().Where(note =>
            note.Title.Contains(query, StringComparison.OrdinalIgnoreCase) ||
            note.Content.Contains(query, StringComparison.OrdinalIgnoreCase));
    }
}

public interface INoteRepositoryFactory<TSelf>
    where TSelf : INoteRepositoryFactory<TSelf>
{
    static abstract TSelf Create();
}
