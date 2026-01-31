using LaundrySystem.BLL.FileStorage;
using LaundrySystem.DAL.DataModel;
using Microsoft.EntityFrameworkCore;

namespace LaundrySystem.Api.BackgroundServices
{
    /// <summary>
    /// Hangfire recurring job that cleans up old lost and found items past the retention period.
    /// </summary>
    public class LostAndFoundCleanupJob
    {
        private readonly DataContext _dataContext;
        private readonly IFileStorageService _fileStorageService;
        private readonly ILogger<LostAndFoundCleanupJob> _logger;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="LostAndFoundCleanupJob"/> class.
        /// </summary>
        /// <param name="dataContext">The data context.</param>
        /// <param name="fileStorageService">The file storage service.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="configuration">The configuration.</param>
        public LostAndFoundCleanupJob(
            DataContext dataContext,
            IFileStorageService fileStorageService,
            ILogger<LostAndFoundCleanupJob> logger,
            IConfiguration configuration)
        {
            _dataContext = dataContext;
            _fileStorageService = fileStorageService;
            _logger = logger;
            _configuration = configuration;
        }

        /// <summary>
        /// Removes lost and found items older than the configured retention period.
        /// Deletes associated image files before removing database records.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task ExecuteAsync()
        {
            var retentionDays = _configuration.GetValue("BackgroundJobs:LostAndFoundRetentionDays", 90);
            var cutoffDate = DateTime.UtcNow.AddDays(-retentionDays);

            var oldItems = await _dataContext.LostAndFoundItems
                .Where(lf => lf.DateFound < cutoffDate)
                .ToListAsync();

            if (!oldItems.Any())
            {
                _logger.LogDebug("No old lost and found items to clean up");
                return;
            }

            _logger.LogInformation("Cleaning up {Count} lost and found items older than {Days} days", oldItems.Count, retentionDays);

            foreach (var item in oldItems)
            {
                try
                {
                    if (!string.IsNullOrEmpty(item.PictureUrl))
                    {
                        await _fileStorageService.DeleteAsync(item.PictureUrl);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to delete image file for lost and found item {ItemId}", item.Id);
                }
            }

            _dataContext.LostAndFoundItems.RemoveRange(oldItems);
            await _dataContext.SaveChangesAsync();

            _logger.LogInformation("Cleaned up {Count} old lost and found items", oldItems.Count);
        }
    }
}
