using LaundrySystem.DAL.DataModel;
using Microsoft.EntityFrameworkCore;

namespace LaundrySystem.Api.BackgroundServices
{
    /// <summary>
    /// Hangfire recurring job that expires old desired-timeslot subscriptions.
    /// </summary>
    public class SubscriptionExpiryJob
    {
        private readonly DataContext _dataContext;
        private readonly ILogger<SubscriptionExpiryJob> _logger;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionExpiryJob"/> class.
        /// </summary>
        /// <param name="dataContext">The data context.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="configuration">The configuration.</param>
        public SubscriptionExpiryJob(
            DataContext dataContext,
            ILogger<SubscriptionExpiryJob> logger,
            IConfiguration configuration)
        {
            _dataContext = dataContext;
            _logger = logger;
            _configuration = configuration;
        }

        /// <summary>
        /// Removes desired-timeslot subscriptions older than the configured expiry period,
        /// and subscriptions whose timeslot has already passed.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task ExecuteAsync()
        {
            var expiryDays = _configuration.GetValue("BackgroundJobs:SubscriptionExpiryDays", 30);
            var cutoffDate = DateTime.UtcNow.AddDays(-expiryDays);
            var now = DateTime.UtcNow;

            // Delete subscriptions that are too old OR whose timeslot has already passed
            var expiredCount = await _dataContext.DesiredTimeslots
                .Where(ds => ds.CreatedAt < cutoffDate
                    || ds.NotificationSent
                    || ds.Timeslot.SlotTime.Start < now)
                .ExecuteDeleteAsync();

            if (expiredCount > 0)
            {
                _logger.LogInformation("Expired {Count} old desired-timeslot subscriptions", expiredCount);
            }
            else
            {
                _logger.LogDebug("No desired-timeslot subscriptions to expire");
            }
        }
    }
}
