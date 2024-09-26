using LaundrySystem.Api.Controllers.Base;
using LaundrySystem.BLL.Infrastructure.Interfaces;
using LaundrySystem.Domain.Model.Models;
using Microsoft.AspNetCore.Mvc;

namespace LaundrySystem.API.Controllers.Implementations
{
    /// <summary>
    /// RoomsController
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class RoomsController : BaseController<RoomsController>
    {
        private readonly IRoomService _roomService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RoomsController"/> class.
        /// </summary>
        /// <param name="roomService">The Room service.</param>
        /// <param name="logger">The logger.</param>
        public RoomsController(IRoomService roomService, ILogger<RoomsController> logger)
            : base(logger)
        {
            _roomService = roomService;
        }

        ///<inheritdoc/>
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            try
            {
                var response = await _roomService.GetAllAsync();
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
                var response = await _roomService.GetByIdAsync(id);
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
        public async Task<IActionResult> InsertAsync([FromBody] RoomModel roomModel)
        {
            try
            {
                var response = await _roomService.InsertAsync(roomModel);
                if (!response.Success)
                {
                    return BadRequest(response.Message);
                }
                if (response.Data == null)
                {
                    return BadRequest("Failed to create the Room");
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
        public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] RoomModel roomModel)
        {
            try
            {
                roomModel.Id = id;
                var response = await _roomService.UpdateAsync(roomModel);
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
                var response = await _roomService.DeleteAsync(id);
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