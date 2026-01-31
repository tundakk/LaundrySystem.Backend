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

                var pagedUsers = await _userManager.Users
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var users = new List<AppUserModel>();
                foreach (var user in pagedUsers)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    users.Add(new AppUserModel
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
                        Role = roles.FirstOrDefault(),
                    });
                }

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
        /// Gets the current authenticated user's details.
        /// </summary>
        /// <returns>The current user details.</returns>
        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                {
                    return Unauthorized(new ServiceResponse<AppUserModel>
                    {
                        Data = null,
                        Success = false,
                        Message = "User not authenticated."
                    });
                }

                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                {
                    return NotFound(new ServiceResponse<AppUserModel>
                    {
                        Data = null,
                        Success = false,
                        Message = "User not found."
                    });
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

                return Ok(new ServiceResponse<AppUserModel>
                {
                    Data = userModel,
                    Success = true,
                    Message = "User retrieved successfully."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching current user");

                return StatusCode(500, new ServiceResponse<AppUserModel>
                {
                    Data = null,
                    Success = false,
                    Message = "An error occurred while fetching the user."
                });
            }
        }

        /// <summary>
        /// Updates the current authenticated user's details.
        /// </summary>
        /// <param name="model">The user update model.</param>
        /// <returns>The updated user details.</returns>
        [Authorize]
        [HttpPut("me")]
        public async Task<IActionResult> UpdateCurrentUser([FromBody] AppUserUpdateModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ServiceResponse<object>
                {
                    Data = null,
                    Success = false,
                    Message = "Invalid model state."
                });
            }

            try
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                {
                    return Unauthorized(new ServiceResponse<object>
                    {
                        Data = null,
                        Success = false,
                        Message = "User not authenticated."
                    });
                }

                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                {
                    return NotFound(new ServiceResponse<object>
                    {
                        Data = null,
                        Success = false,
                        Message = "User not found."
                    });
                }

                // Update user properties
                if (model.PhoneNumber != null) user.PhoneNumber = model.PhoneNumber;
                if (model.PhoneNumberSecondary != null) user.PhoneNumberSecondary = model.PhoneNumberSecondary;
                user.EmailOptOut = model.EmailOptOut;
                user.SmsOptOut = model.SmsOptOut;

                // Handle PinCode if provided
                if (model.PinCode.HasValue)
                {
                    try
                    {
                        user.SetPinCode(model.PinCode.Value);
                    }
                    catch (ArgumentException ex)
                    {
                        return BadRequest(new ServiceResponse<object>
                        {
                            Data = null,
                            Success = false,
                            Message = ex.Message
                        });
                    }
                }

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return Ok(new ServiceResponse<AppUserModel>
                    {
                        Data = new AppUserModel
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
                        },
                        Success = true,
                        Message = "User updated successfully."
                    });
                }

                var errors = result.Errors.Select(e => e.Description).ToList();
                return BadRequest(new ServiceResponse<object>
                {
                    Data = null,
                    Success = false,
                    Message = string.Join("; ", errors)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating current user");

                return StatusCode(500, new ServiceResponse<object>
                {
                    Data = null,
                    Success = false,
                    Message = "An error occurred while updating the user."
                });
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
        /// Admin endpoint to edit user details (name, email, apartment, role).
        /// </summary>
        /// <param name="id">The user ID.</param>
        /// <param name="model">The admin edit model.</param>
        /// <returns>The updated user details.</returns>
        [Authorize(Roles = "AccountAdmin,SuperAdmin")]
        [HttpPut("admin/{id}")]
        public async Task<IActionResult> AdminEditUser(Guid id, [FromBody] AdminEditUserModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ServiceResponse<object>
                {
                    Data = null,
                    Success = false,
                    Message = "Invalid model state."
                });
            }

            try
            {
                var user = await _userManager.FindByIdAsync(id.ToString());
                if (user == null)
                {
                    return NotFound(new ServiceResponse<object>
                    {
                        Data = null,
                        Success = false,
                        Message = "User not found."
                    });
                }

                // Validate duplicate email
                var normalizedEmail = model.Email.ToUpperInvariant();
                var existingUser = await _userManager.Users
                    .FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail && u.Id != id);
                if (existingUser != null)
                {
                    return BadRequest(new ServiceResponse<object>
                    {
                        Data = null,
                        Success = false,
                        Message = "A user with this email already exists."
                    });
                }

                // Validate role exists
                if (!await _roleManager.RoleExistsAsync(model.Role))
                {
                    return BadRequest(new ServiceResponse<object>
                    {
                        Data = null,
                        Success = false,
                        Message = $"Role '{model.Role}' does not exist."
                    });
                }

                // Capture old values for audit log
                var oldUserName = user.UserName;
                var oldEmail = user.Email;
                var oldApartment = user.ApartmentNumber;
                var oldRoles = await _userManager.GetRolesAsync(user);

                // Update user properties
                user.UserName = model.UserName;
                user.Email = model.Email;
                user.ApartmentNumber = model.ApartmentNumber;

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    return BadRequest(new ServiceResponse<object>
                    {
                        Data = null,
                        Success = false,
                        Message = string.Join("; ", errors)
                    });
                }

                // Update role if changed
                if (!oldRoles.Contains(model.Role) || oldRoles.Count != 1)
                {
                    // Remove all existing roles and assign the new one
                    if (oldRoles.Any())
                    {
                        await _userManager.RemoveFromRolesAsync(user, oldRoles);
                    }
                    await _userManager.AddToRoleAsync(user, model.Role);
                }

                // Audit log
                var adminId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                _logger.LogInformation(
                    "Admin {AdminId} edited user {UserId}: UserName '{OldUserName}'->'{NewUserName}', Email '{OldEmail}'->'{NewEmail}', Apartment {OldApartment}->{NewApartment}, Role '{OldRole}'->'{NewRole}'",
                    adminId, id,
                    oldUserName, model.UserName,
                    oldEmail, model.Email,
                    oldApartment, model.ApartmentNumber,
                    string.Join(",", oldRoles), model.Role);

                return Ok(new ServiceResponse<AppUserModel>
                {
                    Data = new AppUserModel
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
                    },
                    Success = true,
                    Message = "User updated successfully."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in admin edit of user {UserId}", id);

                return StatusCode(500, new ServiceResponse<object>
                {
                    Data = null,
                    Success = false,
                    Message = "An error occurred while updating the user."
                });
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