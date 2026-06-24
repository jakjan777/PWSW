using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.Chat;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Polly;
using PWSW5;

// Walidacja i maskowanie IBAN
string iban = "PL61109010140000071219812874";
Console.WriteLine($"IBAN poprawny: {iban.JestPoprawnymIBAN()}");
Console.WriteLine($"IBAN sformatowany: {iban.FormatujIBAN()}");
Console.WriteLine($"IBAN zamaskowany: {iban.Zamaskuj(4)}");
Console.WriteLine();

var logi = new List<LogZdarzenia>
{
    new("Uruchomienie agenta", new(2026, 3, 1, 9, 0, 0), "System"),
    new("Wywolanie pluginu FilePlugin", new(2026, 3, 1, 9, 5, 0), "Plugin"),
    new("Blad HTTP 503", new(2026, 3, 2, 10, 0, 0), "Blad"),
    new("Wywolanie SearchKnowledge", new(2026, 3, 3, 11, 0, 0), "Plugin"),
};

// Fluent API -- lancuchowanie metod rozszerzajacych
var raport = logi
    .ZOkresu(new(2026, 3, 1), new(2026, 3, 3))
    .ZKategorii("Plugin")
    .JakoRaport();

Console.WriteLine($"Raport pluginow: {raport}");

// Porcjowanie logow do wysylki
Console.WriteLine("Logi w porcjach po 2:");
foreach (var porcja in logi.Porcjuj(2))
    Console.WriteLine($"[{string.Join(", ", porcja.Select(l => l.Opis))}]");

Console.WriteLine();

var uslugi = new ServiceCollection();
uslugi.AddTransient<IAgentPlugin, FilePlugin>();
uslugi.AddTransient<IAgentPlugin, WeatherPlugin>();
uslugi.AddSingleton<AgentOrchestrator>();

var provider = uslugi.BuildServiceProvider();

var orchestrator = provider.GetRequiredService<AgentOrchestrator>();
orchestrator.ListujWtyczki();

var p1 = provider.GetRequiredService<AgentOrchestrator>();
var p2 = provider.GetRequiredService<AgentOrchestrator>();
Console.WriteLine($"Singleton - ten sam obiekt? {ReferenceEquals(p1, p2)}");

var plugin1 = provider.GetRequiredService<IAgentPlugin>();
var plugin2 = provider.GetRequiredService<IAgentPlugin>();
Console.WriteLine($"Transient - ten sam obiekt? {ReferenceEquals(plugin1, plugin2)}");

Console.WriteLine();

var uslugiPogoda = new ServiceCollection();
uslugiPogoda.AddHttpClient<KlientPogody>(http =>
{
    http.BaseAddress = new Uri("https://api-pogoda.example.com");
    http.DefaultRequestHeaders.Add("Accept", "application/json");
    http.DefaultRequestHeaders.Add("X-Api-Key", "klucz-demonstracyjny");
    http.Timeout = TimeSpan.FromSeconds(10);
})
.ConfigurePrimaryHttpMessageHandler(() => new SymulowanyHandlerPogody())
.AddStandardResilienceHandler(opcje =>
{
    opcje.Retry.MaxRetryAttempts = 3;
    opcje.Retry.Delay = TimeSpan.FromMilliseconds(500);
    opcje.Retry.BackoffType = DelayBackoffType.Exponential;
    opcje.Retry.OnRetry = args =>
    {
        Console.WriteLine($"Ponowienie #{args.AttemptNumber}...");
        return ValueTask.CompletedTask;
    };
    opcje.CircuitBreaker.FailureRatio = 0.5;
    opcje.CircuitBreaker.MinimumThroughput = 5;
    opcje.CircuitBreaker.BreakDuration = TimeSpan.FromSeconds(30);
});

var providerPogoda = uslugiPogoda.BuildServiceProvider();
var klientPogody = providerPogoda.GetRequiredService<KlientPogody>();

var miasta = new[] { "Warszawa", "Krakow", "Gdansk" };
var zadania = miasta.Select(m => klientPogody.PobierzAsync(m));
var wyniki = await Task.WhenAll(zadania);

foreach (var dane in wyniki.Where(d => d is not null))
    Console.WriteLine(
        $"{dane!.Miasto}: {dane.TemperaturaCelsjusz}C, " +
        $"{dane.Warunki}, wilgotnosc {dane.WilgotnoscProcent}%");

Console.WriteLine();

var logAkcji = new List<string>();
var history = new CommandHistory();

history.ExecuteCommand(new LogActionCommand("Wywolano FilePlugin", logAkcji));
history.ExecuteCommand(new LogActionCommand("Wywolano WeatherPlugin", logAkcji));

var makro = new MacroCommand("Pelny cykl raportu",
    new LogActionCommand("SearchKnowledge", logAkcji),
    new LogActionCommand("GenerateReport", logAkcji));
history.ExecuteCommand(makro);

Console.WriteLine($"Log ma {logAkcji.Count} wpisow.");
history.Undo();
Console.WriteLine($"Po cofnieciu makra: {logAkcji.Count} wpisow.");

Console.WriteLine();

var innerHandler = new PluginCallHandler();
var logging = new LoggingBehavior<PluginCallRequest, PluginCallResponse>();

var request = new PluginCallRequest("WeatherPlugin", "Pogoda w Warszawie");
var result = await logging.HandleAsync(
    request,
    () => innerHandler.HandleAsync(request, default),
    default);

Console.WriteLine($"Wynik: Success={result.Success}, {result.Result} ({result.ElapsedMs} ms)");

Console.WriteLine();

Directory.CreateDirectory(@"C:\Temp");
await File.WriteAllTextAsync(@"C:\Temp\test-agent.txt", "Plik testowy asystenta IT.");

var builder = Kernel.CreateBuilder();
builder.AddOpenAIChatCompletion(
    modelId: "Phi-3-mini-4k-instruct-cuda-gpu:2",
    endpoint: new Uri("http://127.0.0.1:49578/v1"),
    apiKey: "not-needed");

var kernel = builder.Build();
kernel.Plugins.AddFromType<FilePlugin>();

Console.WriteLine("Funkcje FilePlugin:");
foreach (var fn in kernel.Plugins["FilePlugin"])
    Console.WriteLine($"  {fn.Name}: {fn.Description}");

try
{
    var odpowiedz = await kernel.InvokePromptAsync(
        "Czym jest Semantic Kernel? Odpowiedz jednym zdaniem po polsku.");
    Console.WriteLine($"Test polaczenia: {odpowiedz.GetValue<string>()}");

    var ustawienia = new OpenAIPromptExecutionSettings
    {
        FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
    };

    var wynikPlugin = await kernel.InvokePromptAsync(
        "Jakie pliki .txt sa w C:\\Temp? Wypisz nazwy.",
        new KernelArguments(ustawienia));
    Console.WriteLine($"Auto calling FilePlugin: {wynikPlugin.GetValue<string>()}");
}
catch (HttpOperationException ex)
{
    Console.WriteLine("Nie udalo sie polaczyc z modelem AI.");
    Console.WriteLine("Uruchom Foundry: foundry model load Phi-3-mini-4k-instruct-cuda-gpu:2");
    Console.WriteLine("Sprawdz port serwisu i dopasuj go w Program.cs.");
    Console.WriteLine($"Szczegoly: {ex.Message}");
}

var listFiles = kernel.Plugins.GetFunction("FilePlugin", "ListFiles");
var pliki = await kernel.InvokeAsync(listFiles, new KernelArguments
{
    ["path"] = @"C:\Temp",
    ["filter"] = "*.txt"
});
var nazwyPlikow = pliki.GetValue<string[]>() ?? [];
Console.WriteLine($"ListFiles zwraca: {string.Join(", ", nazwyPlikow)}");

Console.WriteLine();

kernel.Plugins.AddFromType<WeatherPlugin>();

Console.WriteLine("Funkcje po dodaniu WeatherPlugin:");
foreach (var plugin in kernel.Plugins)
    foreach (var fn in plugin)
        Console.WriteLine($"  [{plugin.Name}] {fn.Name}: {fn.Description}");

try
{
    var ustawieniaAuto = new OpenAIPromptExecutionSettings
    {
        FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
    };

    var wynikAuto = await kernel.InvokePromptAsync(
        "Jakie pliki .txt sa w C:\\Temp? Jaka jest pogoda w Krakowie?",
        new KernelArguments(ustawieniaAuto));
    Console.WriteLine($"Auto calling oba pluginy:\n{wynikAuto.GetValue<string>()}");
}
catch (HttpOperationException ex)
{
    Console.WriteLine($"Blad auto function calling: {ex.Message}");
}

var getWeather = kernel.Plugins.GetFunction("WeatherPlugin", "GetWeather");
var pogoda = await kernel.InvokeAsync(getWeather, new KernelArguments { ["city"] = "Krakow" });
Console.WriteLine($"GetWeather zwraca: {pogoda.GetValue<string>()}");

Console.WriteLine();

Directory.CreateDirectory("knowledge-docs");

await File.WriteAllTextAsync("knowledge-docs/regulamin-urlopowy.txt", """
    Regulamin urlopowy. Kazdy pracownik ma prawo do 26 dni urlopu
    wypoczynkowego w roku. Urlop nalezy zglosic co najmniej 14 dni
    przed planowanym terminem. Wniosek skladany jest przez system HR.
    Urlop na zadanie nie wymaga wczesniejszego zgloszenia.
    Urlop niewykorzystany przenosi sie na nastepny rok, ale musi byc
    wykorzystany do 30 wrzesnia.
    """);

await File.WriteAllTextAsync("knowledge-docs/polityka-zdalna.txt", """
    Polityka pracy zdalnej. Pracownicy moga pracowac zdalnie do 3 dni
    w tygodniu. Wymagana jest obecnosc w biurze w poniedzialki i
    czwartki. Praca zdalna wymaga stabilnego lacza internetowego. Pracodawca
    zapewnia laptop i monitor. Rozliczenie kosztow internetu odbywa sie na
    podstawie ryczaltu 100 PLN miesiecznie.
    """);

var kb = new KnowledgeBase();
kb.LoadFromDirectory("./knowledge-docs/");

var wynikiKb = kb.Search("ile dni urlopu pracownik");
Console.WriteLine("Wyniki wyszukiwania:");
foreach (var chunk in wynikiKb)
    Console.WriteLine($"[{chunk.Source}] {chunk.Content[..Math.Min(100, chunk.Content.Length)]}...");

Console.WriteLine();

var builderRag = Kernel.CreateBuilder();
builderRag.AddOpenAIChatCompletion(
    modelId: "Phi-3-mini-4k-instruct-cuda-gpu:2",
    endpoint: new Uri("http://127.0.0.1:49578/v1"),
    apiKey: "not-needed");

var kernelRag = builderRag.Build();
kernelRag.Plugins.AddFromObject(new RagPlugin(kb), "RagPlugin");

var ustawieniaRag = new OpenAIPromptExecutionSettings
{
    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
};

string[] pytaniaHr =
[
    "Ile dni urlopu przysluguje pracownikowi?",
    "Ile dni mozna pracowac zdalnie w tygodniu?",
    "Czy firma pokrywa koszty internetu przy pracy zdalnej?"
];

foreach (var pytanie in pytaniaHr)
{
    Console.WriteLine($"Q: {pytanie}");

    var searchFn = kernelRag.Plugins.GetFunction("RagPlugin", "SearchKnowledge");
    var kontekst = await kernelRag.InvokeAsync(searchFn, new KernelArguments { ["query"] = pytanie });
    var tekstKontekstu = kontekst.GetValue<string>() ?? "";
    Console.WriteLine($"SearchKnowledge: {tekstKontekstu[..Math.Min(120, tekstKontekstu.Length)]}...");

    try
    {
        var prompt = $"""
            Odpowiedz na pytanie na podstawie bazy wiedzy firmy.
            Uzyj funkcji SearchKnowledge aby znalezc odpowiednie informacje.
            Jesli baza nie zawiera odpowiedzi, poinformuj o tym.

            Pytanie: {pytanie}
            """;

        var odpowiedzHr = await kernelRag.InvokePromptAsync(
            prompt,
            new KernelArguments(ustawieniaRag));
        Console.WriteLine($"A: {odpowiedzHr.GetValue<string>()}");
    }
    catch (HttpOperationException ex)
    {
        Console.WriteLine($"A: blad modelu - {ex.Message}");
    }

    Console.WriteLine();
}

Console.WriteLine();

var builderAgents = Kernel.CreateBuilder();
builderAgents.AddOpenAIChatCompletion(
    modelId: "Phi-3-mini-4k-instruct-cuda-gpu:2",
    endpoint: new Uri("http://127.0.0.1:49578/v1"),
    apiKey: "not-needed");
var kernelAgents = builderAgents.Build();

var ustawieniaAgenta = new KernelArguments(new OpenAIPromptExecutionSettings
{
    MaxTokens = 512,
    Temperature = 0.3
});

var researcher = new ChatCompletionAgent
{
    Name = "Researcher",
    Instructions = """
        Zbierasz fakty techniczne. Odpowiadasz TYLKO lista 5 punktow z myslnikiem.
        Po polsku. Bez tytulu, bez abstract, bez naglowkow.
        """,
    Kernel = kernelAgents,
    Arguments = ustawieniaAgenta
};

var writer = new ChatCompletionAgent
{
    Name = "Writer",
    Instructions = """
        Piszesz krotki raport techniczny w 3 sekcjach z naglowkami.
        Uzywaj faktow podanych w wiadomosci. Po polsku. Maksymalnie 12 zdan.
        """,
    Kernel = kernelAgents,
    Arguments = ustawieniaAgenta
};

var reviewer = new ChatCompletionAgent
{
    Name = "Reviewer",
    Instructions = """
        Oceniasz raport techniczny. Napisz 2 zdania oceny.
        Jesli raport jest OK, dodaj na koncu osobna linie ze slowem ZATWIERDZAM.
        Po polsku. Bez listy numerowanej.
        """,
    Kernel = kernelAgents,
    Arguments = ustawieniaAgenta
};

const string temat = "NativeAOT w .NET 10: zalety, ograniczenia i przypadki uzycia";

async Task<string> WywolajAgentaAsync(ChatCompletionAgent agent, string pytanie)
{
    var historia = new ChatHistory();
    historia.AddUserMessage(pytanie);
    var sb = new System.Text.StringBuilder();
    await foreach (var odpowiedz in agent.InvokeAsync(historia))
    {
        var tresc = odpowiedz.Message.Content;
        if (!string.IsNullOrWhiteSpace(tresc))
            sb.AppendLine(tresc);
    }
    return sb.ToString().Trim();
}

try
{
    Console.WriteLine("=== RESEARCHER ===");
    var fakty = await WywolajAgentaAsync(researcher,
        $"Temat: {temat}. Wypisz dokladnie 5 punktow. Kazdy punkt to jedno krotkie zdanie. " +
        "Format: myslnik i tekst. Bez tytulu, bez abstract, bez podsumowania.");
    Console.WriteLine(fakty);
    Console.WriteLine();

    Console.WriteLine("=== WRITER ===");
    var raportAgentow = await WywolajAgentaAsync(writer,
        $"Na podstawie faktow od Researchera napisz krotki raport w 3 sekcjach.\n\nFakty:\n{fakty}");
    Console.WriteLine(raportAgentow);
    Console.WriteLine();

    Console.WriteLine("=== REVIEWER ===");
    var ocena = await WywolajAgentaAsync(reviewer,
        $"Ocen ponizszy raport pod katem poprawnosci i kompletnosci.\n\nRaport:\n{raportAgentow}");
    Console.WriteLine(ocena);
    Console.WriteLine();

    Console.WriteLine("Pipeline zakonczony.");
}
catch (HttpOperationException ex)
{
    Console.WriteLine($"Blad pipeline agentow: {ex.Message}");
}

Console.WriteLine();

var logCommandHistory = new LogCommandHistory();
logCommandHistory.Log("Wywolano FilePlugin", "Plugin");
logCommandHistory.Log("Wywolano WeatherPlugin", "Plugin");
logCommandHistory.Log("Blad HTTP 503", "Blad");
logCommandHistory.Log("SearchKnowledge OK", "Plugin");
logCommandHistory.Log("GenerateReport OK", "Plugin");

var raportLogow = logCommandHistory.GetLog()
    .ZKategorii("Plugin")
    .ZOkresu(DateTime.Today, DateTime.Today.AddDays(1))
    .JakoRaport();

Console.WriteLine($"Raport logow agenta: {raportLogow}");
Console.WriteLine($"Liczba wpisow przed cofnieciem: {logCommandHistory.GetLog().Count()}");

logCommandHistory.Undo();
logCommandHistory.Undo();
Console.WriteLine($"Liczba wpisow po cofnieciu: {logCommandHistory.GetLog().Count()}");

Console.WriteLine();

var hrPlugin = new HrAssistantPlugin(kb);
var hrHandler = new HrQueryHandler(hrPlugin);
var hrLog = new List<string>();
var hrLogging = new QueryLoggingBehavior<HrQuery, HrResponse>(hrLog);

string[] pytaniaHrWyzwanie =
[
    "Ile dni urlopu przysluguje pracownikowi?",
    "Ile dni mozna pracowac zdalnie w tygodniu?",
    "Jaka jest stawka za nadgodziny?"
];

foreach (var pytanie in pytaniaHrWyzwanie)
{
    var query = new HrQuery(pytanie);
    var response = await hrLogging.HandleAsync(
        query,
        () => hrHandler.HandleAsync(query),
        default);

    Console.WriteLine($"Q: {pytanie}");
    Console.WriteLine($"Znaleziono w bazie: {response.FoundInKnowledgeBase}");
    Console.WriteLine($"A: {response.Answer}");
    Console.WriteLine();
}

Console.WriteLine("Log zapytan HR:");
foreach (var wpis in hrLog)
    Console.WriteLine(wpis);
