namespace PWSWlab3.Services;

public class DecoratorDiagnostics
{
    public string LastCacheEvent { get; private set; } = "Cache nie zostal jeszcze uzyty.";

    public void SetCacheHit()
    {
        LastCacheEvent = "Cache HIT - dane pobrano z CachingNoteDecorator.";
    }

    public void SetCacheMiss()
    {
        LastCacheEvent = "Cache MISS - dane pobrano z NoteService i zapisano w cache.";
    }

    public void SetCacheInvalidated()
    {
        LastCacheEvent = "Cache INVALIDATED - cache wyczyszczono po zmianie danych.";
    }
}
