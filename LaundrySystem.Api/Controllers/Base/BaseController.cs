using Microsoft.AspNetCore.Mvc;

namespace LaundrySystem.Api.Controllers.Base
{
    /// <summary>
    /// Base controller for API controllers.
    /// </summary>
    /// <typeparam name="T">The type of the controller.</typeparam>
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseController<T> : ControllerBase
    {
        /// <summary>
        /// The logger instance used for logging.
        /// </summary>
        protected readonly ILogger<T> Logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseController{T}"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        protected BaseController(ILogger<T> logger)
        {
            Logger = logger;
        }

        /// <summary>
        /// Handles the error and returns an error response.
        /// </summary>
        /// <param name="ex">The exception.</param>
        /// <returns>The error response.</returns>
        protected IActionResult HandleError(Exception ex)
        {
            Logger.LogError(ex, ex.Message);
            return StatusCode(500, "An unexpected error occurred. Please try again later.");
        }
    }
}