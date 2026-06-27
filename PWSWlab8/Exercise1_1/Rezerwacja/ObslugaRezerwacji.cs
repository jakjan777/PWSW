using Exercise1_1.Rezerwacja;

namespace Exercise1_1;

public static class ObslugaRezerwacji
{
    public static void ObsluzRezerwacje(Action akcja)
    {
        try
        {
            akcja();
        }
        catch (OverbookingException ex) when (ex.DostepneMiejsca > 0)
        {
            Console.WriteLine($"[LISTA OCZEKUJACYCH] {ex.Message}");
        }
        catch (OverbookingException)
        {
            Console.WriteLine("[BRAK MIEJSC] Lot calkowicie pelny.");
        }
        catch (PlatnoscException ex) when (ex.KodBledu == "TIMEOUT_SIECI")
        {
            Console.WriteLine("[SIEC] Blad przejsciowy -- ponow.");
        }
        catch (RezerwacjaException ex)
        {
            Console.WriteLine($"[REZERWACJA] {ex.Message}");
        }
    }
}
