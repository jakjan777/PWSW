using NotatkiApp.Models;

namespace NotatkiApp.Services;

public interface INoteRepository
{
    Task<IEnumerable<Note>> GetAllAsync();

    Task<Note?> GetByIdAsync(Guid id);

    Task SaveAsync(Note note);

    Task DeleteAsync(Guid id);
}
