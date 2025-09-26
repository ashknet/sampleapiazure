using Emr.Domain.Common;

namespace Emr.Domain.Entities;

public class NotificationPreference : BaseEntity
{
    public Guid UserId { get; private set; }
    public string Channel { get; private set; } = string.Empty; // e.g., "Email", "SMS", "Push"
    public string NotificationType { get; private set; } = string.Empty; // e.g., "AppointmentReminder", "LabResult", "DocumentShared"
    public bool IsEnabled { get; private set; }
    public string? Frequency { get; private set; } // e.g., "Immediate", "Daily", "Weekly"
    public string? AdditionalSettings { get; private set; } // JSON for channel-specific settings
    
    // Navigation properties
    public virtual User User { get; private set; } = null!;
    
    protected NotificationPreference() { }
    
    public NotificationPreference(
        Guid userId,
        string channel,
        string notificationType,
        bool isEnabled = true)
    {
        UserId = userId;
        Channel = channel;
        NotificationType = notificationType;
        IsEnabled = isEnabled;
    }
    
    public void UpdateSettings(bool isEnabled, string? frequency, string? additionalSettings)
    {
        IsEnabled = isEnabled;
        Frequency = frequency;
        AdditionalSettings = additionalSettings;
    }
    
    public void Enable() => IsEnabled = true;
    public void Disable() => IsEnabled = false;
}