namespace Ach.Fulfillment.Data
{
    using System;

    public class WebhookEntity : BaseEntity
    {       
        public virtual string Url { get; set; }

        public virtual int Limit { get; set; }

        public virtual string Header { get; set; }

        public virtual string Body { get; set; }

        public virtual DateTime? LastSent { get; set; }
    }
}
