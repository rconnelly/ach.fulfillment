namespace Ach.Fulfillment.Business
{
    using Ach.Fulfillment.Data;

    public interface IWebhookManager
    {
        WebhookEntity Create(WebhookEntity instance);

        void Send();
    }
}
