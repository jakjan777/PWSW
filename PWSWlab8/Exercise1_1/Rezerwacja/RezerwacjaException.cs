namespace Exercise1_1.Rezerwacja;

public class RezerwacjaException(string msg, string? nrLotu = null, Exception? inner = null)
    : Exception(msg, inner)
{
    public string? NumerLotu { get; } = nrLotu;
    public DateTime CzasZdarzenia { get; } = DateTime.Now;
}

public class OverbookingException(string nrLotu, int dostepne, int wymagane)
    : RezerwacjaException(
        $"Lot {nrLotu}: dostepne {dostepne}, wymagane {wymagane}",
        nrLotu)
{
    public int DostepneMiejsca { get; } = dostepne;
}

public class PlatnoscException(decimal kwota, string kod, string? nrLotu = null)
    : RezerwacjaException($"Platnosc {kwota:C} nieudana ({kod})", nrLotu)
{
    public string KodBledu { get; } = kod;
}
