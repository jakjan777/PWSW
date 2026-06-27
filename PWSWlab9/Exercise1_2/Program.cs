using Exercise1_2;

Console.WriteLine("=== Cwiczenie 1.2: Wyrazenia regularne i wyszukiwanie znakow ===\n");

var logBezpieczenstwa = """
    2026-01-15 10:23:01|WARN|192.168.1.50|Brute-force na koncie admin
    2026-01-15 10:24:15|ERROR|10.0.0.12|SQL injection: SELECT * FROM users WHERE id='1' OR '1'='1'
    2026-01-15 10:25:30|WARN|203.0.113.7|Port scan wykryty z 203.0.113.7
    2026-01-15 10:26:00|FATAL|172.16.0.5|DDoS atak z wielu zrodel 172.16.0.5
    2026-01-15 10:27:11|INFO|192.168.1.50|Normalne logowanie uzytkownika
    """;

var sciezkaLogu = Path.Combine(AppContext.BaseDirectory, "security.log");
await File.WriteAllTextAsync(sciezkaLogu, logBezpieczenstwa);
Console.WriteLine($"Zapisano log bezpieczenstwa: {sciezkaLogu}\n");

var adresyIp = AnalizatorLogow.WyodrebnijAdresyIp(logBezpieczenstwa);
Console.WriteLine($"Wyodrebnione adresy IP ({adresyIp.Count}):");
foreach (var ip in adresyIp)
    Console.WriteLine($"  {ip}");

var typyAtakow = AnalizatorLogow.WyodrebnijTypyAtakow(logBezpieczenstwa);
Console.WriteLine($"\nWykryte typy atakow ({typyAtakow.Count}):");
foreach (var typ in typyAtakow)
    Console.WriteLine($"  {typ}");

Console.WriteLine("\n--- Analiza znakow specjalnych (SearchValues) ---");
foreach (var linia in logBezpieczenstwa.Split('\n', StringSplitOptions.RemoveEmptyEntries))
{
    ReadOnlySpan<char> span = linia;
    int znaki = AnalizatorLogow.ZliczZnakiSpecjalne(span);
    bool podejrzana = ParserBezAlokacji.CzyPodejrzanaLinia(span);
    Console.WriteLine($"  znaki={znaki,2} podejrzana={podejrzana,-5} | {linia[..Math.Min(60, linia.Length)]}");
}

Console.WriteLine("\n--- PoliczPoPoziomie (ReadOnlySpan, zero alokacji) ---");
ReadOnlySpan<char> logiSpan = logBezpieczenstwa;
Console.WriteLine($"  Linie z WARN: {ParserBezAlokacji.PoliczPoPoziomie(logiSpan, "WARN")}");
Console.WriteLine($"  Linie z ERROR: {ParserBezAlokacji.PoliczPoPoziomie(logiSpan, "ERROR")}");
Console.WriteLine($"  Linie z FATAL: {ParserBezAlokacji.PoliczPoPoziomie(logiSpan, "FATAL")}");

Console.WriteLine("\n=== Koniec cwiczenia 1.2 ===");
