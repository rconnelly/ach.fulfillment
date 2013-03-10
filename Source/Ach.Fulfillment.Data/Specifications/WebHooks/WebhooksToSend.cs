namespace Ach.Fulfillment.Data.Specifications.WebHooks
{
    using System;
    using System.Linq.Expressions;

    using Ach.Fulfillment.Data.Common;

    public class WebhooksToSend : SpecificationBase<WebhookEntity>
    {
        public override Expression<Func<WebhookEntity, bool>> IsSatisfiedBy()
        {
            return m => m.Limit > 0;
        }
    }
}
