using PWSWlab3.Models;

namespace PWSWlab3.Services;

public static class NoteClassifier
{
    public static string Classify(Note note) =>
        note switch
        {
            { IsPinned: true, Category: "Praca" } => "Wazne zadanie sluzbowe",
            { Category: "Praca" or "Osobiste" } => "Notatka kategoryzowana",
            { Content.Length: > 500, Category: "Pomysly" } => "Rozbudowany pomysl",
            { Content.Length: var length } when length == 0 => "Pusta notatka - do uzupelnienia",
            { IsPinned: true } => "Przypiety element",
            _ => "Notatka standardowa"
        };

    public static string AnalyzeActivity(int[] notesPerDay) =>
        notesPerDay switch
        {
            [] => "Brak aktywnosci",
            [var only] => $"Jeden dzien: {only} notatek",
            [> 10, > 10, ..] => "Wysoka aktywnosc na starcie",
            [var first, .., var last] when last > first => "Trend rosnacy aktywnosci",
            _ => "Aktywnosc stabilna"
        };
}
