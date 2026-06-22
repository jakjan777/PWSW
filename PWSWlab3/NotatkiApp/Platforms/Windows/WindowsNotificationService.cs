using Microsoft.Windows.AppNotifications;
using Microsoft.Windows.AppNotifications.Builder;
using NotatkiApp.Services;

namespace NotatkiApp.Platforms.Windows;

public class WindowsNotificationService : INotificationService
{
    public WindowsNotificationService()
    {
        try
        {
            AppNotificationManager.Default.Register();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Notification register failed] {ex.Message}");
        }
    }

    public Task ShowNotification(string title, string message)
    {
        try
        {
            var notification = new AppNotificationBuilder()
                .AddText(title)
                .AddText(message)
                .BuildNotification();

            AppNotificationManager.Default.Show(notification);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Notification] {title}: {message}");
            Console.WriteLine($"[Notification failed] {ex.Message}");
        }

        return Task.CompletedTask;
    }
}
