namespace Ach.Fulfillment.Data
{
    public class PartnerDetailEntity : BaseEntity
    {
        public virtual PartnerEntity Partner { get; set; }

        public virtual string ImmediateDestination { get; set; }

        public virtual string CompanyIdentification { get; set; }

        public virtual string Destination { get; set; }

        public virtual string OriginOrCompanyName { get; set; }

        public virtual string CompanyName { get; set; }

        // todo: remove unused field
        public virtual string DiscretionaryData { get; set; }

        public virtual string DfiIdentification { get; set; }
    }
}
