namespace Exercise1_1.Logging;

public interface ILogger : IDisposable
{
    void Loguj(PoziomLogu poziom, string wiadomosc);
}
