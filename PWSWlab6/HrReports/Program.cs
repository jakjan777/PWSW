using HrReports.Builders;
using HrReports.Models;

static void WyswietlRaport(string etykieta, HrReport raport)
{
    Console.WriteLine($"\n--- {etykieta} ---");
    Console.WriteLine($"Tytul: {raport.Title}");
    Console.WriteLine($"Autor: {raport.Author}");
    Console.WriteLine($"Format: {raport.Format}");
    Console.WriteLine($"Sekcji: {raport.Sections.Count}");
    foreach (var sekcja in raport.Sections)
    {
        if (sekcja.Type == "Text")
            Console.WriteLine($"  [Tekst] {sekcja.Heading}: {sekcja.Content}");
        else
            Console.WriteLine($"  [Wykres] {sekcja.Heading}: {sekcja.Chart}");
    }
}

Console.WriteLine("=== Budowanie raportow HR ===");

var raportFluent = new FluentReportBuilder()
    .WithTitle("Raport rotacji pracownikow")
    .WithAuthor("Jan Kowalski")
    .InFormat(ReportFormat.Html)
    .AddText("Wstep", "Analiza rotacji za rok 2025")
    .AddChart("Trend rotacji", ChartType.Line)
    .WithMetadata("Dzial", "HR")
    .Build();

WyswietlRaport("Fluent Builder", raportFluent);

var raportStep = StepReportBuilder.Create()
    .WithTitle("Raport zatrudnienia Q1")
    .WithAuthor("Anna Nowak")
    .AddText("Podsumowanie", "Przyjeto 12 nowych pracownikow")
    .AddChart("Struktura zatrudnienia", ChartType.Pie)
    .InFormat(ReportFormat.Pdf)
    .Build();

WyswietlRaport("Step Builder", raportStep);
