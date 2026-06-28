using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using SecurityAudit.Data;
using SecurityAudit.Models;
using SecurityAudit.Services;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpsRedirection(opt =>
    opt.HttpsPort = 443);
builder.Services.AddHsts(opt =>
{
    opt.MaxAge = TimeSpan.FromDays(365);
    opt.IncludeSubDomains = true;
    opt.Preload = true;
});

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlite("Data Source=securityaudit.db"));

builder.Services.AddRateLimiter(opt =>
    opt.AddFixedWindowLimiter("api", o =>
    {
        o.PermitLimit = 100;
        o.Window = TimeSpan.FromMinutes(1);
        o.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        o.QueueLimit = 10;
    }));

var apiKey = builder.Configuration["ApiKey"]
    ?? throw new InvalidOperationException("Brak ApiKey w konfiguracji!");

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
    app.UseHttpsRedirection();
}
app.UseRateLimiter();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.EnsureDeletedAsync();
    await db.Database.EnsureCreatedAsync();
    db.Users.AddRange(
        new User { Username = "admin", Email = "admin@firma.pl" },
        new User { Username = "anna", Email = "anna@firma.pl" });
    await db.SaveChangesAsync();
}

SecureTokenStorage.Save("moj-tajny-token-api");
string token = SecureTokenStorage.Load();
Console.WriteLine($"Token DPAPI zapisany. Odczyt (4 znaki): {token[..4]}***");
Console.WriteLine($"ApiKey z User Secrets (4 znaki): {apiKey[..4]}***");

app.MapGet("/health", () => Results.Ok(new { status = "healthy" }))
   .RequireRateLimiting("api");

app.MapPost("/audits", async (CreateAuditRequest req, AppDbContext db) =>
{
    var results = new List<ValidationResult>();
    var ctx = new ValidationContext(req);

    if (!Validator.TryValidateObject(req, ctx, results, true))
    {
        var errors = results.ToDictionary(
            r => r.MemberNames.First(),
            r => new[] { r.ErrorMessage! });
        return Results.ValidationProblem(errors);
    }

    var item = new AuditItem { Title = req.Title, Priority = req.Priority, Description = req.Description };
    db.Audits.Add(item);
    await db.SaveChangesAsync();
    return Results.Created($"/audits/{item.Id}", item);
}).RequireRateLimiting("api");

app.MapGet("/users/search", async (string username, AppDbContext db) =>
{
    var user = await UserQueries.GetUserSecureAsync(username, db);
    return user is null ? Results.NotFound() : Results.Ok(user);
}).RequireRateLimiting("api");

app.MapGet("/users/search-raw", async (string username, AppDbContext db) =>
{
    var user = await UserQueries.GetUserRawAsync(username, db);
    return user is null ? Results.NotFound() : Results.Ok(user);
}).RequireRateLimiting("api");

app.MapGet("/test/sql-injection", async (AppDbContext db) =>
{
    var malicious = "' OR 1=1 --";
    var secure = await UserQueries.GetUserSecureAsync(malicious, db);
    var raw = await UserQueries.GetUserRawAsync(malicious, db);
    return Results.Ok(new
    {
        input = malicious,
        secureResult = secure?.Username,
        rawResult = raw?.Username,
        safe = secure is null && raw is null
    });
}).RequireRateLimiting("api");

app.MapGet("/test/validation", () =>
{
    var bad = new CreateAuditRequest
    {
        Title = "<script>alert('xss')</script>",
        Priority = 99
    };
    var vResults = new List<ValidationResult>();
    bool valid = Validator.TryValidateObject(
        bad, new ValidationContext(bad), vResults, true);
    return Results.Ok(new
    {
        valid,
        errors = vResults.Select(v => v.ErrorMessage).ToList()
    });
}).RequireRateLimiting("api");

app.MapGet("/test/token", () =>
{
    SecureTokenStorage.Save("moj-tajny-token-api");
    string loaded = SecureTokenStorage.Load();
    return Results.Ok(new
    {
        encryptedFile = Path.GetFullPath("token.enc"),
        preview = $"{loaded[..4]}***"
    });
}).RequireRateLimiting("api");

Console.WriteLine("SecurityAudit uruchomiony.");
Console.WriteLine("POST /audits  GET /test/validation  GET /test/token  GET /test/sql-injection");

app.Run();
