using FluentValidation;
using KonferencjaApi;
using KonferencjaApi.Models;
using KonferencjaApi.Validators;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddProblemDetails();
builder.Services.AddScoped<IValidator<RejestracjaRequest>, RejestracjaValidator>();
builder.Services.AddExceptionHandler<GlobalnyObslugaBledow>();

var app = builder.Build();
app.UseExceptionHandler();

app.MapPost("/rejestracja", async (
    RejestracjaRequest request,
    IValidator<RejestracjaRequest> walidator) =>
{
    var wynik = await walidator.ValidateAsync(request);
    if (!wynik.IsValid)
    {
        var bledy = wynik.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.ErrorMessage).ToArray());
        return Results.ValidationProblem(bledy,
            title: "Blad walidacji rejestracji");
    }

    return TypedResults.Created(
        $"/uczestnicy/{request.Email}",
        new { Wiadomosc = $"Witaj, {request.Imie}!" });
});

app.MapGet("/", () => "KonferencjaApi - POST /rejestracja");

app.Run();
