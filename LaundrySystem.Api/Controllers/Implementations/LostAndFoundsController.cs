using System.Security.Claims;
using LaundrySystem.Api.Controllers.Base;
using LaundrySystem.BLL.FileStorage;
using LaundrySystem.BLL.Infrastructure.Interfaces;
using LaundrySystem.Domain.Model.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LaundrySystem.API.Controllers.Implementations
{
    /// <summary>
    /// Controller for managing lost and found items.
    /// </summary>
    [ApiController]
    [Route("api/lost-found")]
    [Authorize]
    public class LostAndFoundsController : BaseController<LostAndFoundsController>
    {
        private readonly ILostAndFoundService _lostAndFoundService;
        private readonly IFileStorageService _fileStorageService;
        private const long MaxFileSizeBytes = 5 * 1024 * 1024; // 5MB

        /// <summary>
        /// Initializes a new instance of the <see cref="LostAndFoundsController"/> class.
        /// </summary>
        /// <param name="lostAndFoundService">The LostAndFound service.</param>
        /// <param name="fileStorageService">The file storage service.</param>
        /// <param name="logger">The logger.</param>
        public LostAndFoundsController(
            ILostAndFoundService lostAndFoundService,
            IFileStorageService fileStorageService,
            ILogger<LostAndFoundsController> logger)
            : base(logger)
        {
            _lostAndFoundService = lostAndFoundService;
            _fileStorageService = fileStorageService;
        }

        /// <summary>
        /// Gets a paginated list of lost and found items.
        /// </summary>
        /// <param name="page">Page number (default 1).</param>
        /// <param name="pageSize">Items per page (default 20, max 100).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Paginated list of items with image URLs.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllAsync(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken cancellationToken = default)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1) pageSize = 20;
                if (pageSize > 100) pageSize = 100;

                var response = await _lostAndFoundService.GetAllAsync(cancellationToken);
                if (!response.Success)
                {
                    return BadRequest(response.Message);
                }

                var allItems = response.Data?.OrderByDescending(x => x.DateFound).ToList() ?? [];
                var totalCount = allItems.Count;
                var items = allItems.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                return Ok(new
                {
                    items,
                    totalCount,
                    page,
                    pageSize,
                    totalPages = (int)Math.Ceiling((double)totalCount / pageSize)
                });
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        /// <summary>
        /// Gets a single lost and found item by ID.
        /// </summary>
        /// <param name="id">The item ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The lost and found item details.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _lostAndFoundService.GetByIdAsync(id, cancellationToken);
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
        /// Reports a found item with an uploaded image (multipart form data).
        /// </summary>
        /// <param name="buildingId">The building where the item was found.</param>
        /// <param name="description">Optional description of the item.</param>
        /// <param name="file">The image file (JPEG or PNG, max 5MB).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created lost and found item.</returns>
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> ReportAsync(
            [FromForm] Guid buildingId,
            [FromForm] string? description,
            IFormFile file,
            CancellationToken cancellationToken)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest("No file uploaded.");
                }

                // Validate file size (5MB max)
                if (file.Length > MaxFileSizeBytes)
                {
                    return BadRequest("File size exceeds maximum allowed size of 5MB.");
                }

                // Validate file type (JPEG/PNG only)
                var allowedTypes = new[] { "image/jpeg", "image/png" };
                if (!allowedTypes.Contains(file.ContentType.ToLowerInvariant()))
                {
                    return BadRequest("Invalid file type. Only JPEG and PNG images are accepted.");
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                await using var stream = file.OpenReadStream();
                var fileUrl = await _fileStorageService.UploadAsync(stream, file.FileName, "lostandfound", cancellationToken);

                var model = new LostAndFoundModel
                {
                    BuildingId = buildingId,
                    Description = description,
                    PictureUrl = fileUrl,
                    DateFound = DateTime.UtcNow,
                    ReportedByUserId = userId != null ? Guid.Parse(userId) : null
                };

                var response = await _lostAndFoundService.InsertAsync(model, cancellationToken);
                if (!response.Success)
                {
                    await _fileStorageService.DeleteAsync(fileUrl, cancellationToken);
                    return BadRequest(response.Message);
                }

                return CreatedAtAction(nameof(GetByIdAsync), new { id = response.Data!.Id }, response.Data);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        /// <summary>
        /// Deletes a lost and found item. Only the owner or an admin can delete.
        /// </summary>
        /// <param name="id">The item ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Success message.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                // Check ownership or admin role
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var isAdmin = User.IsInRole("AccountAdmin") || User.IsInRole("BuildingAdmin");

                var itemResponse = await _lostAndFoundService.GetByIdAsync(id, cancellationToken);
                if (!itemResponse.Success || itemResponse.Data == null)
                {
                    return NotFound("Item not found.");
                }

                if (!isAdmin && itemResponse.Data.ReportedByUserId?.ToString() != userId)
                {
                    return Forbid();
                }

                // Delete the image file
                if (!string.IsNullOrEmpty(itemResponse.Data.PictureUrl))
                {
                    await _fileStorageService.DeleteAsync(itemResponse.Data.PictureUrl, cancellationToken);
                }

                var response = await _lostAndFoundService.DeleteAsync(id, cancellationToken);
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
