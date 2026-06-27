using Microsoft.Extensions.Logging;

namespace Exercise2_1;

public class SerwisTransakcji(
    ILogger<SerwisTransakcji> logger,
    SerwisAutoryzacji autoryzacja)
{
    private readonly ILogger<SerwisTransakcji> _logger = logger;
    private readonly SerwisAutoryzacji _autoryzacja = autoryzacja;

    public void ObsluzSesje(string uzytkownik, string ip)
    {
        using (_logger.BeginScope(new Dictionary<string, object>
        {
            ["SesjaId"] = Guid.NewGuid().ToString()[..8],
            ["Uzytkownik"] = uzytkownik,
            ["Ip"] = ip
        }))
        {
            _logger.LogInformation("Rozpoczecie sesji");
            bool ok = _autoryzacja.Zaloguj(uzytkownik, ip);
            if (!ok)
            {
                _logger.LogWarning("Brak autoryzacji -- koniec sesji");
                return;
            }
            _logger.LogInformation("Sesja zakonczona pomyslnie");
        }
    }

    public void WykonajPrzelew(
        string nadawca, string odbiorca, decimal kwota)
    {
        LogiTransakcji.PrzelewZrealizowany(_logger, kwota, nadawca, odbiorca);

        if (kwota >= 10_000m)
            LogiTransakcji.PrzelewWysokiejWartosci(_logger, kwota);
    }
}
