namespace NotatkiApp.Services;

public class RepositoryCacheDiagnostics
{
    public string LastEvent { get; private set; } = "Cache repozytorium nie zostal jeszcze uzyty.";

    public void Hit()
    {
        LastEvent = "Cache HIT - liste notatek zwrocono z CachingNoteDecorator.";
    }

    public void Miss()
    {
        LastEvent = "Cache MISS - liste notatek pobrano z InMemoryNoteRepository.";
    }

    public void Invalidated()
    {
        LastEvent = "Cache INVALIDATED - cache wyczyszczono po zapisie lub usunieciu.";
    }
}
