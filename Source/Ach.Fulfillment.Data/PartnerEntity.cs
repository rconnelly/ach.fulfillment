namespace Ach.Fulfillment.Data
{
    using System.Collections.Generic;

    public class PartnerEntity : BaseEntity
    {
        public virtual ICollection<UserEntity> Users { get; set; }

        public virtual PartnerDetailEntity Details { get; set; }

        public virtual string Name { get; set; }

        public virtual bool Disabled { get; set; }
    }
}