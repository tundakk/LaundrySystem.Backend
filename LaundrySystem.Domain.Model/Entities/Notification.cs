namespace LaundrySystem.Domain.Model.Entities
{
    /// <summary>
    /// Represents an in-app notification for a user.
    /// </summary>
    public class Notification
    {
        /// <summary>
        /// The unique identifier for the notification.
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// The user who receives this notification.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Short title for the notification.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// The notification message body.
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Optional link to navigate to when clicked (e.g. /lost-found).
        /// </summary>
        public string? Link { get; set; }

        /// <summary>
        /// Whether the user has read this notification.
        /// </summary>
        public bool IsRead { get; set; } = false;

        /// <summary>
        /// When the notification was created.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Navigation property to the user.
        /// </summary>
        public AppUser User { get; set; } = null!;
    }
}
