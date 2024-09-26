namespace LaundrySystem.BLL.SMS
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Options;
    using Twilio;
    using Twilio.Rest.Api.V2010.Account;

    /// <summary>
    /// Service for sending SMS messages using Twilio.
    /// </summary>
    public class SMSService : ISMSService
    {
        private readonly TwilioSettings _settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="SMSService"/> class.
        /// </summary>
        /// <param name="settings">The Twilio settings.</param>
        /// <param name="configuration">The configuration instance.</param>
        public SMSService(IOptions<TwilioSettings> settings, IConfiguration configuration)
        {
            _settings = settings.Value;
            TwilioClient.Init(_settings.AccountSid, _settings.AuthToken);
        }

        /// <summary>
        /// Sends an SMS message to the specified phone number.
        /// </summary>
        /// <param name="to">The recipient's phone number.</param>
        /// <param name="message">The message to send.</param>
        public void SendSMS(string to, string message)
        {
            MessageResource.Create(
                body: message,
                from: new Twilio.Types.PhoneNumber(_settings.PhoneNumber),
                to: new Twilio.Types.PhoneNumber(to)
            );
        }
    }
}