using PWSWlab3.Models;

namespace PWSWlab3.Services;

public interface INoteService
{
    IReadOnlyList<Note> GetAll();

    Note? GetById(Guid id);

    IEnumerable<Note> Search(string query);

    void Add(Note note);

    bool Update(Note note);

    bool Delete(Guid id);
}
