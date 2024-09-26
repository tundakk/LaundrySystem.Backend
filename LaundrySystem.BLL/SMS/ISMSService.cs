namespace LaundrySystem.BLL.SMS
{
    /// <summary>
    /// Interface for SMS service providing methods to send SMS messages.
    /// </summary>
    public interface ISMSService
    {
        /// <summary>
        /// Sends an SMS message to the specified recipient.
        /// </summary>
        /// <param name="to">The recipient's phone number.</param>
        /// <param name="message">The message content.</param>
        void SendSMS(string to, string message);
    }
}