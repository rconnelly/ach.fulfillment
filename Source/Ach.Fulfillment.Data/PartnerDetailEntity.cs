using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ach.Fulfillment.Data
{
    public class PartnerDetailEntity : BaseEntity
    {
        private int PartnerId { get; set; }
        private PartnerEntity Partner { get; set; }

        public virtual string ImmediateDestination { get; set; }
        public virtual string CompanyIdentification { get; set; }
        public virtual string Destination { get; set; }
        public virtual string OriginOrCompanyName { get; set; }
        public virtual string CompanyName { get; set; }
        public virtual string DiscretionaryData { get; set; }
        public virtual string DFIIdentification { get; set; }

        protected PartnerDetailEntity(){}

        public PartnerDetailEntity(PartnerEntity partnerEntity)
        {
            Partner = partnerEntity;
        }
    }
}
