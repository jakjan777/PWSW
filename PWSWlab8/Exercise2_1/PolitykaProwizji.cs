namespace Exercise2_1;

public class PolitykaProwizji
{
    public decimal ObliczProwizje(decimal kwota, TypKlienta typ)
    {
        decimal stawka = typ switch
        {
            TypKlienta.Premium => 0.005m,
            TypKlienta.Biznesowy => 0.01m,
            TypKlienta.Detaliczny => 0.02m,
            _ => throw new ArgumentException($"Nieznany typ: {typ}")
        };

        return Math.Clamp(kwota * stawka, 1m, 500m);
    }
}
