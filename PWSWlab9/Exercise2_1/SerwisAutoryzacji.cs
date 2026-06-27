using Microsoft.Extensions.Logging;

namespace Exercise2_1;

public class SerwisAutoryzacji(ILogger<SerwisAutoryzacji> logger)
{
    private readonly ILogger<SerwisAutoryzacji> _logger = logger;

    public bool Zaloguj(string login, string adresIp)
    {
        _logger.LogInformation(
            "Proba logowania {Login} z {AdresIp}", login, adresIp);

        if (login == "admin" && adresIp.StartsWith("192.168"))
        {
            _logger.LogInformation(
                "Uzytkownik {Login} zalogowany pomyslnie", login);
            return true;
        }

        _logger.LogWarning(
            "Nieudane logowanie {Login} z {AdresIp}", login, adresIp);
        return false;
    }
}
