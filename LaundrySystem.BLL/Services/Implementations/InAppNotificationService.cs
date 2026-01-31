namespace LaundrySystem.BLL.Infrastructure.Services.Implementations
{
    using LaundrySystem.BLL.Infrastructure.Interfaces;
    using LaundrySystem.DAL.Repos.Interfaces;
    using LaundrySystem.Domain.Model.Entities;
    using LaundrySystem.Domain.Model.Models;
    using LaundrySystem.Domain.Model.Responses;
    using Mapster;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Service for managing in-app notifications.
    /// </summary>
    public class InAppNotificationService : IInAppNotificationService
    {
        private readonly INotificationRepo _notificationRepo;
        private readonly ILogger<InAppNotificationService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="InAppNotificationService"/> class.
        /// </summary>
        /// <param name="notificationRepo">The notification repository.</param>
        /// <param name="logger">The logger.</param>
        public InAppNotificationService(INotificationRepo notificationRepo, ILogger<InAppNotificationService> logger)
        {
            _notificationRepo = notificationRepo;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<ServiceResponse<IEnumerable<NotificationModel>>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            try
            {
                var notifications = await _notificationRepo.GetByUserIdAsync(userId, cancellationToken);
                var models = notifications.Adapt<IEnumerable<NotificationModel>>();
                return new ServiceResponse<IEnumerable<NotificationModel>> { Success = true, Data = models };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting notifications for user {UserId}", userId);
                return new ServiceResponse<IEnumerable<NotificationModel>> { Success = false, Message = ex.Message };
            }
        }

        /// <inheritdoc />
        public async Task<ServiceResponse<int>> GetUnreadCountAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            try
            {
                var count = await _notificationRepo.GetUnreadCountAsync(userId, cancellationToken);
                return new ServiceResponse<int> { Success = true, Data = count };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting unread count for user {UserId}", userId);
                return new ServiceResponse<int> { Success = false, Message = ex.Message };
            }
        }

        /// <inheritdoc />
        public async Task<ServiceResponse<bool>> MarkAllAsReadAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            try
            {
                await _notificationRepo.MarkAllAsReadAsync(userId, cancellationToken);
                return new ServiceResponse<bool> { Success = true, Data = true };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking notifications as read for user {UserId}", userId);
                return new ServiceResponse<bool> { Success = false, Message = ex.Message };
            }
        }

        /// <inheritdoc />
        public async Task<ServiceResponse<NotificationModel>> CreateAsync(Guid userId, string title, string message, string? link = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var entity = new Notification
                {
                    UserId = userId,
                    Title = title,
                    Message = message,
                    Link = link,
                    CreatedAt = DateTime.UtcNow
                };

                var inserted = await _notificationRepo.InsertAsync(entity, cancellationToken);
                var model = inserted.Adapt<NotificationModel>();
                return new ServiceResponse<NotificationModel> { Success = true, Data = model };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating notification for user {UserId}", userId);
                return new ServiceResponse<NotificationModel> { Success = false, Message = ex.Message };
            }
        }
    }
}
