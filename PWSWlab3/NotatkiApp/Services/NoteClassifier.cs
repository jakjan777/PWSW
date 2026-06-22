using NotatkiApp.Models;

namespace NotatkiApp.Services;

public class NoteClassifier : INoteClassifier
{
    public NoteClass Classify(Note note) => ClassifyCore(note);

    public string GetRecommendation(NoteClass noteClass) =>
        noteClass switch
        {
            NoteClass.Urgent => "Zrealizuj od razu albo przenieś na początek listy zadań.",
            NoteClass.Archived => "Zostaw jako materiał historyczny, bez codziennego przeglądania.",
            NoteClass.Extensive => "Podziel treść na krótsze notatki albo dodaj podsumowanie.",
            NoteClass.PinnedWithCategory => "Trzymaj wysoko na liście, bo jest przypięta i ma kontekst.",
            NoteClass.Pinned => "Uzupełnij kategorię, żeby łatwiej ją później odnaleźć.",
            NoteClass.General => "Nie wymaga specjalnej akcji, wystarczy standardowe przechowywanie.",
            _ => "Brak rekomendacji."
        };

    public static ILookup<NoteClass, Note> BatchClassify(IEnumerable<Note> notes) =>
        notes.ToLookup(note => ClassifyCore(note));

    private static NoteClass ClassifyCore(Note note) =>
        note switch
        {
            { Category: "Pilne", Content.Length: <= 50 } => NoteClass.Urgent,
            { Category: "Archiwum" } => NoteClass.Archived,
            { Content.Length: > 300 } => NoteClass.Extensive,
            { IsPinned: true, Category: not null } => NoteClass.PinnedWithCategory,
            { IsPinned: true } => NoteClass.Pinned,
            _ => NoteClass.General
        };
}
