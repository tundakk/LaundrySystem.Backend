using LaundrySystem.Api.Controllers.Base;
using LaundrySystem.BLL.Services.Interfaces;
using LaundrySystem.Domain.Model.Models;
using Microsoft.AspNetCore.Mvc;

namespace LaundrySystem.API.Controllers
{
    /// <summary>
    /// LaundryReservationsController
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class LaundryReservationsController : BaseController<LaundryReservationsController>
    {
        private readonly ILaundryReservationService _laundryreservationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="LaundryReservationsController"/> class.
        /// </summary>
        /// <param name="laundryreservationService">The LaundryReservation service.</param>
        /// <param name="logger">The logger.</param>
        public LaundryReservationsController(ILaundryReservationService laundryreservationService, ILogger<LaundryReservationsController> logger)
            : base(logger)
        {
            _laundryreservationService = laundryreservationService;
        }

        ///<inheritdoc/>
        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var response = _laundryreservationService.GetAll();
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
                var response = _laundryreservationService.GetById(id);
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
        public IActionResult Insert([FromBody] LaundryReservationModel laundryreservationModel)
        {
            try
            {
                var response = _laundryreservationService.Insert(laundryreservationModel);
                if (!response.Success)
                {
                    return BadRequest(response.Message);
                }
                if (response.Data == null)
                {
                    return BadRequest("Reservation failed. Please try again.");
                }
                return CreatedAtAction(nameof(GetById), new { id = response.Data.ReservationId }, response.Data);
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        ///<inheritdoc/>
        [HttpPut("<built-in function id>")]
        public IActionResult Update(int id, [FromBody] LaundryReservationModel laundryreservationModel)
        {
            try
            {
                laundryreservationModel.ReservationId = id;
                var response = _laundryreservationService.Update(laundryreservationModel);
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
                var response = _laundryreservationService.Delete(id);
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