using LaundrySystem.Api.Controllers.Base;
using LaundrySystem.BLL.Services.Interfaces;
using LaundrySystem.Domain.Model.Models;
using Microsoft.AspNetCore.Mvc;

namespace LaundrySystem.API.Controllers
{
    /// <summary>
    /// ServiceMessagesController
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ServiceMessagesController : BaseController<ServiceMessagesController>
    {
        private readonly IServiceMessageService _servicemessageService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceMessagesController"/> class.
        /// </summary>
        /// <param name="servicemessageService">The ServiceMessage service.</param>
        /// <param name="logger">The logger.</param>
        public ServiceMessagesController(IServiceMessageService servicemessageService, ILogger<ServiceMessagesController> logger)
            : base(logger)
        {
            _servicemessageService = servicemessageService;
        }

        ///<inheritdoc/>
        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var response = _servicemessageService.GetAll();
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
        [HttpGet("<built-in function id>")]
        public IActionResult GetById(int id)
        {
            try
            {
                var response = _servicemessageService.GetById(id);
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
        [HttpPost]
        public IActionResult Insert([FromBody] ServiceMessageModel servicemessageModel)
        {
            try
            {
                var response = _servicemessageService.Insert(servicemessageModel);
                if (!response.Success)
                {
                    return BadRequest(response.Message);
                }
                if (response.Data == null)
                {
                    return BadRequest("Failed to create the ServiceMessage");
                }
                return CreatedAtAction(nameof(GetById), new { id = response.Data.ServiceMessageId }, response.Data);
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        ///<inheritdoc/>
        [HttpPut("<built-in function id>")]
        public IActionResult Update(int id, [FromBody] ServiceMessageModel servicemessageModel)
        {
            try
            {
                servicemessageModel.ServiceMessageId = id;
                var response = _servicemessageService.Update(servicemessageModel);
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
        [HttpDelete("<built-in function id>")]
        public IActionResult Delete(int id)
        {
            try
            {
                var response = _servicemessageService.Delete(id);
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
    }
}