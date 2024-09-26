using LaundrySystem.Api.Controllers.Base;
using LaundrySystem.BLL.Infrastructure.Interfaces;
using LaundrySystem.Domain.Model.Models;
using Microsoft.AspNetCore.Mvc;

namespace LaundrySystem.API.Controllers.Implementations
{
    /// <summary>
    /// LostAndFoundsController
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class LostAndFoundsController : BaseController<LostAndFoundsController>
    {
        private readonly ILostAndFoundService _lostAndFoundService;

        /// <summary>
        /// Initializes a new instance of the <see cref="LostAndFoundsController"/> class.
        /// </summary>
        /// <param name="lostAndFoundService">The LostAndFound service.</param>
        /// <param name="logger">The logger.</param>
        public LostAndFoundsController(ILostAndFoundService lostAndFoundService, ILogger<LostAndFoundsController> logger)
            : base(logger)
        {
            _lostAndFoundService = lostAndFoundService;
        }

        ///<inheritdoc/>
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            try
            {
                var response = await _lostAndFoundService.GetAllAsync();
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
                var response = await _lostAndFoundService.GetByIdAsync(id);
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
        public async Task<IActionResult> InsertAsync([FromBody] LostAndFoundModel lostAndFoundModel)
        {
            try
            {
                var response = await _lostAndFoundService.InsertAsync(lostAndFoundModel);
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
        public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] LostAndFoundModel lostAndFoundModel)
        {
            try
            {
                lostAndFoundModel.Id = id;
                var response = await _lostAndFoundService.UpdateAsync(lostAndFoundModel);
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
                var response = await _lostAndFoundService.DeleteAsync(id);
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