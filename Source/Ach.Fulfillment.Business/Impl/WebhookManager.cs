namespace Ach.Fulfillment.Business.Impl
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;

    using Ach.Fulfillment.Data;
    using Ach.Fulfillment.Data.Specifications;

    public class WebhookManager : ManagerBase<WebhookEntity>, IWebhookManager
    {
        public override WebhookEntity Create(WebhookEntity webhook)
        {
            Contract.Assert(webhook != null);

            webhook.Limit = 50; // todo make configurable

            var instance = base.Create(webhook);
        
            return instance;
        }

        public List<WebhookEntity> GetAllToSend()
        {
            var webhooks = this.Repository.FindAll(new WebhooksToSend()).ToList();

            return webhooks;
        }

        public void Send()
        {
            var webhooks = this.GetAllToSend();

            foreach (var webhookEntity in webhooks)
            {
                Send(webhookEntity);
            }
        }

        public void Send(WebhookEntity webhook)
        {
            var uri = new Uri(webhook.Url);
            
            var c = new HttpClient
                {
                    BaseAddress = uri
                };
            c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var request = new HttpRequestMessage(HttpMethod.Post, "relativeAddress")
                {
                    Content = new StringContent(webhook.Body, Encoding.UTF8)
                };

            webhook.LastSent = DateTime.Now;

            var response = c.SendAsync(request);
            if (response.Result.IsSuccessStatusCode) this.Delete(webhook);
            else
            {
                webhook.Limit--;

                if(webhook.Limit > 0)
                    this.Update(webhook);
                else 
                    this.Delete(webhook);
            }

        }

    }
}
