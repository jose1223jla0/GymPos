using GymPos.Models;
using System;

namespace GymPos.Services;

public interface INotificationService
{
    event EventHandler<Notification>? NotificationRequested;
    void ShowNotification(string title, string message, NotificationType type = NotificationType.Informational, int durationSeconds = 3);
    void ShowSuccess(string title, string message, int durationSeconds = 3);
    void ShowError(string title, string message, int durationSeconds = 5);
    void ShowWarning(string title, string message, int durationSeconds = 4);
    void ShowInfo(string title, string message, int durationSeconds = 3);
}

public class NotificationService : INotificationService
{
    public event EventHandler<Notification>? NotificationRequested;

    public void ShowNotification(string title, string message, NotificationType type = NotificationType.Informational, int durationSeconds = 3)
    {
        var notification = new Notification
        {
            Title = title,
            Message = message,
            Type = type,
            DurationSeconds = durationSeconds
        };

        NotificationRequested?.Invoke(this, notification);
    }

    public void ShowSuccess(string title, string message, int durationSeconds = 3)
    {
        ShowNotification(title, message, NotificationType.Success, durationSeconds);
    }

    public void ShowError(string title, string message, int durationSeconds = 5)
    {
        ShowNotification(title, message, NotificationType.Error, durationSeconds);
    }

    public void ShowWarning(string title, string message, int durationSeconds = 4)
    {
        ShowNotification(title, message, NotificationType.Warning, durationSeconds);
    }

    public void ShowInfo(string title, string message, int durationSeconds = 3)
    {
        ShowNotification(title, message, NotificationType.Informational, durationSeconds);
    }
}
