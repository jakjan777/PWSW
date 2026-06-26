namespace MonitorDostepnosci.Models;

public record WynikSprawdzenia(
    string Nazwa,
    bool Sukces,
    int KodHttp,
    long CzasMs,
    DateTime SprawdzonoUtc);
