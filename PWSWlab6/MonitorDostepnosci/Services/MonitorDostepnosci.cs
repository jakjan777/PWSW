using System.Diagnostics;
using MonitorDostepnosci.Models;

namespace MonitorDostepnosci.Services;

public class MonitorDostepnosci : BackgroundService
{
    private readonly IHttpClientFactory _httpFactory;
    private readonly RepozytoriumWynikow _repo;
    private readonly ILogger<MonitorDostepnosci> _logger;

    private readonly List<MonitorowanaStrona> _strony =
    [
        new("GitHub", "https://github.com"),
        new("Google", "https://www.google.com"),
        new("Microsoft", "https://www.microsoft.com"),
    ];

    public MonitorDostepnosci(
        IHttpClientFactory httpFactory,
        RepozytoriumWynikow repo,
        ILogger<MonitorDostepnosci> logger)
    {
        _httpFactory = httpFactory;
        _repo = repo;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        _logger.LogInformation("Monitor dostepnosci uruchomiony");

        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(30));

        await SprawdzWszystkieAsync(ct);

        while (await timer.WaitForNextTickAsync(ct))
            await SprawdzWszystkieAsync(ct);

        _logger.LogInformation("Monitor dostepnosci zatrzymany");
    }

    private async Task SprawdzWszystkieAsync(CancellationToken ct)
    {
        var zadania = _strony.Select(s => SprawdzStroneAsync(s, ct));
        await Task.WhenAll(zadania);
    }

    private async Task SprawdzStroneAsync(MonitorowanaStrona strona, CancellationToken ct)
    {
        var http = _httpFactory.CreateClient();
        http.Timeout = TimeSpan.FromSeconds(10);
        var stoper = Stopwatch.StartNew();
        try
        {
            var resp = await http.GetAsync(strona.Url, ct);
            stoper.Stop();
            _repo.Dodaj(new WynikSprawdzenia(
                strona.Nazwa, resp.IsSuccessStatusCode,
                (int)resp.StatusCode, stoper.ElapsedMilliseconds,
                DateTime.UtcNow));
            _logger.LogInformation("{Strona}: {Kod} w {Czas} ms",
                strona.Nazwa, (int)resp.StatusCode, stoper.ElapsedMilliseconds);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            stoper.Stop();
            _repo.Dodaj(new WynikSprawdzenia(
                strona.Nazwa, false, 0, stoper.ElapsedMilliseconds,
                DateTime.UtcNow));
            _logger.LogWarning("{Strona}: NIEDOSTEPNA -- {Blad}",
                strona.Nazwa, ex.Message);
        }
    }
}
