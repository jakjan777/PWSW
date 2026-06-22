using NotatkiApp.Models;

namespace NotatkiApp.Services;

public enum NoteClass
{
    Urgent,
    Archived,
    Extensive,
    PinnedWithCategory,
    Pinned,
    General
}

public interface INoteClassifier
{
    NoteClass Classify(Note note);

    string GetRecommendation(NoteClass noteClass);
}
