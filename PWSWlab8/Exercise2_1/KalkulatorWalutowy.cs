namespace Exercise2_1;

public class KalkulatorWalutowy(ISerwisKursow kursy, PolitykaProwizji prowizje)
{
    public decimal Przelicz(
        decimal kwota, string zrodlo, string cel, TypKlienta typ)
    {
        if (kwota <= 0)
            throw new ArgumentException("Kwota musi byc dodatnia");

        if (string.IsNullOrEmpty(zrodlo) || string.IsNullOrEmpty(cel))
            throw new ArgumentException("Waluta nie moze byc pusta");

        decimal kurs = kursy.PobierzKurs(zrodlo, cel);
        decimal prowizja = prowizje.ObliczProwizje(kwota, typ);

        return Math.Round((kwota - prowizja) * kurs, 2, MidpointRounding.ToEven);
    }
}
