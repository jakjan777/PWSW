using System.Diagnostics;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using UglyToad.PdfPig;

string foundryBaseUrl =
    Environment.GetEnvironmentVariable("FOUNDRY_URL")
    ?? await FoundryUrlResolver.ResolveAsync();

Console.WriteLine($"Foundry URL: {foundryBaseUrl}");

const string pytanieAi = "Jakie jest rozwinięcie skrótu PWSW?";

Func<string, WynikWalidacji> niepuste = value =>
    string.IsNullOrWhiteSpace(value)
        ? WynikWalidacji.Blad("Wiadomosc nie moze byc pusta.")
        : WynikWalidacji.Ok;

Func<string, WynikWalidacji> MaxDlugosc(int max) =>
    value => value.Length > max
        ? WynikWalidacji.Blad($"Maksymalna dlugosc to {max} znakow.")
        : WynikWalidacji.Ok;

Func<string, WynikWalidacji> bezNiebezpiecznychZnakow = value =>
{
    char[] zabronione = ['<', '>', '"', '\'', '\\'];

    foreach (var znak in zabronione)
    {
        if (value.Contains(znak))
        {
            return WynikWalidacji.Blad(
                $"Wiadomosc zawiera niedozwolony znak: '{znak}'.");
        }
    }

    return WynikWalidacji.Ok;
};

Func<string, WynikWalidacji> tylkoTekst =
    static value => value.All(c =>
        char.IsLetterOrDigit(c)
        || char.IsWhiteSpace(c)
        || char.IsPunctuation(c))
            ? WynikWalidacji.Ok
            : WynikWalidacji.Blad("Wiadomosc zawiera niedozwolone znaki.");

var walidacjaWiadomosci = Walidator.Wszystkie(
    niepuste,
    MaxDlugosc(500),
    bezNiebezpiecznychZnakow,
    tylkoTekst);

var wiadomosci = new[]
{
    "",
    "Czym jest <NativeAOT>?",
    "Jak dziala lokalna inferencja AI?"
};

Action<string, string> zapiszBlad = (wiadomosc, blad) =>
    Console.WriteLine($"Blad walidacji dla \"{wiadomosc}\": {blad}");

foreach (var wiadomosc in wiadomosci)
{
    var poprawna = Walidator.Waliduj(wiadomosc, walidacjaWiadomosci, zapiszBlad);
    Console.WriteLine(poprawna
        ? $"OK: \"{wiadomosc}\""
        : $"NIEPOPRAWNA: \"{wiadomosc}\"");
}

Console.WriteLine();
Console.WriteLine("=== Pipeline odpowiedzi AI ===");

var suroweOdpowiedzi = new[]
{
    " Lokalna inferencja AI dziala poprawnie! ",
    ""
};

foreach (var surowaOdpowiedz in suroweOdpowiedzi)
{
    var wynik = PipelineOdpowiedzi.Przetworz(surowaOdpowiedz);
    Console.WriteLine(wynik);
}

Console.WriteLine();
Console.WriteLine("=== Historia konwersacji AI w SQLite ===");

await using var db = new ChatDbContext();
await db.Database.MigrateAsync();

var historia = new ChatHistoryService(db);
var sesja = await historia.StartSessionAsync("Demo lokalnego czatu AI");

await historia.AddEntryAsync(
    sesja.Id,
    "user",
    pytanieAi);

await historia.AddEntryAsync(
    sesja.Id,
    "assistant",
    "Programowanie w Srodowisku Windows.");

var zapisanaSesja = await historia.GetSessionAsync(sesja.Id);

if (zapisanaSesja is not null)
{
    Console.WriteLine($"Sesja: {zapisanaSesja.Title}");
    Console.WriteLine($"Model: {zapisanaSesja.ModelName}");
    Console.WriteLine("Historia:");

    foreach (var wpis in zapisanaSesja.Entries)
    {
        Console.WriteLine($"[{wpis.Timestamp:HH:mm:ss}] {wpis.Role}: {wpis.Content}");
    }
}

Console.WriteLine();
Console.WriteLine("=== Strategia wyboru dostawcy AI ===");

using var foundryHttp = new HttpClient
{
    BaseAddress = new Uri(foundryBaseUrl),
    Timeout = TimeSpan.FromSeconds(120)
};

var foundryLocal = new FoundryLocalClient(foundryHttp);

var mockProvider = new MockProvider();
var aiContext = new AiContext(mockProvider);
var odpMock = await aiContext.AskAsync(pytanieAi);
Console.WriteLine(odpMock);

var composite = new CompositeProvider(
    [new FoundryProvider(foundryLocal), new MockProvider()]);

aiContext.SetProvider(composite);
var odpAuto = await aiContext.AskAsync(pytanieAi);
Console.WriteLine(odpAuto);

Console.WriteLine();
Console.WriteLine("=== Cache odpowiedzi i rownolegle zapytania ===");

var cachedAi = new CachedAiService(mockProvider);

var pierwszaOdpowiedz = await cachedAi.AskAsync(pytanieAi);
Console.WriteLine(pierwszaOdpowiedz);

var drugaOdpowiedz = await cachedAi.AskAsync(pytanieAi);
Console.WriteLine(drugaOdpowiedz);

var odpowiedzi = await AiParallelPipeline.ZapytajWszystkichAsync(
    [new MockProvider(), composite],
    pytanieAi,
    timeout: TimeSpan.FromSeconds(120));

foreach (var (dostawca, wynik) in odpowiedzi)
{
    Console.WriteLine($"[{dostawca}]: {wynik}");
}

Console.WriteLine();
Console.WriteLine("=== Foundry Local ===");

if (await foundryLocal.IsAvailableAsync())
{
    Console.WriteLine($"Foundry Local: serwer dostepny na {foundryBaseUrl}");
    var odpFoundry = await foundryLocal.ChatAsync(pytanieAi);
    Console.WriteLine(odpFoundry);

    Console.WriteLine();
    Console.Write("Streaming: ");
    await foundryLocal.StreamChatAsync(pytanieAi);
    Console.WriteLine();
}
else
{
    Console.WriteLine("Foundry Local: serwer niedostepny.");
    Console.WriteLine("Start uslugi:   foundry service start");
    Console.WriteLine("Pobierz model:  foundry model download deepseek-r1-7b");
    Console.WriteLine("Zaladuj model:  foundry model load deepseek-r1-7b");
    Console.WriteLine("Status/port:    foundry service status");
}

Console.WriteLine();
Console.WriteLine("=== ONNX Runtime MobileNet ===");

var sciezkaModelu = Path.Combine(AppContext.BaseDirectory, "mobilenet.onnx");
if (!File.Exists(sciezkaModelu))
{
    Console.WriteLine($"Brak pliku modelu: {sciezkaModelu}");
}
else
{
    OnnxMobilenetDemo.Run(sciezkaModelu);
}

Console.WriteLine();
Console.WriteLine("=== AiRouter ===");

var router = new AiRouter(
[
    new FoundryAiBackend(foundryLocal),
    new MockAiBackend()
]);

foreach (var linia in router.GetCapabilityReport())
{
    Console.WriteLine(linia);
}

Console.WriteLine();
var odpRouter = await router.GenerateAsync(pytanieAi);
Console.WriteLine(odpRouter);

Console.WriteLine();
Console.WriteLine("=== Wyzwanie 2: ConversationRepository ===");

await using var aiDb = new AiDbContext();
await aiDb.Database.EnsureCreatedAsync();

if (!await aiDb.Entries.AnyAsync())
{
    aiDb.Entries.AddRange(
        new ConversationEntry
        {
            Role = "user",
            Content = "Jakie jest rozwinięcie skrótu PWSW?",
            CreatedAt = DateTime.Today.AddHours(9)
        },
        new ConversationEntry
        {
            Role = "assistant",
            Content = "Programowanie w Srodowisku Windows.",
            CreatedAt = DateTime.Today.AddHours(9).AddMinutes(1)
        },
        new ConversationEntry
        {
            Role = "assistant",
            Content = "Lokalna inferencja AI dziala poprawnie.",
            CreatedAt = DateTime.Today.AddHours(14)
        });

    await aiDb.SaveChangesAsync();
}

var conversationRepo = new ConversationRepository(aiDb);
var od = DateTime.Today;
var doDaty = DateTime.Today.AddDays(1).AddTicks(-1);

var zDnia = await conversationRepo.GetByDateRange(od, doDaty);
Console.WriteLine($"GetByDateRange ({od:yyyy-MM-dd}): {zDnia.Count} wpis(y)");

foreach (var wpis in zDnia)
{
    Console.WriteLine($"  [{wpis.CreatedAt:HH:mm:ss}] {wpis.Role}: {wpis.Content}");
}

var zSlowem = await conversationRepo.GetByKeyword("inferencja");
Console.WriteLine($"GetByKeyword (\"inferencja\"): {zSlowem.Count} wpis(y)");

foreach (var wpis in zSlowem)
{
    Console.WriteLine($"  {wpis.Content}");
}

var demoPipeline = AiResponsePipeline.Create(logToConsole: true);

Console.WriteLine();
Console.WriteLine("Pipeline na odpowiedzi MockAiBackend:");

var mockBackend = new MockAiBackend();
var rawResponse = await mockBackend.GenerateAsync(pytanieAi);
var processed = demoPipeline(rawResponse);
Console.WriteLine($"Wynik: {processed}");

Console.WriteLine("W REPL: search <fraza> | history");

Console.WriteLine();
Console.WriteLine("=== Lokalna aplikacja AI (Q&A + sumaryzacja) ===");

var foundryDostepny = await foundryLocal.IsAvailableAsync();
LocalAiService? localAi = null;
DocumentProcessor? processor = null;
Func<string, string>? replPipeline = null;
var przykladDoc = Path.Combine(AppContext.BaseDirectory, "sample-doc.txt");

if (foundryDostepny)
{
    localAi = new LocalAiService(foundryLocal);
    processor = new DocumentProcessor(localAi);
    replPipeline = AiResponsePipeline.Create(logToConsole: false);

    Console.WriteLine("Komendy: load | summary | ask | search | history | quit | help");
    Console.WriteLine($"Przyklad: load {przykladDoc}");
    Console.WriteLine("Wyszukiwanie: search inferencja");
    Console.WriteLine("Obslugiwane formaty: .txt, .pdf");
}
else
{
    Console.WriteLine("Foundry Local niedostepny - dostepne: search | history | quit | help");
    Console.WriteLine("Przyklad: search inferencja");
    Console.WriteLine("Start uslugi:   foundry service start");
    Console.WriteLine("Zaladuj model:  foundry model load deepseek-r1-7b");
}

if (Console.IsInputRedirected)
{
    Console.WriteLine("\nTryb nieinteraktywny (brak klawiatury) - pomijam REPL.");
    if (foundryDostepny && processor is not null && File.Exists(przykladDoc))
    {
        processor.LoadDocument(await File.ReadAllTextAsync(przykladDoc));
        Console.WriteLine("\n[auto] summary:");
        Console.WriteLine(await processor.SummarizeAsync());
    }
}
else
{
    Console.WriteLine();

    while (true)
    {
        Console.Write("> ");
        var raw = Console.ReadLine();
        var input = DocumentRepl.OczyscWejscie(raw);

        if (DocumentRepl.CzyPominacWejscie(input))
        {
            continue;
        }

        if (input.Equals("quit", StringComparison.OrdinalIgnoreCase)
            || input.Equals("exit", StringComparison.OrdinalIgnoreCase))
        {
            break;
        }

        if (input.Equals("help", StringComparison.OrdinalIgnoreCase))
        {
            DocumentRepl.WypiszPomoc(przykladDoc, foundryDostepny);
            continue;
        }

        if (DocumentRepl.ProbujeParsowacWyszukiwanie(input, out var keyword))
        {
            await ConversationRepl.WypiszWynikiWyszukiwaniaAsync(conversationRepo, keyword);
        }
        else if (input.Equals("history", StringComparison.OrdinalIgnoreCase))
        {
            await ConversationRepl.WypiszHistorieDzisAsync(conversationRepo);
        }
        else if (!foundryDostepny || processor is null || replPipeline is null)
        {
            Console.WriteLine("Ta komenda wymaga Foundry Local. Dostepne: search, history, help, quit.");
        }
        else if (input.StartsWith("load ", StringComparison.OrdinalIgnoreCase))
        {
            var path = input[5..].Trim().Trim('"');
            if (!File.Exists(path))
            {
                Console.WriteLine($"Plik nie istnieje: {path}");
            }
            else if (path.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            {
                processor.LoadPdfDocument(path);
            }
            else
            {
                processor.LoadDocument(await File.ReadAllTextAsync(path));
            }
        }
        else if (input.Equals("summary", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("\nGenerowanie streszczenia...\n");

            try
            {
                await conversationRepo.AddAsync("user", "summary");
                var rawSummary = await processor.SummarizeAsync();
                var processedSummary = replPipeline(rawSummary);
                await conversationRepo.AddAsync("assistant", processedSummary);
                Console.WriteLine(processedSummary);
            }
            catch (HttpRequestException ex) when (ex.Message.Contains("out of memory", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine(
                    "Blad GPU: brak pamieci VRAM. Zaladuj mniejszy model " +
                    "(np. phi-3-mini) lub ustaw FOUNDRY_MODEL na model CPU.");
                Console.WriteLine(ex.Message);
            }
        }
        else if (input.StartsWith("ask ", StringComparison.OrdinalIgnoreCase))
        {
            var question = input[4..].Trim();
            Console.WriteLine("\nSzukam odpowiedzi...\n");

            try
            {
                await conversationRepo.AddAsync("user", question);
                var rawAnswer = await processor.AskQuestionAsync(question);
                var processedAnswer = replPipeline(rawAnswer);
                await conversationRepo.AddAsync("assistant", processedAnswer);
                Console.WriteLine(processedAnswer);
            }
            catch (HttpRequestException ex) when (ex.Message.Contains("out of memory", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine(
                    "Blad GPU: brak pamieci VRAM. Zaladuj mniejszy model " +
                    "(np. phi-3-mini) lub ustaw FOUNDRY_MODEL na model CPU.");
                Console.WriteLine(ex.Message);
            }
        }
        else
        {
            Console.WriteLine("Nieznana komenda. Wpisz: help");
        }

        Console.WriteLine();
    }
}

public record WynikWalidacji(bool Poprawny, string? Komunikat = null)
{
    public static WynikWalidacji Ok => new(true);

    public static WynikWalidacji Blad(string komunikat) => new(false, komunikat);
}

public static class Walidator
{
    public static Func<string, WynikWalidacji> Wszystkie(
        params Func<string, WynikWalidacji>[] reguly) =>
        value =>
        {
            foreach (var regula in reguly)
            {
                var wynik = regula(value);

                if (!wynik.Poprawny)
                {
                    return wynik;
                }
            }

            return WynikWalidacji.Ok;
        };

    public static bool Waliduj(
        string wiadomosc,
        Func<string, WynikWalidacji> walidacja,
        Action<string, string> zbierzBlad)
    {
        var wynik = walidacja(wiadomosc);

        if (wynik.Poprawny)
        {
            return true;
        }

        zbierzBlad(wiadomosc, wynik.Komunikat ?? "Nieznany blad walidacji.");
        return false;
    }
}

public class CachedAiService
{
    private readonly IAiProvider _provider;
    private readonly Dictionary<string, string> _cache = [];
    private readonly Dictionary<string, DateTime> _timestamps = [];
    private readonly TimeSpan _ttl = TimeSpan.FromMinutes(5);

    public CachedAiService(IAiProvider provider) => _provider = provider;

    public ValueTask<string> AskAsync(string prompt, CancellationToken ct = default)
    {
        if (_cache.TryGetValue(prompt, out var cached)
            && DateTime.Now - _timestamps[prompt] < _ttl)
        {
            Console.WriteLine("[CACHE HIT]");
            return ValueTask.FromResult(cached);
        }

        Console.WriteLine("[CACHE MISS]");
        return new ValueTask<string>(FetchAndCacheAsync(prompt, ct));
    }

    private async Task<string> FetchAndCacheAsync(string prompt, CancellationToken ct)
    {
        var result = await _provider.GenerateAsync(prompt, ct);
        _cache[prompt] = result;
        _timestamps[prompt] = DateTime.Now;

        return result;
    }
}

public static class AiParallelPipeline
{
    public static async Task<List<(string Dostawca, string Wynik)>> ZapytajWszystkichAsync(
        IAiProvider[] providers,
        string prompt,
        TimeSpan timeout)
    {
        using var cts = new CancellationTokenSource(timeout);

        var tasks = providers.Select(async provider =>
        {
            try
            {
                var wynik = await provider.GenerateAsync(prompt, cts.Token);
                return (Dostawca: provider.Name, Wynik: wynik);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine($"[TIMEOUT] {provider.Name}");
                return (Dostawca: provider.Name, Wynik: (string?)null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[BLAD] {provider.Name}: {ex.Message}");
                return (Dostawca: provider.Name, Wynik: (string?)null);
            }
        });

        var results = await Task.WhenAll(tasks);

        return results
            .Where(result => result.Wynik is not null)
            .Select(result => (result.Dostawca, result.Wynik!))
            .ToList();
    }
}

public readonly struct Result<T>
{
    private readonly T? _wartosc;
    private readonly string? _blad;

    public bool Sukces { get; }

    private Result(T wartosc)
    {
        _wartosc = wartosc;
        _blad = null;
        Sukces = true;
    }

    private Result(string blad)
    {
        _wartosc = default;
        _blad = blad;
        Sukces = false;
    }

    public static Result<T> Ok(T wartosc) => new(wartosc);

    public static Result<T> Blad(string komunikat) => new(komunikat);

    public Result<TOut> Map<TOut>(Func<T, TOut> f) =>
        Sukces
            ? Result<TOut>.Ok(f(_wartosc!))
            : Result<TOut>.Blad(_blad!);

    public Result<TOut> Bind<TOut>(Func<T, Result<TOut>> f) =>
        Sukces
            ? f(_wartosc!)
            : Result<TOut>.Blad(_blad!);

    public TResult Match<TResult>(
        Func<T, TResult> naSukces,
        Func<string, TResult> naBlad) =>
        Sukces
            ? naSukces(_wartosc!)
            : naBlad(_blad!);
}

public static class PipelineOdpowiedzi
{
    public static Result<string> WalidujOdpowiedz(string odpowiedz) =>
        string.IsNullOrWhiteSpace(odpowiedz)
            ? Result<string>.Blad("Model zwrocil pusta odpowiedz.")
            : Result<string>.Ok(odpowiedz);

    public static Result<string> ObetnijDoLimitu(string tekst) =>
        Result<string>.Ok(tekst.Length > 1000
            ? tekst[..1000] + "..."
            : tekst);

    public static string DodajMetadane(string tekst) =>
        $"[{DateTime.Now:HH:mm:ss}] {tekst.Trim()}";

    public static string Przetworz(string surowaOdpowiedz) =>
        WalidujOdpowiedz(surowaOdpowiedz)
            .Bind(ObetnijDoLimitu)
            .Map(DodajMetadane)
            .Match(
                naSukces: tekst => tekst,
                naBlad: blad => $"BLAD: {blad}");
}

public class ConversationSession
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public string ModelName { get; set; } = "deepseek-r1-7b";

    public List<ChatEntry> Entries { get; set; } = [];
}

public class ChatEntry
{
    public int Id { get; set; }

    public string Role { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public DateTime Timestamp { get; set; } = DateTime.Now;

    public int SessionId { get; set; }

    public ConversationSession Session { get; set; } = null!;
}

public class ChatDbContext : DbContext
{
    public DbSet<ConversationSession> Sessions => Set<ConversationSession>();

    public DbSet<ChatEntry> Entries => Set<ChatEntry>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
        optionsBuilder.UseSqlite("Data Source=chat-history.db");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ConversationSession>(entity =>
        {
            entity.HasKey(session => session.Id);
            entity.Property(session => session.Title)
                .HasMaxLength(200)
                .IsRequired();
            entity.Property(session => session.ModelName)
                .HasMaxLength(50);
        });

        modelBuilder.Entity<ChatEntry>(entity =>
        {
            entity.HasKey(entry => entry.Id);
            entity.Property(entry => entry.Role)
                .HasMaxLength(20)
                .IsRequired();
            entity.Property(entry => entry.Content)
                .IsRequired();
            entity.HasOne(entry => entry.Session)
                .WithMany(session => session.Entries)
                .HasForeignKey(entry => entry.SessionId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(entry => new { entry.SessionId, entry.Timestamp });
        });
    }
}

public class ChatHistoryService
{
    private readonly ChatDbContext _db;

    public ChatHistoryService(ChatDbContext db) => _db = db;

    public async Task<ConversationSession> StartSessionAsync(
        string title,
        string model = "deepseek-r1-7b")
    {
        var session = new ConversationSession
        {
            Title = title,
            ModelName = model
        };

        _db.Sessions.Add(session);
        await _db.SaveChangesAsync();

        return session;
    }

    public async Task AddEntryAsync(int sessionId, string role, string content)
    {
        _db.Entries.Add(new ChatEntry
        {
            SessionId = sessionId,
            Role = role,
            Content = content
        });

        await _db.SaveChangesAsync();
    }

    public async Task<ConversationSession?> GetSessionAsync(int sessionId) =>
        await _db.Sessions
            .Include(session => session.Entries.OrderBy(entry => entry.Timestamp))
            .FirstOrDefaultAsync(session => session.Id == sessionId);
}

public interface IAiProvider
{
    string Name { get; }

    bool IsAvailable();

    Task<string> GenerateAsync(string prompt, CancellationToken ct = default);
}

public class MockProvider : IAiProvider
{
    public string Name => "Mock (testowy)";

    public bool IsAvailable() => true;

    public Task<string> GenerateAsync(string prompt, CancellationToken ct = default)
    {
        var response = prompt.Length switch
        {
            < 20 => "Krotki prompt - krotka odpowiedz.",
            < 100 => "Sredni prompt - standardowa odpowiedz.",
            _ => "Dlugi prompt - szczegolowa odpowiedz."
        };

        return Task.FromResult(response);
    }
}

public class FoundryProvider(FoundryLocalClient client) : IAiProvider
{
    public string Name => "Foundry Local";

    public bool IsAvailable() =>
        client.IsAvailableAsync().GetAwaiter().GetResult();

    public Task<string> GenerateAsync(string prompt, CancellationToken ct = default) =>
        client.ChatAsync(prompt, ct);
}

public record ChatRequest
{
    [JsonPropertyName("model")]
    public string Model { get; init; } = "deepseek-r1-7b";

    [JsonPropertyName("messages")]
    public ChatMessage[] Messages { get; init; } = [];

    [JsonPropertyName("max_tokens")]
    public int MaxTokens { get; init; } = 4096;

    [JsonPropertyName("temperature")]
    public float Temperature { get; init; } = 0.7f;

    [JsonPropertyName("stream")]
    public bool Stream { get; init; }
}

public record ChatMessage(
    [property: JsonPropertyName("role")] string Role,
    [property: JsonPropertyName("content")] string Content);

public record ChatResponse
{
    [JsonPropertyName("choices")]
    public ChatChoice[] Choices { get; init; } = [];
}

public record ChatChoice
{
    [JsonPropertyName("message")]
    public ChatMessage Message { get; init; } = new("", "");

    [JsonPropertyName("delta")]
    public ChatMessage? Delta { get; init; }
}

public record ModelsListResponse
{
    [JsonPropertyName("data")]
    public ModelInfo[] Data { get; init; } = [];
}

public record ModelInfo
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = "";
}

public static class FoundryUrlResolver
{
    private static readonly Regex ServiceUrlPattern =
        new(@"http://127\.0\.0\.1:\d+", RegexOptions.Compiled);

    public static async Task<string> ResolveAsync()
    {
        var fromCli = TryResolveFromCli();
        if (fromCli is not null)
        {
            return fromCli;
        }

        using var probe = new HttpClient { Timeout = TimeSpan.FromSeconds(2) };

        foreach (var port in new[] { 59074, 56155, 49743, 5272 })
        {
            var candidate = $"http://127.0.0.1:{port}";

            try
            {
                using var response = await probe.GetAsync($"{candidate}/v1/models");
                if (response.IsSuccessStatusCode)
                {
                    return candidate;
                }
            }
            catch
            {
                // probuj kolejny port
            }
        }

        return "http://127.0.0.1:5272";
    }

    private static string? TryResolveFromCli()
    {
        try
        {
            using var process = Process.Start(new ProcessStartInfo
            {
                FileName = "foundry",
                Arguments = "service status",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            });

            if (process is null)
            {
                return null;
            }

            var output = process.StandardOutput.ReadToEnd();
            process.WaitForExit(5000);

            var match = ServiceUrlPattern.Match(output);
            return match.Success ? match.Value : null;
        }
        catch
        {
            return null;
        }
    }
}

public class FoundryLocalClient(HttpClient client)
{
    private string? _modelId;

    public async Task<bool> IsAvailableAsync(CancellationToken ct = default)
    {
        try
        {
            using var response = await client.GetAsync("/v1/models", ct);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    private async Task<string> ResolveModelIdAsync(CancellationToken ct)
    {
        if (_modelId is not null)
        {
            return _modelId;
        }

        var envModel = Environment.GetEnvironmentVariable("FOUNDRY_MODEL");
        if (!string.IsNullOrWhiteSpace(envModel))
        {
            _modelId = envModel;
            Console.WriteLine($"Uzyty model (FOUNDRY_MODEL): {_modelId}");
            return _modelId;
        }

        using var response = await client.GetAsync("/v1/models", ct);
        await EnsureSuccessOrThrowAsync(response, ct);

        var models = await response.Content.ReadFromJsonAsync<ModelsListResponse>(ct);
        var available = models?.Data.Select(model => model.Id).ToArray() ?? [];

        if (available.Length == 0)
        {
            throw new InvalidOperationException(
                "Brak zaladowanego modelu. Uruchom: foundry model load deepseek-r1-7b");
        }

        Console.WriteLine("Dostepne modele: " + string.Join(", ", available));

        _modelId = available.FirstOrDefault(id =>
            id.Contains("deepseek", StringComparison.OrdinalIgnoreCase))
            ?? available.FirstOrDefault(id =>
                id.Contains("phi", StringComparison.OrdinalIgnoreCase))
            ?? available.First();

        Console.WriteLine($"Uzyty model: {_modelId}");
        return _modelId;
    }

    private static async Task EnsureSuccessOrThrowAsync(
        HttpResponseMessage response,
        CancellationToken ct)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var body = await response.Content.ReadAsStringAsync(ct);
        throw new HttpRequestException(
            $"Foundry API blad {(int)response.StatusCode}: {body}");
    }

    public async Task<string> ChatAsync(string prompt, CancellationToken ct = default) =>
        await ChatWithSystemAsync(
            "Odpowiadaj tresciwie i zwiezle po polsku.",
            prompt,
            maxTokens: 400,
            temperature: 0.3f,
            ct);

    public async Task<string> ChatWithSystemAsync(
        string systemPrompt,
        string userMessage,
        int maxTokens = 2048,
        float temperature = 0.3f,
        CancellationToken ct = default)
    {
        var modelId = await ResolveModelIdAsync(ct);

        var request = new ChatRequest
        {
            Model = modelId,
            Messages =
            [
                new("system", systemPrompt),
                new("user", userMessage)
            ],
            MaxTokens = maxTokens,
            Temperature = temperature
        };

        using var response = await client.PostAsJsonAsync(
            "/v1/chat/completions",
            request,
            ct);

        await EnsureSuccessOrThrowAsync(response, ct);

        var result = await response.Content.ReadFromJsonAsync<ChatResponse>(ct);
        return result?.Choices.FirstOrDefault()?.Message.Content ?? "(brak odpowiedzi)";
    }

    public async Task StreamChatAsync(string prompt, CancellationToken ct = default)
    {
        var modelId = await ResolveModelIdAsync(ct);

        var streamRequest = new HttpRequestMessage(HttpMethod.Post, "/v1/chat/completions")
        {
            Content = JsonContent.Create(new ChatRequest
            {
                Model = modelId,
                Messages = [new("user", prompt)],
                Stream = true,
                MaxTokens = 400,
                Temperature = 0.3f
            })
        };

        using var response = await client.SendAsync(
            streamRequest,
            HttpCompletionOption.ResponseHeadersRead,
            ct);

        await EnsureSuccessOrThrowAsync(response, ct);

        await using var stream = await response.Content.ReadAsStreamAsync(ct);
        using var reader = new StreamReader(stream);

        while (await reader.ReadLineAsync(ct) is { } line)
        {
            if (!line.StartsWith("data: ") || line == "data: [DONE]")
            {
                continue;
            }

            var json = line[6..];
            var chunk = JsonSerializer.Deserialize<ChatResponse>(json);
            var choice = chunk?.Choices.FirstOrDefault();
            var content = choice?.Delta?.Content ?? choice?.Message.Content;

            if (!string.IsNullOrEmpty(content))
            {
                Console.Write(content);
            }
        }
    }
}

public class CompositeProvider(IEnumerable<IAiProvider> providers) : IAiProvider
{
    public string Name => "Composite (auto-selekcja)";

    public bool IsAvailable() => providers.Any(provider => provider.IsAvailable());

    public async Task<string> GenerateAsync(string prompt, CancellationToken ct = default)
    {
        var active = providers.FirstOrDefault(provider => provider.IsAvailable())
            ?? throw new InvalidOperationException("Brak dostepnego dostawcy AI.");

        Console.WriteLine($"-> Wybrany dostawca: {active.Name}");
        return await active.GenerateAsync(prompt, ct);
    }
}

public class AiContext
{
    private IAiProvider _provider;

    public AiContext(IAiProvider provider) =>
        _provider = provider
        ?? throw new ArgumentNullException(nameof(provider));

    public void SetProvider(IAiProvider provider)
    {
        _provider = provider;
        Console.WriteLine($"-> Przelaczono na: {_provider.Name}");
    }

    public async Task<string> AskAsync(string prompt, CancellationToken ct = default)
    {
        Console.WriteLine($"[{_provider.Name}] Pytam...");
        return await _provider.GenerateAsync(prompt, ct);
    }
}

public interface IAiBackend
{
    string Name { get; }

    bool IsAvailable();

    Task<string> GenerateAsync(string prompt, CancellationToken ct = default);
}

public class MockAiBackend : IAiBackend
{
    public string Name => "Mock";

    public bool IsAvailable() => true;

    public Task<string> GenerateAsync(string prompt, CancellationToken ct = default) =>
        Task.FromResult($"[Mock] Odpowiedz na: {prompt}");
}

public class FoundryAiBackend(FoundryLocalClient client) : IAiBackend
{
    public string Name => "Foundry Local";

    public bool IsAvailable()
    {
        try
        {
            return client.IsAvailableAsync().GetAwaiter().GetResult();
        }
        catch
        {
            return false;
        }
    }

    public Task<string> GenerateAsync(string prompt, CancellationToken ct = default) =>
        client.ChatAsync(prompt, ct);
}

public class AiRouter(IEnumerable<IAiBackend> backends)
{
    private readonly IEnumerable<IAiBackend> _backends = backends;

    public IAiBackend SelectBackend()
    {
        return _backends.FirstOrDefault(backend => backend.IsAvailable())
            ?? throw new InvalidOperationException("Brak dostepnego backendu AI.");
    }

    public Task<string> GenerateAsync(string prompt, CancellationToken ct = default) =>
        SelectBackend().GenerateAsync(prompt, ct);

    public List<string> GetCapabilityReport() =>
        _backends
            .Select(backend => backend.IsAvailable()
                ? $"[ OK ] {backend.Name}"
                : $"[ NIEDOSTEPNY ] {backend.Name}")
            .ToList();
}

public class ConversationEntry
{
    public int Id { get; set; }

    public required string Role { get; set; }

    public required string Content { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

public class AiDbContext : DbContext
{
    public DbSet<ConversationEntry> Entries => Set<ConversationEntry>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
        optionsBuilder.UseSqlite("Data Source=conversation.db");
}

public class ConversationRepository
{
    private readonly AiDbContext _ctx;

    public ConversationRepository(AiDbContext ctx) => _ctx = ctx;

    public async Task<List<ConversationEntry>> GetByDateRange(DateTime from, DateTime to) =>
        await _ctx.Entries
            .Where(entry => entry.CreatedAt >= from && entry.CreatedAt <= to)
            .ToListAsync();

    public async Task<List<ConversationEntry>> GetByKeyword(string keyword) =>
        await _ctx.Entries
            .Where(entry => entry.Content.ToLower().Contains(keyword.ToLower()))
            .ToListAsync();

    public async Task AddAsync(string role, string content)
    {
        _ctx.Entries.Add(new ConversationEntry
        {
            Role = role,
            Content = content
        });

        await _ctx.SaveChangesAsync();
    }
}

public static class AiResponsePipeline
{
    public static Func<string, string> Create(bool logToConsole = true)
    {
        Func<string, string> trim = s => s.Trim();
        Func<string, string> addPrefix = s => $"[AI] {s}";
        Func<string, string> addTime = s => $"{s} ({DateTime.Now:HH:mm:ss})";
        Func<string, string> logResult = logToConsole
            ? s =>
            {
                Console.WriteLine(s);
                return s;
            }
            : s => s;

        return s => logResult(addTime(addPrefix(trim(s))));
    }
}

public class LocalAiService(FoundryLocalClient client)
{
    public Task<string> ChatAsync(
        string systemPrompt,
        string userMessage,
        int maxTokens = 2048,
        float temperature = 0.3f,
        CancellationToken ct = default) =>
        client.ChatWithSystemAsync(systemPrompt, userMessage, maxTokens, temperature, ct);
}

public static class ConversationRepl
{
    public static async Task WypiszWynikiWyszukiwaniaAsync(
        ConversationRepository repo,
        string keyword)
    {
        var wyniki = await repo.GetByKeyword(keyword);
        Console.WriteLine($"Znaleziono {wyniki.Count} wpis(y) dla frazy \"{keyword}\":");

        foreach (var wpis in wyniki)
        {
            Console.WriteLine($"  [{wpis.CreatedAt:HH:mm:ss}] {wpis.Role}: {wpis.Content}");
        }
    }

    public static async Task WypiszHistorieDzisAsync(ConversationRepository repo)
    {
        var od = DateTime.Today;
        var doDaty = DateTime.Today.AddDays(1).AddTicks(-1);
        var wyniki = await repo.GetByDateRange(od, doDaty);
        Console.WriteLine($"Historia z dnia {od:yyyy-MM-dd}: {wyniki.Count} wpis(y)");

        foreach (var wpis in wyniki)
        {
            Console.WriteLine($"  [{wpis.CreatedAt:HH:mm:ss}] {wpis.Role}: {wpis.Content}");
        }
    }
}

public static class DocumentRepl
{
    public static string OczyscWejscie(string? raw)
    {
        if (raw is null)
        {
            return string.Empty;
        }

        var input = raw.Trim();
        while (input.StartsWith('>'))
        {
            input = input[1..].TrimStart();
        }

        return input;
    }

    public static bool CzyPominacWejscie(string input) =>
        string.IsNullOrWhiteSpace(input);

    public static bool ProbujeParsowacWyszukiwanie(string input, out string keyword)
    {
        keyword = string.Empty;

        if (input.StartsWith("search ", StringComparison.OrdinalIgnoreCase))
        {
            keyword = input[7..].Trim();
        }
        else
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(keyword))
        {
            Console.WriteLine("Podaj slowo kluczowe: search <fraza>");
            return false;
        }

        return true;
    }

    public static void WypiszPomoc(string przykladSciezka, bool pelnyRepl = true)
    {
        Console.WriteLine("Dostepne komendy:");
        Console.WriteLine("  search <fraza>   - wyszukaj wpisy po slowie kluczowym (GetByKeyword)");
        Console.WriteLine("  history          - wpisy z dzisiejszego dnia (GetByDateRange)");

        if (pelnyRepl)
        {
            Console.WriteLine("  load <sciezka>   - wczytaj .txt lub .pdf");
            Console.WriteLine("  summary          - streszczenie dokumentu (zapis do bazy)");
            Console.WriteLine("  ask <pytanie>    - pytanie o dokument (zapis do bazy)");
            Console.WriteLine($"  Przyklad: load {przykladSciezka}");
        }

        Console.WriteLine("  help             - ta lista");
        Console.WriteLine("  quit / exit      - wyjscie");
        Console.WriteLine("  Przyklad: search inferencja");
    }
}

public class DocumentProcessor(LocalAiService ai)
{
    private string _loadedDocument = string.Empty;

    public bool HasDocument => _loadedDocument.Length > 0;

    public void LoadDocument(string text)
    {
        _loadedDocument = text;
        Console.WriteLine($"Zaladowano dokument ({text.Length} zn.)");
    }

    public void LoadPdfDocument(string path)
    {
        using var pdf = PdfDocument.Open(path);
        var text = string.Join("\n", pdf.GetPages().Select(p => p.Text));
        _loadedDocument = text;
        Console.WriteLine($"Zaladowano PDF ({pdf.NumberOfPages} str., {text.Length} zn.)");
    }

    public async Task<string> SummarizeAsync(CancellationToken ct = default)
    {
        if (!HasDocument)
        {
            return "Najpierw zaladuj dokument.";
        }

        const string systemPrompt =
            "Jestes asystentem jezyka polskiego. Streszczaj dokumenty w 3-5 punktach. " +
            "Calosc odpowiedzi MUSI byc wylacznie po polsku. Nie uzywaj jezyka angielskiego.";

        var text = _loadedDocument.Length > 3000
            ? _loadedDocument[..3000] + "..."
            : _loadedDocument;

        var userMessage = $"Streszcz po polsku nastepujacy dokument:\n\n{text}";
        return await ai.ChatAsync(systemPrompt, userMessage, maxTokens: 400, ct: ct);
    }

    public async Task<string> AskQuestionAsync(string question, CancellationToken ct = default)
    {
        if (!HasDocument)
        {
            return "Najpierw zaladuj dokument.";
        }

        const string systemPrompt =
            "Jestes asystentem prawnym. Odpowiadaj na pytania wylacznie na podstawie " +
            "podanego dokumentu. Jesli odpowiedz nie wynika z dokumentu, powiedz o tym. " +
            "Calosc odpowiedzi MUSI byc wylacznie po polsku.";

        var context = _loadedDocument.Length > 2500
            ? _loadedDocument[..2500] + "..."
            : _loadedDocument;

        var userMessage = $"Dokument:\n{context}\n\nPytanie: {question}";
        return await ai.ChatAsync(systemPrompt, userMessage, maxTokens: 300, ct: ct);
    }
}

public static class OnnxMobilenetDemo
{
    public static void Run(string modelPath)
    {
        using var session = new InferenceSession(modelPath);

        Console.WriteLine("=== Wejscia modelu ===");
        foreach (var input in session.InputMetadata)
        {
            var dims = string.Join(", ", input.Value.Dimensions);
            Console.WriteLine($"{input.Key}: [{dims}] ({input.Value.ElementType})");
        }

        Console.WriteLine("=== Wyjscia modelu ===");
        foreach (var output in session.OutputMetadata)
        {
            var dims = string.Join(", ", output.Value.Dimensions);
            Console.WriteLine($"{output.Key}: [{dims}] ({output.Value.ElementType})");
        }

        var inputName = session.InputMetadata.Keys.First();
        var inputTensor = new DenseTensor<float>(new[] { 1, 3, 224, 224 });
        var rng = new Random(42);

        for (int c = 0; c < 3; c++)
        {
            for (int h = 0; h < 224; h++)
            {
                for (int w = 0; w < 224; w++)
                {
                    inputTensor[0, c, h, w] = (float)(rng.NextDouble() * 2 - 1);
                }
            }
        }

        var inputs = new List<NamedOnnxValue>
        {
            NamedOnnxValue.CreateFromTensor(inputName, inputTensor)
        };

        var swCpu = Stopwatch.StartNew();
        using (var resultsCpu = session.Run(inputs))
        {
            var outputCpu = resultsCpu.First().AsEnumerable<float>().ToArray();
            swCpu.Stop();
            Console.WriteLine($"CPU: {swCpu.ElapsedMilliseconds} ms");
            WypiszTop5(outputCpu);
        }

        var directMlDostepny = false;
        InferenceSession? gpuSession = null;

        try
        {
            var opts = new SessionOptions();
            opts.AppendExecutionProvider_DML(0);
            gpuSession = new InferenceSession(modelPath, opts);
            directMlDostepny = true;
            Console.WriteLine("\nDirectML dostepny - uzywam GPU");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nDirectML niedostepny: {ex.Message}");
        }

        if (directMlDostepny && gpuSession is not null)
        {
            var swGpu = Stopwatch.StartNew();
            using (var resultsGpu = gpuSession.Run(inputs))
            {
                _ = resultsGpu.First().AsEnumerable<float>().ToArray();
                swGpu.Stop();
                Console.WriteLine($"DirectML (GPU): {swGpu.ElapsedMilliseconds} ms");
            }

            var cpu10 = Bench(session, inputs, 10);
            var gpu10 = Bench(gpuSession, inputs, 10);

            Console.WriteLine("\nBenchmark 10 iteracji:");
            Console.WriteLine($"CPU: {cpu10} ms (sr. {cpu10 / 10.0:F1} ms/iter)");
            Console.WriteLine($"DirectML (GPU): {gpu10} ms (sr. {gpu10 / 10.0:F1} ms/iter)");

            if (gpu10 > 0)
            {
                Console.WriteLine($"Przyspieszenie: {(double)cpu10 / gpu10:F1}x");
            }

            gpuSession.Dispose();
        }
        else
        {
            var cpu10 = Bench(session, inputs, 10);
            Console.WriteLine("\nBenchmark 10 iteracji (tylko CPU):");
            Console.WriteLine($"CPU: {cpu10} ms (sr. {cpu10 / 10.0:F1} ms/iter)");
        }
    }

    private static void WypiszTop5(float[] output)
    {
        var top5 = output
            .Select((score, index) => (Score: score, Index: index))
            .OrderByDescending(x => x.Score)
            .Take(5);

        Console.WriteLine("Top-5 klas:");
        foreach (var (score, index) in top5)
        {
            Console.WriteLine($"Klasa {index}: {score:F4}");
        }
    }

    private static long Bench(InferenceSession session, List<NamedOnnxValue> inputs, int iterations)
    {
        var sw = Stopwatch.StartNew();

        for (int i = 0; i < iterations; i++)
        {
            using var results = session.Run(inputs);
            _ = results.First().AsEnumerable<float>().ToArray();
        }

        sw.Stop();
        return sw.ElapsedMilliseconds;
    }
}
