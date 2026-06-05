namespace GymPos.Models;

public enum NotificationType
{
    Success,
    Error,
    Warning,
    Informational
}

public class Notification
{
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; } = NotificationType.Informational;
    public int DurationSeconds { get; set; } = 2;
}
