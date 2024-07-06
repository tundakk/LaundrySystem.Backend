namespace LaundrySystem.BLL.SMS
{
    public interface ISMSService
    {
        void SendSMS(string to, string message);
    }
}