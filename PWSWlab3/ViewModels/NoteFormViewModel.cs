using System.ComponentModel.DataAnnotations;

namespace PWSWlab3.ViewModels;

public class NoteFormViewModel
{
    public Guid? Id { get; set; }

    [Required(ErrorMessage = "Tytul notatki jest wymagany.")]
    [StringLength(100, ErrorMessage = "Tytul moze miec maksymalnie 100 znakow.")]
    public string Title { get; set; } = string.Empty;

    public string? Content { get; set; }

    public string? Category { get; set; }

    public bool IsPinned { get; set; }
}
