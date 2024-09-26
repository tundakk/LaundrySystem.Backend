using LaundrySystem.Api.Controllers.Base;
using LaundrySystem.BLL.Infrastructure.Interfaces;
using LaundrySystem.Domain.Model.Models;
using Microsoft.AspNetCore.Mvc;

namespace LaundrySystem.API.Controllers.Implementations
{
    /// <summary>
    /// ServiceMessagesController
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ServiceMessagesController : BaseController<ServiceMessagesController>
    {
        private readonly IServiceMessageService _serviceMessageService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceMessagesController"/> class.
        /// </summary>
        /// <param name="serviceMessageService">The ServiceMessage service.</param>
        /// <param name="logger">The logger.</param>
        public ServiceMessagesController(IServiceMessageService serviceMessageService, ILogger<ServiceMessagesController> logger)
            : base(logger)
        {
            _serviceMessageService = serviceMessageService;
        }

        ///<inheritdoc/>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var response = await _serviceMessageService.GetAllAsync();
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
                var response = await _serviceMessageService.GetByIdAsync(id);
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
        public async Task<IActionResult> Insert([FromBody] ServiceMessageModel serviceMessageModel)
        {
            try
            {
                var response = await _serviceMessageService.InsertAsync(serviceMessageModel);
                if (!response.Success)
                {
                    return BadRequest(response.Message);
                }
                if (response.Data == null)
                {
                    return BadRequest("Failed to create the ServiceMessage");
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
        public async Task<IActionResult> Update(Guid id, [FromBody] ServiceMessageModel serviceMessageModel)
        {
            try
            {
                serviceMessageModel.Id = id;
                var response = await _serviceMessageService.UpdateAsync(serviceMessageModel);
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
                var response = await _serviceMessageService.DeleteAsync(id);
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