using System.Security.Claims;
using LaundrySystem.Api.Controllers.Base;
using LaundrySystem.BLL.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LaundrySystem.API.Controllers.Implementations
{
    /// <summary>
    /// Controller for managing in-app notifications.
    /// </summary>
    [ApiController]
    [Route("api/notifications")]
    [Authorize]
    public class NotificationsController : BaseController<NotificationsController>
    {
        private readonly IInAppNotificationService _notificationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationsController"/> class.
        /// </summary>
        /// <param name="notificationService">The in-app notification service.</param>
        /// <param name="logger">The logger.</param>
        public NotificationsController(IInAppNotificationService notificationService, ILogger<NotificationsController> logger)
            : base(logger)
        {
            _notificationService = notificationService;
        }

        /// <summary>
        /// Gets all notifications for the current user.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>List of notifications.</returns>
        [HttpGet]
        public async Task<IActionResult> GetMyNotifications(CancellationToken cancellationToken)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null) return Unauthorized();

                var response = await _notificationService.GetByUserIdAsync(userId.Value, cancellationToken);
                if (!response.Success)
                {
                    return BadRequest(response.Message);
                }
                return Ok(response.Data);
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        /// <summary>
        /// Gets the unread notification count for the current user.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Unread count.</returns>
        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCount(CancellationToken cancellationToken)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null) return Unauthorized();

                var response = await _notificationService.GetUnreadCountAsync(userId.Value, cancellationToken);
                if (!response.Success)
                {
                    return BadRequest(response.Message);
                }
                return Ok(new { count = response.Data });
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        /// <summary>
        /// Marks all notifications as read for the current user.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Success status.</returns>
        [HttpPost("mark-all-read")]
        public async Task<IActionResult> MarkAllAsRead(CancellationToken cancellationToken)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null) return Unauthorized();

                var response = await _notificationService.MarkAllAsReadAsync(userId.Value, cancellationToken);
                if (!response.Success)
                {
                    return BadRequest(response.Message);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        private Guid? GetCurrentUserId()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(userIdStr, out var userId) ? userId : null;
        }
    }
}
