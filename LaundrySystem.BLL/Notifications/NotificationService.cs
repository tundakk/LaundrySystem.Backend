using LaundrySystem.BLL.SMS;
using LaundrySystem.DAL.DataModel;
using LaundrySystem.Domain.Model.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using sib_api_v3_sdk.Api;
using sib_api_v3_sdk.Client;
using sib_api_v3_sdk.Model;

namespace LaundrySystem.BLL.Notifications
{
    /// <summary>
    /// Service for sending notifications via email and SMS.
    /// </summary>
    public class NotificationService : INotificationService
    {
        private readonly DataContext _dataContext;
        private readonly ISMSService _smsService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<NotificationService> _logger;
        private readonly TransactionalEmailsApi? _emailApi;
        private readonly bool _emailConfigured;

        /// <summary>
        /// Number of recent users to notify for Lost and Found items.
        /// </summary>
        private const int RecentUsersToNotify = 5;

        public NotificationService(
            DataContext dataContext,
            ISMSService smsService,
            IConfiguration configuration,
            ILogger<NotificationService> logger)
        {
            _dataContext = dataContext;
            _smsService = smsService;
            _configuration = configuration;
            _logger = logger;

            // Initialize email API
            var brevoApiKey = _configuration["BrevoApi:ApiKey"];
            _emailConfigured = !string.IsNullOrWhiteSpace(brevoApiKey) && brevoApiKey != "dev-placeholder-key";

            if (_emailConfigured)
            {
                try
                {
                    Configuration.Default.ApiKey["api-key"] = brevoApiKey;
                    _emailApi = new TransactionalEmailsApi();
                    _logger.LogInformation("Email notification service initialized successfully");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to initialize email API");
                    _emailConfigured = false;
                }
            }
            else
            {
                _logger.LogWarning("Email service is not configured. Emails will be logged but not sent.");
            }
        }

        /// <inheritdoc />
        public async Task<int> NotifyTimeslotAvailableAsync(Guid timeslotId, CancellationToken ct = default)
        {
            try
            {
                // Get all unnotified subscriptions for this timeslot with user info
                var subscriptions = await _dataContext.Set<DesiredTimeslot>()
                    .Include(dt => dt.User)
                    .Include(dt => dt.Timeslot)
                        .ThenInclude(t => t.Room)
                    .Where(dt => dt.TimeslotId == timeslotId && !dt.NotificationSent)
                    .ToListAsync(ct);

                if (!subscriptions.Any())
                {
                    _logger.LogDebug("No subscriptions to notify for timeslot {TimeslotId}", timeslotId);
                    return 0;
                }

                var notifiedCount = 0;
                var timeslot = subscriptions.First().Timeslot;
                var roomName = timeslot.Room?.Name ?? "Laundry Room";
                var slotTime = timeslot.SlotTime;

                foreach (var subscription in subscriptions)
                {
                    var user = subscription.User;
                    if (user == null) continue;

                    var subject = "Vasketid er nu ledig! / Laundry time is now available!";
                    var message = $@"
                        <p>Hej {user.UserName},</p>
                        <p>En vasketid du har abonneret på er nu ledig:</p>
                        <ul>
                            <li><strong>Rum:</strong> {roomName}</li>
                            <li><strong>Tid:</strong> {slotTime.Start:dd/MM/yyyy HH:mm} - {slotTime.End:HH:mm}</li>
                        </ul>
                        <p>Log ind for at booke tiden.</p>
                        <hr/>
                        <p>Hi {user.UserName},</p>
                        <p>A laundry time you subscribed to is now available:</p>
                        <ul>
                            <li><strong>Room:</strong> {roomName}</li>
                            <li><strong>Time:</strong> {slotTime.Start:dd/MM/yyyy HH:mm} - {slotTime.End:HH:mm}</li>
                        </ul>
                        <p>Log in to book the time.</p>
                    ";

                    var smsMessage = $"Vasketid ledig: {roomName}, {slotTime.Start:dd/MM HH:mm}-{slotTime.End:HH:mm}. Log ind for at booke.";

                    if (await SendNotificationToUserAsync(user, subject, message, smsMessage, ct))
                    {
                        subscription.MarkNotificationAsSent();
                        notifiedCount++;
                    }

                    // Create in-app notification
                    _dataContext.Set<Notification>().Add(new Notification
                    {
                        UserId = user.Id,
                        Title = "Laundry time available",
                        Message = $"A timeslot you subscribed to is now available: {roomName}, {slotTime.Start:dd/MM/yyyy HH:mm} - {slotTime.End:HH:mm}",
                        Link = "/bookings",
                        CreatedAt = DateTime.UtcNow
                    });
                }

                await _dataContext.SaveChangesAsync(ct);
                _logger.LogInformation("Notified {Count} users about available timeslot {TimeslotId}", notifiedCount, timeslotId);

                return notifiedCount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error notifying users about available timeslot {TimeslotId}", timeslotId);
                return 0;
            }
        }

        /// <inheritdoc />
        public async Task<int> NotifyLostAndFoundAsync(Guid buildingId, string? pictureUrl, string? description, CancellationToken ct = default)
        {
            try
            {
                // Get users who had bookings in the past 48 hours in this building
                var cutoff = DateTime.UtcNow.AddHours(-48);
                var recentBookings = await _dataContext.Set<Booking>()
                    .Include(b => b.User)
                    .Include(b => b.Timeslot)
                        .ThenInclude(t => t.Room)
                    .Where(b => b.Timeslot.Room.Building.Id == buildingId && b.CreatedAt >= cutoff)
                    .OrderByDescending(b => b.CreatedAt)
                    .ToListAsync(ct);

                var uniqueUsers = recentBookings
                    .Select(b => b.User)
                    .Where(u => u != null)
                    .DistinctBy(u => u.Id)
                    .ToList();

                if (!uniqueUsers.Any())
                {
                    _logger.LogDebug("No recent users to notify for lost and found in building {BuildingId}", buildingId);
                    return 0;
                }

                var notifiedCount = 0;
                var building = await _dataContext.Set<Building>()
                    .FirstOrDefaultAsync(b => b.Id == buildingId, ct);
                var buildingName = building?.Name ?? "bygningen";

                foreach (var user in uniqueUsers)
                {
                    var subject = "Fundet genstand i vaskerummet / Found item in laundry room";
                    var imageHtml = !string.IsNullOrEmpty(pictureUrl)
                        ? $"<p><img src=\"{pictureUrl}\" alt=\"Found item\" style=\"max-width: 300px;\"/></p>"
                        : "";
                    var descriptionHtml = !string.IsNullOrEmpty(description)
                        ? $"<p><strong>Beskrivelse:</strong> {description}</p>"
                        : "";

                    var message = $@"
                        <p>Hej {user.UserName},</p>
                        <p>Der er blevet fundet en genstand i vaskerummet i {buildingName}.</p>
                        {descriptionHtml}
                        {imageHtml}
                        <p>Hvis dette tilhører dig, så hent det venligst i vaskerummet.</p>
                        <hr/>
                        <p>Hi {user.UserName},</p>
                        <p>An item has been found in the laundry room at {buildingName}.</p>
                        {(!string.IsNullOrEmpty(description) ? $"<p><strong>Description:</strong> {description}</p>" : "")}
                        {imageHtml}
                        <p>If this belongs to you, please pick it up from the laundry room.</p>
                    ";

                    var smsMessage = $"Fundet i vaskerum ({buildingName}): {description ?? "Se email for billede"}";

                    if (await SendNotificationToUserAsync(user, subject, message, smsMessage, ct))
                    {
                        notifiedCount++;
                    }

                    // Create in-app notification
                    var inAppTitle = "Found item in laundry room";
                    var inAppMessage = !string.IsNullOrEmpty(description)
                        ? $"An item was found in the laundry room at {buildingName}: {description}"
                        : $"An item was found in the laundry room at {buildingName}. Check if it's yours.";

                    _dataContext.Set<Notification>().Add(new Notification
                    {
                        UserId = user.Id,
                        Title = inAppTitle,
                        Message = inAppMessage,
                        Link = "/lost-found",
                        CreatedAt = DateTime.UtcNow
                    });
                }

                await _dataContext.SaveChangesAsync(ct);
                _logger.LogInformation("Notified {Count} users about lost and found item in building {BuildingId}", notifiedCount, buildingId);
                return notifiedCount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error notifying users about lost and found in building {BuildingId}", buildingId);
                return 0;
            }
        }

        /// <inheritdoc />
        public async Task<bool> SendBookingReminderAsync(Guid bookingId, CancellationToken ct = default)
        {
            try
            {
                var booking = await _dataContext.Set<Booking>()
                    .Include(b => b.User)
                    .Include(b => b.Timeslot)
                        .ThenInclude(t => t.Room)
                    .FirstOrDefaultAsync(b => b.Id == bookingId, ct);

                if (booking == null)
                {
                    _logger.LogWarning("Booking {BookingId} not found for reminder", bookingId);
                    return false;
                }

                var user = booking.User;
                var timeslot = booking.Timeslot;
                var roomName = timeslot.Room?.Name ?? "Laundry Room";

                var subject = "Påmindelse: Din vasketid nærmer sig / Reminder: Your laundry time is coming up";
                var message = $@"
                    <p>Hej {user.UserName},</p>
                    <p>Dette er en påmindelse om din kommende vasketid:</p>
                    <ul>
                        <li><strong>Rum:</strong> {roomName}</li>
                        <li><strong>Tid:</strong> {timeslot.SlotTime.Start:dd/MM/yyyy HH:mm} - {timeslot.SlotTime.End:HH:mm}</li>
                    </ul>
                    <p>Husk at møde op til tiden!</p>
                    <hr/>
                    <p>Hi {user.UserName},</p>
                    <p>This is a reminder about your upcoming laundry time:</p>
                    <ul>
                        <li><strong>Room:</strong> {roomName}</li>
                        <li><strong>Time:</strong> {timeslot.SlotTime.Start:dd/MM/yyyy HH:mm} - {timeslot.SlotTime.End:HH:mm}</li>
                    </ul>
                    <p>Remember to show up on time!</p>
                ";

                var smsMessage = $"Påmindelse: Vasketid {timeslot.SlotTime.Start:dd/MM HH:mm} i {roomName}";

                var result = await SendNotificationToUserAsync(user, subject, message, smsMessage, ct);

                if (result)
                {
                    _logger.LogInformation("Sent booking reminder for booking {BookingId}", bookingId);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending booking reminder for {BookingId}", bookingId);
                return false;
            }
        }

        /// <inheritdoc />
        public async Task<bool> SendNotificationAsync(Guid userId, string subject, string message, CancellationToken ct = default)
        {
            try
            {
                var user = await _dataContext.Set<AppUser>()
                    .FirstOrDefaultAsync(u => u.Id == userId, ct);

                if (user == null)
                {
                    _logger.LogWarning("User {UserId} not found for notification", userId);
                    return false;
                }

                // Strip HTML for SMS
                var smsMessage = System.Text.RegularExpressions.Regex.Replace(message, "<[^>]+>", "")
                    .Replace("&nbsp;", " ")
                    .Trim();
                if (smsMessage.Length > 160)
                {
                    smsMessage = smsMessage.Substring(0, 157) + "...";
                }

                return await SendNotificationToUserAsync(user, subject, message, smsMessage, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending notification to user {UserId}", userId);
                return false;
            }
        }

        /// <summary>
        /// Sends notification to a user via email and/or SMS based on their preferences.
        /// </summary>
        private async Task<bool> SendNotificationToUserAsync(AppUser user, string subject, string htmlMessage, string smsMessage, CancellationToken ct)
        {
            var emailSent = false;
            var smsSent = false;

            // Check user preferences (use Settings override or fall back to legacy flags)
            var emailEnabled = user.Settings.EmailNotificationsOverride ?? !user.EmailOptOut;
            var smsEnabled = user.Settings.SmsNotificationsOverride ?? !user.SmsOptOut;

            // Send email
            if (emailEnabled && !string.IsNullOrEmpty(user.Email))
            {
                emailSent = await SendEmailAsync(user.Email, subject, htmlMessage);
            }

            // Send SMS
            if (smsEnabled && !string.IsNullOrEmpty(user.PhoneNumber))
            {
                smsSent = await _smsService.SendSmsAsync(user.PhoneNumber, smsMessage, ct);
            }

            return emailSent || smsSent;
        }

        /// <summary>
        /// Sends an email using Brevo API.
        /// </summary>
        private async Task<bool> SendEmailAsync(string toEmail, string subject, string htmlContent)
        {
            // Dev mode - just log
            if (!_emailConfigured || _emailApi == null)
            {
                _logger.LogInformation("[DEV MODE] Email would be sent to {To}: Subject: {Subject}", toEmail, subject);
                return true;
            }

            try
            {
                var senderName = _configuration["BrevoApi:SenderName"] ?? "LaundrySystem";
                var senderEmail = _configuration["BrevoApi:SenderEmail"] ?? "noreply@laundrysystem.local";

                var sendSmtpEmail = new SendSmtpEmail
                {
                    To = new List<SendSmtpEmailTo> { new SendSmtpEmailTo(toEmail) },
                    Subject = subject,
                    HtmlContent = htmlContent,
                    Sender = new SendSmtpEmailSender(senderName, senderEmail)
                };

                var response = await _emailApi.SendTransacEmailAsync(sendSmtpEmail);
                _logger.LogInformation("Email sent successfully to {To}: {MessageId}", toEmail, response.MessageId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {To}", toEmail);
                return false;
            }
        }
    }
}
