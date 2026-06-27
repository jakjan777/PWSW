using NSubstitute;
using Xunit;

namespace Exercise2_1.Tests;

public class KalkulatorWalutowyTests
{
    private readonly ISerwisKursow _mockKursy;
    private readonly KalkulatorWalutowy _kalkulator;

    public KalkulatorWalutowyTests()
    {
        _mockKursy = Substitute.For<ISerwisKursow>();
        _kalkulator = new KalkulatorWalutowy(
            _mockKursy, new PolitykaProwizji());
    }

    [Fact]
    public void Przelicz_PlnNaEur_PoprawnyWynik()
    {
        _mockKursy.PobierzKurs("PLN", "EUR").Returns(0.25m);

        decimal wynik = _kalkulator.Przelicz(
            1000m, "PLN", "EUR", TypKlienta.Detaliczny);

        Assert.Equal(245.00m, wynik);
        _mockKursy.Received(1).PobierzKurs("PLN", "EUR");
    }

    [Fact]
    public void Przelicz_KwotaUjemna_RzucaArgumentException()
    {
        var wyjatek = Assert.Throws<ArgumentException>(() =>
            _kalkulator.Przelicz(-100m, "PLN", "EUR", TypKlienta.Detaliczny));

        Assert.Contains("dodatnia", wyjatek.Message);
    }

    [Theory]
    [InlineData("", "EUR")]
    [InlineData("PLN", "")]
    public void Przelicz_PustaWaluta_RzucaArgumentException(
        string zrodlo, string cel)
    {
        Assert.Throws<ArgumentException>(() =>
            _kalkulator.Przelicz(100m, zrodlo, cel, TypKlienta.Detaliczny));
    }
}
