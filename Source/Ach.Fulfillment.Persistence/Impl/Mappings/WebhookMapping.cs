namespace Ach.Fulfillment.Persistence.Impl.Mappings
{
    using Ach.Fulfillment.Data;

    using FluentNHibernate.Automapping;
    using FluentNHibernate.Automapping.Alterations;

    public class WebhookMapping : IAutoMappingOverride<WebhookEntity>
    {
        public void Override(AutoMapping<WebhookEntity> mapping)
        {
            mapping.Table("Webhook");
            mapping.Id(x => x.Id, "WebhookId").GeneratedBy.Identity();
        }
    }
}
 
