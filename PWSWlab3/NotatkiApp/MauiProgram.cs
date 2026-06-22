using NotatkiApp.Services;
#if WINDOWS
using NotatkiApp.Platforms.Windows;
#endif

namespace NotatkiApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder.UseMauiApp<App>();

        builder.Services.AddMauiBlazorWebView();
        builder.Services.AddSingleton<InMemoryNoteRepository>();
        builder.Services.AddSingleton<RepositoryCacheDiagnostics>();
        builder.Services.AddSingleton<INoteRepository>(sp =>
        {
            var inner = sp.GetRequiredService<InMemoryNoteRepository>();
            var diagnostics = sp.GetRequiredService<RepositoryCacheDiagnostics>();
            return new CachingNoteDecorator(inner, diagnostics);
        });
        builder.Services.AddSingleton<INoteService, NoteService>();
        builder.Services.AddSingleton<INoteClassifier, NoteClassifier>();
        builder.Services.AddTransient<ExportService>();

#if WINDOWS
        builder.Services.AddSingleton<INotificationService, WindowsNotificationService>();
#else
        builder.Services.AddSingleton<INotificationService, FallbackNotificationService>();
#endif

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
#endif

        return builder.Build();
    }
}
