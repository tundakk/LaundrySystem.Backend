using LaundrySystem.Api.Controllers.Base;
using LaundrySystem.BLL.Services.Interfaces;
using LaundrySystem.Domain.Model.Models;
using Microsoft.AspNetCore.Mvc;

namespace LaundrySystem.API.Controllers
{
    /// <summary>
    /// LostAndFoundsController
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class LostAndFoundsController : BaseController<LostAndFoundsController>
    {
        private readonly ILostAndFoundService _lostandfoundService;

        /// <summary>
        /// Initializes a new instance of the <see cref="LostAndFoundsController"/> class.
        /// </summary>
        /// <param name="lostandfoundService">The LostAndFound service.</param>
        /// <param name="logger">The logger.</param>
        public LostAndFoundsController(ILostAndFoundService lostandfoundService, ILogger<LostAndFoundsController> logger)
            : base(logger)
        {
            _lostandfoundService = lostandfoundService;
        }

        ///<inheritdoc/>
        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var response = _lostandfoundService.GetAll();
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
                var response = _lostandfoundService.GetById(id);
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
        public IActionResult Insert([FromBody] LostAndFoundModel lostandfoundModel)
        {
            try
            {
                var response = _lostandfoundService.Insert(lostandfoundModel);
                if (!response.Success)
                {
                    return BadRequest(response.Message);
                }
                if (response.Data == null)
                {
                    return BadRequest("Failed to insert LostAndFound");
                }
                return CreatedAtAction(nameof(GetById), new { id = response.Data.LostAndFoundId }, response.Data);
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        ///<inheritdoc/>
        [HttpPut("<built-in function id>")]
        public IActionResult Update(int id, [FromBody] LostAndFoundModel lostandfoundModel)
        {
            try
            {
                lostandfoundModel.LostAndFoundId = id;
                var response = _lostandfoundService.Update(lostandfoundModel);
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
                var response = _lostandfoundService.Delete(id);
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