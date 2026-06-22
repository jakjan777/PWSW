using NotatkiApp.Models;

namespace NotatkiApp.Services;

public class NoteService(INoteRepository repository) : INoteService
{
    public IReadOnlyList<Note> GetAll()
    {
        return repository.GetAllAsync()
            .GetAwaiter()
            .GetResult()
            .ToList();
    }

    public Note? GetById(Guid id)
    {
        return repository.GetByIdAsync(id)
            .GetAwaiter()
            .GetResult();
    }

    public void Add(Note note)
    {
        repository.SaveAsync(note)
            .GetAwaiter()
            .GetResult();
    }

    public void Update(Note note)
    {
        repository.SaveAsync(note)
            .GetAwaiter()
            .GetResult();
    }

    public void Delete(Guid id)
    {
        repository.DeleteAsync(id)
            .GetAwaiter()
            .GetResult();
    }
}
