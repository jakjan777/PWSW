using MonitorDostepnosci.Models;

namespace MonitorDostepnosci;

public class RepozytoriumWynikow
{
    private readonly List<WynikSprawdzenia> _wyniki = [];

    public void Dodaj(WynikSprawdzenia wynik) => _wyniki.Add(wynik);

    public IReadOnlyList<WynikSprawdzenia> PobierzWszystkie() => _wyniki.AsReadOnly();
}
