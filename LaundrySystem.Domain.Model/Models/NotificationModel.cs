namespace LaundrySystem.Domain.Model.Models
{
    /// <summary>
    /// Model for in-app notifications.
    /// </summary>
    public class NotificationModel
    {
        /// <summary>
        /// The notification ID.
        /// </summary>
        public Guid Id { get; set; }

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
        /// Optional link to navigate to when clicked.
        /// </summary>
        public string? Link { get; set; }

        /// <summary>
        /// Whether the user has read this notification.
        /// </summary>
        public bool IsRead { get; set; }

        /// <summary>
        /// When the notification was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}
