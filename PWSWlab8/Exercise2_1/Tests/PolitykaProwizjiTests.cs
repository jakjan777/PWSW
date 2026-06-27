using Xunit;

namespace Exercise2_1.Tests;

public class PolitykaProwizjiTests
{
    [Theory]
    [InlineData(1000, TypKlienta.Detaliczny, 20)]
    [InlineData(1000, TypKlienta.Biznesowy, 10)]
    [InlineData(1000, TypKlienta.Premium, 5)]
    [InlineData(10, TypKlienta.Premium, 1)]
    [InlineData(100_000, TypKlienta.Detaliczny, 500)]
    public void ObliczProwizje_RozneKombinacje_PoprawnyWynik(
        decimal kwota, TypKlienta typ, decimal oczekiwana)
    {
        var polityka = new PolitykaProwizji();
        Assert.Equal(oczekiwana, polityka.ObliczProwizje(kwota, typ));
    }
}
