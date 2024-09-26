using LaundrySystem.Domain.Model.Models;
using LaundrySystem.Domain.Model.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LaundrySystem.API.Controllers
{
    /// <summary>
    /// Controller for managing application users.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AppUsersController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<AppUsersController> _logger;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppUsersController"/> class.
        /// </summary>
        /// <param name="userManager">The user manager.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="roleManager">/// Provides the APIs for managing roles in a persistence store.</param>
        ///
        public AppUsersController(UserManager<AppUser> userManager, RoleManager<IdentityRole<Guid>> roleManager, ILogger<AppUsersController> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        /// <summary>
        /// Gets all users with pagination.
        /// </summary>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <returns>A list of users.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllUsers(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var totalUsers = await _userManager.Users.CountAsync();

                var users = await _userManager.Users
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(user => new AppUserModel
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        Email = user.Email,
                        PhoneNumber = user.PhoneNumber,
                        ApartmentNumber = user.ApartmentNumber,
                        PhoneNumberSecondary = user.PhoneNumberSecondary,
                        EmailOptOut = user.EmailOptOut,
                        SmsOptOut = user.SmsOptOut,
                        PinCode = user.PinCode,
                    })
                    .ToListAsync();

                var response = new ServiceResponse<object>
                {
                    Data = new
                    {
                        TotalUsers = totalUsers,
                        PageNumber = pageNumber,
                        PageSize = pageSize,
                        Users = users
                    },
                    Success = true,
                    Message = "Users retrieved successfully."
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching users");

                var response = new ServiceResponse<object>
                {
                    Data = null,
                    Success = false,
                    Message = "An error occurred while fetching users."
                };

                return StatusCode(500, response);
            }
        }

        /// <summary>
        /// Gets a user by ID.
        /// </summary>
        /// <param name="id">The user ID.</param>
        /// <returns>The user details.</returns>
        [Authorize(Roles = "User")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id.ToString());
                if (user == null)
                {
                    var response = new ServiceResponse<AppUserModel>
                    {
                        Data = null,
                        Success = false,
                        Message = "User not found."
                    };
                    return NotFound(response);
                }

                var userModel = new AppUserModel
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    ApartmentNumber = user.ApartmentNumber,
                    PhoneNumberSecondary = user.PhoneNumberSecondary,
                    EmailOptOut = user.EmailOptOut,
                    SmsOptOut = user.SmsOptOut,
                    PinCode = user.PinCode,
                };

                var responseSuccess = new ServiceResponse<AppUserModel>
                {
                    Data = userModel,
                    Success = true,
                    Message = "User retrieved successfully."
                };

                return Ok(responseSuccess);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user by id");

                var response = new ServiceResponse<AppUserModel>
                {
                    Data = null,
                    Success = false,
                    Message = "An error occurred while fetching the user."
                };

                return StatusCode(500, response);
            }
        }

        /// <summary>
        /// Updates a user by ID.
        /// </summary>
        /// <param name="id">The user ID.</param>
        /// <param name="model">The user update model.</param>
        /// <returns>The updated user details.</returns>
        [Authorize(Roles = "User")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] AppUserUpdateModel model)
        {
            if (!ModelState.IsValid)
            {
                var response = new ServiceResponse<object>
                {
                    Data = null,
                    Success = false,
                    Message = "Invalid model state."
                };
                return BadRequest(response);
            }

            try
            {
                var user = await _userManager.FindByIdAsync(id.ToString());
                if (user == null)
                {
                    var response = new ServiceResponse<object>
                    {
                        Data = null,
                        Success = false,
                        Message = "User not found."
                    };
                    return NotFound(response);
                }

                // Update user properties
                user.UserName = user.UserName;
                user.PhoneNumber = model.PhoneNumber;
                user.ApartmentNumber = model.ApartmentNumber;
                user.PhoneNumberSecondary = model.PhoneNumberSecondary;
                user.EmailOptOut = model.EmailOptOut;
                user.SmsOptOut = model.SmsOptOut;
                user.PinCode = user.PinCode;

                // Handle PinCode if necessary
                if (model.PinCode.HasValue)
                {
                    try
                    {
                        user.SetPinCode(model.PinCode.Value);
                    }
                    catch (ArgumentException ex)
                    {
                        var response = new ServiceResponse<object>
                        {
                            Data = null,
                            Success = false,
                            Message = ex.Message
                        };
                        return BadRequest(response);
                    }
                }

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    var response = new ServiceResponse<AppUserModel>
                    {
                        Data = new AppUserModel
                        {
                            UserName = user.UserName,
                            Email = user.Email,
                            PhoneNumber = user.PhoneNumber,
                            ApartmentNumber = user.ApartmentNumber,
                            PhoneNumberSecondary = user.PhoneNumberSecondary,
                            EmailOptOut = user.EmailOptOut,
                            SmsOptOut = user.SmsOptOut,
                            PinCode = user.PinCode,
                        },
                        Success = true,
                        Message = "User updated successfully."
                    };
                    return Ok(response);
                }

                // Collect and return errors
                var errors = result.Errors.Select(e => e.Description).ToList();
                var responseError = new ServiceResponse<object>
                {
                    Data = null,
                    Success = false,
                    Message = string.Join("; ", errors)
                };
                return BadRequest(responseError);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user");

                var response = new ServiceResponse<object>
                {
                    Data = null,
                    Success = false,
                    Message = "An error occurred while updating the user."
                };

                return StatusCode(500, response);
            }
        }

        /// <summary>
        /// Deletes a user by ID.
        /// </summary>
        /// <param name="id">The user ID.</param>
        /// <returns>A response indicating the result of the delete operation.</returns>
        [Authorize(Roles = "Admin, Manager")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id.ToString());
                if (user == null)
                {
                    var response = new ServiceResponse<object>
                    {
                        Data = null,
                        Success = false,
                        Message = "User not found."
                    };
                    return NotFound(response);
                }

                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    var response = new ServiceResponse<object>
                    {
                        Data = null,
                        Success = true,
                        Message = "User deleted successfully."
                    };
                    return Ok(response);
                }

                // Collect and return errors
                var errors = result.Errors.Select(e => e.Description).ToList();
                var responseError = new ServiceResponse<object>
                {
                    Data = null,
                    Success = false,
                    Message = string.Join("; ", errors)
                };
                return BadRequest(responseError);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user");

                var response = new ServiceResponse<object>
                {
                    Data = null,
                    Success = false,
                    Message = "An error occurred while deleting the user."
                };

                return StatusCode(500, response);
            }
        }

        /// <summary>
        /// Assigns a role to a user.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="role">The role to assign.</param>
        /// <returns>A response indicating the result of the assign role operation.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPost("{userId}/assign-role")]
        public async Task<IActionResult> AssignRole(Guid userId, [FromBody] string role)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)

            {
                return NotFound(new { Message = "User not found" });
            }

            if (!await _roleManager.RoleExistsAsync(role))

            {
                return BadRequest(new { Message = "Role does not exist" });
            }

            var result = await _userManager.AddToRoleAsync(user, role);

            if (result.Succeeded)

            {
                return Ok(new { Message = "Role assigned successfully" });
            }

            return BadRequest(result.Errors);
        }

        /// <summary>
        /// Removes a role from a user.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="role">The role to remove.</param>
        /// <returns>A response indicating the result of the remove role operation.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPost("{userId}/remove-role")]
        public async Task<IActionResult> RemoveRole(Guid userId, [FromBody] string role)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)

            {
                return NotFound(new { Message = "User not found" });
            }

            if (!await _roleManager.RoleExistsAsync(role))

            {
                return BadRequest(new { Message = "Role does not exist" });
            }

            var result = await _userManager.RemoveFromRoleAsync(user, role);

            if (result.Succeeded)

            {
                return Ok(new { Message = "Role removed successfully" });
            }

            return BadRequest(result.Errors);
        }
    }
}