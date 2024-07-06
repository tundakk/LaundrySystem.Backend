using LaundrySystem.Api.Controllers.Base;
using LaundrySystem.BLL.Services.Interfaces;
using LaundrySystem.Domain.Model.Models;
using Microsoft.AspNetCore.Mvc;

namespace LaundrySystem.API.Controllers
{
    /// <summary>
    /// AddresssController
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AddresssController : BaseController<AddresssController>
    {
        private readonly IAddressService _addressService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddresssController"/> class.
        /// </summary>
        /// <param name="addressService">The Address service.</param>
        /// <param name="logger">The logger.</param>
        public AddresssController(IAddressService addressService, ILogger<AddresssController> logger)
            : base(logger)
        {
            _addressService = addressService;
        }

        ///<inheritdoc/>
        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var response = _addressService.GetAll();
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
                var response = _addressService.GetById(id);
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
        public IActionResult Insert([FromBody] AddressModel addressModel)
        {
            try
            {
                var response = _addressService.Insert(addressModel);
                if (!response.Success)
                {
                    return BadRequest(response.Message);
                }
                if (response.Data == null)
                {
                    return BadRequest("Address could not be created.");
                }
                return CreatedAtAction(nameof(GetById), new { id = response.Data.AddressId }, response.Data);
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        ///<inheritdoc/>
        [HttpPut("<built-in function id>")]
        public IActionResult Update(int id, [FromBody] AddressModel addressModel)
        {
            try
            {
                addressModel.AddressId = id;
                var response = _addressService.Update(addressModel);
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
                var response = _addressService.Delete(id);
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