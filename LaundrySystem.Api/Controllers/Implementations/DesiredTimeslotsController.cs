using LaundrySystem.Api.Controllers.Base;
using LaundrySystem.BLL.Infrastructure.Interfaces;
using LaundrySystem.Domain.Model.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LaundrySystem.API.Controllers.Implementations
{
    /// <summary>
    /// Request model for subscribing to a timeslot notification.
    /// </summary>
    public class CreateDesiredTimeslotRequest
    {
        /// <summary>
        /// The timeslot ID to subscribe to.
        /// </summary>
        public Guid TimeslotId { get; set; }
    }

    /// <summary>
    /// Controller for managing desired timeslot subscriptions.
    /// </summary>
    [ApiController]
    [Route("api/desired-timeslots")]
    [Authorize]
    public class DesiredTimeslotsController : BaseController<DesiredTimeslotsController>
    {
        private readonly IDesiredTimeslotService _desiredTimeslotService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DesiredTimeslotsController"/> class.
        /// </summary>
        /// <param name="desiredTimeslotService">The DesiredTimeslot service.</param>
        /// <param name="logger">The logger.</param>
        public DesiredTimeslotsController(IDesiredTimeslotService desiredTimeslotService, ILogger<DesiredTimeslotsController> logger)
            : base(logger)
        {
            _desiredTimeslotService = desiredTimeslotService;
        }

        /// <summary>
        /// Gets the current user's desired timeslot subscriptions.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>List of the user's desired timeslot subscriptions.</returns>
        [HttpGet("my")]
        public async Task<IActionResult> GetMySubscriptionsAsync(CancellationToken cancellationToken)
        {
            try
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!Guid.TryParse(userIdClaim, out var userId))
                {
                    return Unauthorized(new { message = "Unable to extract user ID from authentication token." });
                }

                var response = await _desiredTimeslotService.GetAllAsync(cancellationToken);
                if (!response.Success)
                {
                    return BadRequest(response.Message);
                }

                var mySubscriptions = response.Data?.Where(d => d.UserId == userId && !d.NotificationSent).ToList();
                return Ok(mySubscriptions);
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        /// <summary>
        /// Creates a new desired timeslot subscription for the authenticated user.
        /// </summary>
        /// <param name="request">The request containing the timeslot ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created desired timeslot subscription.</returns>
        [HttpPost]
        public async Task<IActionResult> SubscribeAsync([FromBody] CreateDesiredTimeslotRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!Guid.TryParse(userIdClaim, out var userId))
                {
                    return Unauthorized(new { message = "Unable to extract user ID from authentication token." });
                }

                var model = new DesiredTimeslotModel
                {
                    UserId = userId,
                    TimeslotId = request.TimeslotId,
                    NotificationSent = false,
                    CreatedAt = DateTime.UtcNow
                };

                var response = await _desiredTimeslotService.InsertAsync(model, cancellationToken);
                if (!response.Success)
                {
                    return BadRequest(response.Message);
                }
                if (response.Data == null)
                {
                    return BadRequest("Failed to create the subscription.");
                }
                return Created($"api/desired-timeslots/{response.Data.Id}", response.Data);
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        /// <summary>
        /// Deletes a desired timeslot subscription.
        /// </summary>
        /// <param name="id">The desired timeslot subscription ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Success message.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _desiredTimeslotService.DeleteAsync(id, cancellationToken);
                if (!response.Success)
                {
                    return BadRequest(response.Message);
                }
                return Ok(response.Message);
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }
    }
}
