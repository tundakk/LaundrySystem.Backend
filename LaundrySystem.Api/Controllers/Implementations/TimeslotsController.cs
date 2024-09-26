using LaundrySystem.Api.Controllers.Base;
using LaundrySystem.BLL.Infrastructure.Interfaces;
using LaundrySystem.Domain.Model.Models;
using Microsoft.AspNetCore.Mvc;

namespace LaundrySystem.API.Controllers.Implementations
{
    /// <summary>
    /// TimeslotsController
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class TimeslotsController : BaseController<TimeslotsController>
    {
        private readonly ITimeslotService _timeslotService;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeslotsController"/> class.
        /// </summary>
        /// <param name="timeslotService">The Timeslot service.</param>
        /// <param name="logger">The logger.</param>
        public TimeslotsController(ITimeslotService timeslotService, ILogger<TimeslotsController> logger)
            : base(logger)
        {
            _timeslotService = timeslotService;
        }

        ///<inheritdoc/>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var response = await _timeslotService.GetAllAsync();
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
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var response = await _timeslotService.GetByIdAsync(id);
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
        public async Task<IActionResult> Insert([FromBody] TimeslotModel timeslotModel)
        {
            try
            {
                var response = await _timeslotService.InsertAsync(timeslotModel);
                if (!response.Success)
                {
                    return BadRequest(response.Message);
                }
                if (response.Data == null)
                {
                    return BadRequest("Failed to create the Timeslot");
                }
                return CreatedAtAction(nameof(GetById), new { id = response.Data.Id }, response.Data);
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        ///<inheritdoc/>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] TimeslotModel timeslotModel)
        {
            try
            {
                timeslotModel.Id = id;
                var response = await _timeslotService.UpdateAsync(timeslotModel);
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
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var response = await _timeslotService.DeleteAsync(id);
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