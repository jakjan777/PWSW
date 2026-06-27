using Npgsql;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? "Host=db;Database=lab08;Username=postgres;Password=devpass";

var app = builder.Build();

await InitDatabaseAsync(connectionString);

app.MapGet("/", () => Results.Ok(new
{
    service = "lab08-web",
    message = "Cwiczenie 6.2 - Docker Compose"
}));

app.MapGet("/api/status", async () =>
{
    await using var conn = new NpgsqlConnection(connectionString);
    await conn.OpenAsync();

    await using var cmd = new NpgsqlCommand(
        "SELECT COUNT(*) FROM lab_log", conn);
    var count = (long)(await cmd.ExecuteScalarAsync() ?? 0L);

    return Results.Ok(new
    {
        status = "healthy",
        database = "lab08",
        logEntries = count
    });
});

app.Run();

static async Task InitDatabaseAsync(string connectionString)
{
    for (int i = 1; i <= 30; i++)
    {
        try
        {
            await using var conn = new NpgsqlConnection(connectionString);
            await conn.OpenAsync();

            await using var create = new NpgsqlCommand("""
                CREATE TABLE IF NOT EXISTS lab_log (
                    id SERIAL PRIMARY KEY,
                    message TEXT NOT NULL,
                    created_at TIMESTAMPTZ DEFAULT NOW()
                )
                """, conn);
            await create.ExecuteNonQueryAsync();

            await using var insert = new NpgsqlCommand("""
                INSERT INTO lab_log (message)
                SELECT 'Stack uruchomiony'
                WHERE NOT EXISTS (SELECT 1 FROM lab_log)
                """, conn);
            await insert.ExecuteNonQueryAsync();

            Console.WriteLine("Polaczenie z PostgreSQL OK");
            return;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Proba {i}/30 polaczenia z baza: {ex.Message}");
            await Task.Delay(2000);
        }
    }

    throw new InvalidOperationException("Nie udalo sie polaczyc z PostgreSQL");
}
