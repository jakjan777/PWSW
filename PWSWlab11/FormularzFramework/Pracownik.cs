namespace FormularzFramework;

public class Pracownik
{
    [DisplayName("Imie")]
    [Required(ErrorMessage = "Imie jest wymagane")]
    [MaxLength(50)]
    [DisplayOrder(1)]
    public string Imie { get; set; } = "";

    [DisplayName("Nazwisko")]
    [Required]
    [MaxLength(50)]
    [DisplayOrder(2)]
    public string Nazwisko { get; set; } = "";

    [DisplayName("Adres email")]
    [Required]
    [EmailAddress]
    [DisplayOrder(3)]
    public string Email { get; set; } = "";

    [DisplayName("Wiek")]
    [Range(18, 100, ErrorMessage = "Wiek musi byc miedzy 18 a 100")]
    [DisplayOrder(4)]
    public int Wiek { get; set; }

    [DisplayName("Pensja (PLN)")]
    [Range(0, 1_000_000)]
    [DisplayOrder(5)]
    public decimal Pensja { get; set; }
}
