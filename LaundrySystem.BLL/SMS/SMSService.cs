namespace LaundrySystem.BLL.SMS
{
    using Twilio;
    using Twilio.Rest.Api.V2010.Account; // is this the right using statement?

    public class SMSService : ISMSService
    {
        private readonly string _accountSid;
        private readonly string _authToken;
        private readonly string _fromPhoneNumber;

        public SMSService(string accountSid, string authToken, string fromPhoneNumber)
        {
            _accountSid = accountSid;
            _authToken = authToken;
            _fromPhoneNumber = fromPhoneNumber;
        }

        // https://console.twilio.com/
        public void SendSMS(string to, string message)
        {
            TwilioClient.Init(_accountSid, _authToken);

            MessageResource.Create(
                body: message,
                from: new Twilio.Types.PhoneNumber(_fromPhoneNumber),
                to: new Twilio.Types.PhoneNumber(to)
            );
        }
    }
}