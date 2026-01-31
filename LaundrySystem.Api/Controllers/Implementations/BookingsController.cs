using LaundrySystem.Api.Controllers.Base;
using LaundrySystem.BLL.Infrastructure.Interfaces;
using LaundrySystem.Domain.Model.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LaundrySystem.API.Controllers.Implementations
{
    /// <summary>
    /// Request model for creating a booking.
    /// </summary>
    public class CreateBookingRequest
    {
        /// <summary>
        /// The timeslot ID to book.
        /// </summary>
        public Guid TimeslotId { get; set; }
    }

    /// <summary>
    /// Controller for managing bookings.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BookingsController : BaseController<BookingsController>
    {
        private readonly IBookingService _bookingService;

        /// <summary>
        /// Initializes a new instance of the <see cref="BookingsController"/> class.
        /// </summary>
        /// <param name="bookingService">The Booking service.</param>
        /// <param name="logger">The logger.</param>
        public BookingsController(IBookingService bookingService, ILogger<BookingsController> logger)
            : base(logger)
        {
            _bookingService = bookingService;
        }

        /// <summary>
        /// Gets all bookings.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>List of all bookings.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
        {
            try
            {
                var response = await _bookingService.GetAllAsync(cancellationToken);
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
        /// Gets the authenticated user's upcoming bookings with timeslot and room details.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>List of the user's upcoming bookings.</returns>
        [HttpGet("my")]
        public async Task<IActionResult> GetMyBookingsAsync(CancellationToken cancellationToken)
        {
            try
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!Guid.TryParse(userIdClaim, out var userId))
                {
                    return Unauthorized(new { message = "Unable to extract user ID from authentication token." });
                }

                var response = await _bookingService.GetUpcomingByUserIdAsync(userId, cancellationToken);
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
        /// Gets a booking by ID.
        /// </summary>
        /// <param name="id">The booking ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The booking.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _bookingService.GetByIdAsync(id, cancellationToken);
                if (!response.Success)
                {
                    return NotFound(response.Message);
                }
                return Ok(response.Data);
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        /// <summary>
        /// Creates a new booking for the authenticated user and timeslot.
        /// This is the recommended way to create bookings as it handles timeslot availability.
        /// </summary>
        /// <param name="request">The booking request containing timeslotId.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created booking.</returns>
        [HttpPost]
        public async Task<IActionResult> CreateBookingAsync([FromBody] CreateBookingRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!Guid.TryParse(userIdClaim, out var userId))
                {
                    return Unauthorized(new { message = "Unable to extract user ID from authentication token." });
                }

                var response = await _bookingService.CreateBookingAsync(userId, request.TimeslotId, cancellationToken);
                if (!response.Success)
                {
                    Logger.LogWarning("Booking creation failed for user {UserId}: {Message}", userId, response.Message);
                    return BadRequest(new { message = response.Message });
                }
                if (response.Data == null)
                {
                    return BadRequest(new { message = "Failed to create the booking." });
                }
                return Created($"api/bookings/{response.Data.Id}", response.Data);
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        /// <summary>
        /// Creates a new booking directly from a booking model.
        /// Note: Prefer using the POST endpoint with CreateBookingRequest for proper availability handling.
        /// </summary>
        /// <param name="bookingModel">The booking model.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created booking.</returns>
        [HttpPost("direct")]
        public async Task<IActionResult> InsertAsync([FromBody] BookingModel bookingModel, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _bookingService.InsertAsync(bookingModel, cancellationToken);
                if (!response.Success)
                {
                    return BadRequest(response.Message);
                }
                if (response.Data == null)
                {
                    return BadRequest("Failed to create the Booking");
                }
                return Created($"api/bookings/{response.Data.Id}", response.Data);
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        /// <summary>
        /// Updates an existing booking.
        /// </summary>
        /// <param name="id">The booking ID.</param>
        /// <param name="bookingModel">The booking model.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The updated booking.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] BookingModel bookingModel, CancellationToken cancellationToken)
        {
            try
            {
                bookingModel.Id = id;
                var response = await _bookingService.UpdateAsync(bookingModel, cancellationToken);
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
        /// Deletes a booking.
        /// </summary>
        /// <param name="id">The booking ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Success message.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _bookingService.DeleteAsync(id, cancellationToken);
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
