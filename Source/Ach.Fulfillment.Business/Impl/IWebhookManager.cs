namespace Ach.Fulfillment.Business
{
    using System.Collections.Generic;

    using Ach.Fulfillment.Data;

    public interface IWebhookManager : IManager<WebhookEntity>
    {
        List<WebhookEntity> GetAlltoSend();

    }
}
