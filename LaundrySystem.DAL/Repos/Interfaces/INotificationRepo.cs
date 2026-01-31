using LaundrySystem.DAL.Repos.Base;
using LaundrySystem.Domain.Model.Entities;

namespace LaundrySystem.DAL.Repos.Interfaces
{
    /// <summary>
    /// Repository interface for Notification entities.
    /// </summary>
    public interface INotificationRepo : IBaseRepo<Notification>
    {
        /// <summary>
        /// Gets all notifications for a specific user, ordered by newest first.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Notifications for the user.</returns>
        Task<IEnumerable<Notification>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the count of unread notifications for a user.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Unread notification count.</returns>
        Task<int> GetUnreadCountAsync(Guid userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Marks all notifications for a user as read.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task MarkAllAsReadAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
