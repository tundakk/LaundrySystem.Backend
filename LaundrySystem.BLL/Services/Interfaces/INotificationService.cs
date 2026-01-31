namespace LaundrySystem.BLL.Infrastructure.Interfaces
{
    using LaundrySystem.Domain.Model.Models;
    using LaundrySystem.Domain.Model.Responses;

    /// <summary>
    /// Service interface for managing in-app notifications.
    /// </summary>
    public interface IInAppNotificationService
    {
        /// <summary>
        /// Gets all notifications for a specific user.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>List of notification models.</returns>
        Task<ServiceResponse<IEnumerable<NotificationModel>>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets unread notification count for a user.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Unread count.</returns>
        Task<ServiceResponse<int>> GetUnreadCountAsync(Guid userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Marks all notifications for a user as read.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<ServiceResponse<bool>> MarkAllAsReadAsync(Guid userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a notification for a specific user.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="title">Notification title.</param>
        /// <param name="message">Notification message.</param>
        /// <param name="link">Optional link.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<ServiceResponse<NotificationModel>> CreateAsync(Guid userId, string title, string message, string? link = null, CancellationToken cancellationToken = default);
    }
}
