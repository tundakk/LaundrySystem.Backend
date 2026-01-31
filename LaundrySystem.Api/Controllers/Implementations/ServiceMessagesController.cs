using LaundrySystem.Api.Controllers.Base;
using LaundrySystem.BLL.Infrastructure.Interfaces;
using LaundrySystem.Domain.Model.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LaundrySystem.API.Controllers.Implementations
{
    /// <summary>
    /// Controller for managing service messages.
    /// </summary>
    [ApiController]
    [Route("api/service-messages")]
    public class ServiceMessagesController : BaseController<ServiceMessagesController>
    {
        private readonly IServiceMessageService _serviceMessageService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceMessagesController"/> class.
        /// </summary>
        public ServiceMessagesController(IServiceMessageService serviceMessageService, ILogger<ServiceMessagesController> logger)
            : base(logger)
        {
            _serviceMessageService = serviceMessageService;
        }

        /// <summary>
        /// Gets active service messages (within their active date range).
        /// </summary>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            try
            {
                var response = await _serviceMessageService.GetAllAsync(cancellationToken);
                if (!response.Success)
                {
                    return BadRequest(response.Message);
                }
                // Filter to only active messages (within date range)
                var now = DateTime.UtcNow;
                var active = response.Data?
                    .Where(m => now >= m.ActiveFrom && (m.ActiveTo == null || now <= m.ActiveTo))
                    .ToList();
                return Ok(active);
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        /// <summary>
        /// Gets a service message by ID.
        /// </summary>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _serviceMessageService.GetByIdAsync(id, cancellationToken);
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
        /// Creates a new service message (admin only).
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "AccountAdmin,BuildingAdmin")]
        public async Task<IActionResult> Insert([FromBody] ServiceMessageModel serviceMessageModel, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _serviceMessageService.InsertAsync(serviceMessageModel, cancellationToken);
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

        /// <summary>
        /// Updates an existing service message (admin only).
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "AccountAdmin,BuildingAdmin")]
        public async Task<IActionResult> Update(Guid id, [FromBody] ServiceMessageModel serviceMessageModel, CancellationToken cancellationToken)
        {
            try
            {
                serviceMessageModel.Id = id;
                var response = await _serviceMessageService.UpdateAsync(serviceMessageModel, cancellationToken);
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
        /// Deletes a service message (admin only).
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "AccountAdmin,BuildingAdmin")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _serviceMessageService.DeleteAsync(id, cancellationToken);
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
