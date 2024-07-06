using LaundrySystem.Api.Controllers.Base;
using LaundrySystem.BLL.Services.Interfaces;
using LaundrySystem.Domain.Model.Models;
using Microsoft.AspNetCore.Mvc;

namespace LaundrySystem.API.Controllers
{
    /// <summary>
    /// ChatMessagesController
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ChatMessagesController : BaseController<ChatMessagesController>
    {
        private readonly IChatMessageService _chatmessageService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChatMessagesController"/> class.
        /// </summary>
        /// <param name="chatmessageService">The ChatMessage service.</param>
        /// <param name="logger">The logger.</param>
        public ChatMessagesController(IChatMessageService chatmessageService, ILogger<ChatMessagesController> logger)
            : base(logger)
        {
            _chatmessageService = chatmessageService;
        }

        ///<inheritdoc/>
        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var response = _chatmessageService.GetAll();
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
                var response = _chatmessageService.GetById(id);
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
        public IActionResult Insert([FromBody] ChatMessageModel chatmessageModel)
        {
            try
            {
                var response = _chatmessageService.Insert(chatmessageModel);
                if (!response.Success)
                {
                    return BadRequest(response.Message);
                }
                if (response.Data == null)
                {
                    return BadRequest("ChatMessage could not be created");
                }
                return CreatedAtAction(nameof(GetById), new { id = response.Data.ChatMessageId }, response.Data);
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        ///<inheritdoc/>
        [HttpPut("<built-in function id>")]
        public IActionResult Update(int id, [FromBody] ChatMessageModel chatmessageModel)
        {
            try
            {
                chatmessageModel.ChatMessageId = id;
                var response = _chatmessageService.Update(chatmessageModel);
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
                var response = _chatmessageService.Delete(id);
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