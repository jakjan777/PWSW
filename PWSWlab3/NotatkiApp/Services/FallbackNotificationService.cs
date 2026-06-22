namespace NotatkiApp.Services;

public class FallbackNotificationService : INotificationService
{
    public Task ShowNotification(string title, string message)
    {
        Console.WriteLine($"[Notification] {title}: {message}");
        return Task.CompletedTask;
    }
}
