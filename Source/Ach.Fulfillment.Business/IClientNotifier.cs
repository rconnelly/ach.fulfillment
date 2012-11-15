namespace Ach.Fulfillment.Business
{
    public interface IClientNotifier
    {
        void NotificationRequest(string callbackUrl, string data);
    }
}
