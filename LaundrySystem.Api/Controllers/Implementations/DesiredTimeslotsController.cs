using LaundrySystem.Api.Controllers.Base;
using LaundrySystem.BLL.Infrastructure.Interfaces;
using LaundrySystem.Domain.Model.Models;
using Microsoft.AspNetCore.Mvc;

namespace LaundrySystem.API.Controllers.Implementations
{
    /// <summary>
    /// DesiredTimeslotsController
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
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

        ///<inheritdoc/>
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            try
            {
                var response = await _desiredTimeslotService.GetAllAsync();
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
                var response = await _desiredTimeslotService.GetByIdAsync(id);
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

        ///<inheritdoc/>
        [HttpPost]
        public async Task<IActionResult> InsertAsync([FromBody] DesiredTimeslotModel desiredTimeslotModel)
        {
            try
            {
                var response = await _desiredTimeslotService.InsertAsync(desiredTimeslotModel);
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
        public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] DesiredTimeslotModel desiredTimeslotModel)
        {
            try
            {
                desiredTimeslotModel.Id = id;
                var response = await _desiredTimeslotService.UpdateAsync(desiredTimeslotModel);
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
                var response = await _desiredTimeslotService.DeleteAsync(id);
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