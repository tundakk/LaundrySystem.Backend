namespace LaundrySystem.Domain.Model.Entities;

public enum ServiceMessageSeverity
{
    Info = 0,
    Warning = 1,
    Critical = 2
}

public class ServiceMessage
{
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Multi-tenancy: Which account this message belongs to (required)
    /// </summary>
    public Guid AccountId { get; set; }

    /// <summary>
    /// Optional: If set, message only applies to this building.
    /// If null, message applies to all buildings in the account.
    /// </summary>
    public Guid? BuildingId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Body { get; set; } = string.Empty;

    public ServiceMessageSeverity Severity { get; set; } = ServiceMessageSeverity.Info;

    public DateTime ActiveFrom { get; set; } = DateTime.UtcNow;

    public DateTime? ActiveTo { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Account Account { get; set; } = null!;
    public Building? Building { get; set; }

    public bool IsActive => DateTime.UtcNow >= ActiveFrom && (ActiveTo == null || DateTime.UtcNow <= ActiveTo);
}
