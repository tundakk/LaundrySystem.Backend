using LaundrySystem.Api.Controllers.Base;
using LaundrySystem.BLL.Services.Interfaces;
using LaundrySystem.Domain.Model.Models;
using Microsoft.AspNetCore.Mvc;

namespace LaundrySystem.API.Controllers
{
    /// <summary>
    /// HouseholdsController
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class HouseholdsController : BaseController<HouseholdsController>
    {
        private readonly IHouseholdService _householdService;

        /// <summary>
        /// Initializes a new instance of the <see cref="HouseholdsController"/> class.
        /// </summary>
        /// <param name="householdService">The Household service.</param>
        /// <param name="logger">The logger.</param>
        public HouseholdsController(IHouseholdService householdService, ILogger<HouseholdsController> logger)
            : base(logger)
        {
            _householdService = householdService;
        }

        ///<inheritdoc/>
        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var response = _householdService.GetAll();
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
                var response = _householdService.GetById(id);
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
        public IActionResult Insert([FromBody] HouseholdModel householdModel)
        {
            try
            {
                var response = _householdService.Insert(householdModel);
                if (!response.Success)
                {
                    return BadRequest(response.Message);
                }
                if (response.Data == null)
                {
                    return BadRequest("Household could not be created.");
                }
                return CreatedAtAction(nameof(GetById), new { id = response.Data.HouseholdId }, response.Data);
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        ///<inheritdoc/>
        [HttpPut("<built-in function id>")]
        public IActionResult Update(int id, [FromBody] HouseholdModel householdModel)
        {
            try
            {
                householdModel.HouseholdId = id;
                var response = _householdService.Update(householdModel);
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
                var response = _householdService.Delete(id);
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