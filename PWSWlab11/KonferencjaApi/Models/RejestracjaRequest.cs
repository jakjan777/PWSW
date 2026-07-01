using System.ComponentModel.DataAnnotations;

namespace KonferencjaApi.Models;

public class RejestracjaRequest : IValidatableObject
{
    [Required(ErrorMessage = "Imie jest wymagane")]
    [StringLength(50, MinimumLength = 2)]
    public string Imie { get; set; } = string.Empty;

    [Required]
    [EmailAddress(ErrorMessage = "Nieprawidlowy adres email")]
    public string Email { get; set; } = string.Empty;

    [Range(18, 120, ErrorMessage = "Wiek musi byc miedzy 18 a 120")]
    public int Wiek { get; set; }

    [Required]
    [RegularExpression("^(Uczestnik|Prelegent|Student)$")]
    public string TypUczestnika { get; set; } = string.Empty;

    public string? KodRabatowy { get; set; }

    [Range(0, 5, ErrorMessage = "Mozna wybrac max 5 sesji")]
    public int LiczbaSesji { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext ctx)
    {
        if (KodRabatowy == "STUDENT-2026" && TypUczestnika != "Student")
            yield return new ValidationResult(
                "Kod STUDENT-2026 jest dostepny tylko dla studentow",
                [nameof(KodRabatowy), nameof(TypUczestnika)]);

        if (TypUczestnika == "Prelegent" && LiczbaSesji > 0)
            yield return new ValidationResult(
                "Prelegent nie wybiera sesji -- prowadzi wlasna",
                [nameof(LiczbaSesji)]);
    }
}
