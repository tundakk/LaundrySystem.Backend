/// <summary>
/// Represents the Twilio settings found in the appsettings.json file.
/// </summary>
public class TwilioSettings
{
    /// <summary>
    /// Gets or sets the Account SID for Twilio.
    /// </summary>
    public required string AccountSid { get; set; }

    /// <summary>
    /// Gets or sets the Auth Token for Twilio.
    /// </summary>
    public required string AuthToken { get; set; }

    /// <summary>
    /// Gets or sets the Phone Number for Twilio.
    /// </summary>
    public required string PhoneNumber { get; set; }
}