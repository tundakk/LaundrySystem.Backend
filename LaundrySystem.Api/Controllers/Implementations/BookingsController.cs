using LaundrySystem.Api.Controllers.Base;
using LaundrySystem.BLL.Infrastructure.Interfaces;
using LaundrySystem.Domain.Model.Models;
using Microsoft.AspNetCore.Mvc;

namespace LaundrySystem.API.Controllers.Implementations
{
    /// <summary>
    /// BookingsController
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
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

        ///<inheritdoc/>
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            try
            {
                var response = await _bookingService.GetAllAsync();
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

        ///<inheritdoc/>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            try
            {
                var response = await _bookingService.GetByIdAsync(id);
                if (!response.Success)
                {
                    return NotFound(response.Message); // Use NotFound for invalid IDs
                }
                return Ok(response.Data);
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        ///<inheritdoc/>
        [HttpPost]
        public async Task<IActionResult> InsertAsync([FromBody] BookingModel bookingModel)
        {
            try
            {
                var response = await _bookingService.InsertAsync(bookingModel);
                if (!response.Success)
                {
                    return BadRequest(response.Message);
                }
                if (response.Data == null)
                {
                    return BadRequest("Failed to create the Booking");
                }
                return CreatedAtAction(nameof(GetByIdAsync), new { id = response.Data.Id }, response.Data);
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        ///<inheritdoc/>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] BookingModel bookingModel)
        {
            try
            {
                bookingModel.Id = id;
                var response = await _bookingService.UpdateAsync(bookingModel);
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

        ///<inheritdoc/>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            try
            {
                var response = await _bookingService.DeleteAsync(id);
                if (!response.Success)
                {
                    return BadRequest(response.Message);
                }
                return Ok(response.Message); // Return a success message or confirmation
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }
    }
}