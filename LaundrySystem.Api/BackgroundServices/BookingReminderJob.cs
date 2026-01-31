using LaundrySystem.BLL.Notifications;
using LaundrySystem.DAL.Repos.Interfaces;

namespace LaundrySystem.Api.BackgroundServices
{
    /// <summary>
    /// Hangfire recurring job that sends reminders for upcoming bookings.
    /// </summary>
    public class BookingReminderJob
    {
        private readonly IBookingRepo _bookingRepo;
        private readonly INotificationService _notificationService;
        private readonly ILogger<BookingReminderJob> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="BookingReminderJob"/> class.
        /// </summary>
        /// <param name="bookingRepo">The booking repository.</param>
        /// <param name="notificationService">The notification service.</param>
        /// <param name="logger">The logger.</param>
        public BookingReminderJob(
            IBookingRepo bookingRepo,
            INotificationService notificationService,
            ILogger<BookingReminderJob> logger)
        {
            _bookingRepo = bookingRepo;
            _notificationService = notificationService;
            _logger = logger;
        }

        /// <summary>
        /// Checks for upcoming bookings and sends reminder notifications.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task ExecuteAsync()
        {
            var ct = CancellationToken.None;
            var bookingsNeedingReminder = await _bookingRepo.GetBookingsNeedingReminderAsync(ct);

            if (!bookingsNeedingReminder.Any())
            {
                _logger.LogDebug("No bookings need reminders at this time");
                return;
            }

            _logger.LogInformation("Found {Count} bookings needing reminders", bookingsNeedingReminder.Count());

            foreach (var booking in bookingsNeedingReminder)
            {
                try
                {
                    var sent = await _notificationService.SendBookingReminderAsync(booking.Id, ct);
                    if (sent)
                    {
                        booking.MarkReminderSent();
                        await _bookingRepo.UpdateAsync(booking, ct);
                        _logger.LogInformation("Sent reminder for booking {BookingId}", booking.Id);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send reminder for booking {BookingId}", booking.Id);
                }
            }
        }
    }
}
