using ExpiringCacheDemo;

var cache = new ExpiringCache<string, string>(TimeSpan.FromSeconds(2));

cache.Set("session", "abc-123");
Console.WriteLine($"Get przed wygasnieciem: {cache.Get("session")}");

Console.WriteLine("Czekam 3 sekundy...");
Thread.Sleep(3000);

Console.WriteLine($"Get po wygasnieciu: {cache.Get("session") ?? "null"}");
Console.WriteLine($"Remove nieistniejacego: {cache.Remove("brak")}");
