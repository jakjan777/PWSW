using FluentValidation;
using KonferencjaApi.Models;

namespace KonferencjaApi.Validators;

public class RejestracjaValidator : AbstractValidator<RejestracjaRequest>
{
    private readonly HashSet<string> _kody = ["STUDENT-2026", "EARLY-BIRD", "SPEAKER-VIP"];

    public RejestracjaValidator()
    {
        RuleFor(x => x.Imie).NotEmpty().MinimumLength(2).MaximumLength(50);
        RuleFor(x => x.Email).NotEmpty().EmailAddress()
            .Must(e => !e.EndsWith("@tempmail.com"))
            .WithMessage("Tymczasowe adresy email nie sa akceptowane");
        RuleFor(x => x.TypUczestnika).NotEmpty()
            .Must(t => t is "Uczestnik" or "Prelegent" or "Student");
        When(x => !string.IsNullOrEmpty(x.KodRabatowy), () =>
            RuleFor(x => x.KodRabatowy)
                .Must(k => _kody.Contains(k!))
                .WithMessage("Nieprawidlowy kod rabatowy"));
        RuleFor(x => x.LiczbaSesji).Equal(0)
            .When(x => x.TypUczestnika == "Prelegent")
            .WithMessage("Prelegent nie wybiera sesji");
    }
}
