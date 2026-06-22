using NotatkiApp.Models;

namespace NotatkiApp.Services;

public interface INoteService
{
    IReadOnlyList<Note> GetAll();

    Note? GetById(Guid id);

    void Add(Note note);

    void Update(Note note);

    void Delete(Guid id);
}
