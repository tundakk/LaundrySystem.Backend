using LaundrySystem.DAL.DataModel;
using LaundrySystem.DAL.Repos.Base;
using LaundrySystem.DAL.Repos.Interfaces;
using LaundrySystem.Domain.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LaundrySystem.DAL.Repos.Implementations
{
    /// <summary>
    /// Repository for Notification entities.
    /// </summary>
    public class NotificationRepo : BaseRepo<Notification>, INotificationRepo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationRepo"/> class.
        /// </summary>
        /// <param name="dataContext">The data context.</param>
        /// <param name="logger">The logger.</param>
        public NotificationRepo(DataContext dataContext, ILogger<NotificationRepo> logger)
            : base(dataContext, logger)
        {
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Notification>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await dataContext.Set<Notification>()
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<int> GetUnreadCountAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await dataContext.Set<Notification>()
                .CountAsync(n => n.UserId == userId && !n.IsRead, cancellationToken);
        }

        /// <inheritdoc />
        public async Task MarkAllAsReadAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            await dataContext.Set<Notification>()
                .Where(n => n.UserId == userId && !n.IsRead)
                .ExecuteUpdateAsync(s => s.SetProperty(n => n.IsRead, true), cancellationToken);
        }
    }
}
