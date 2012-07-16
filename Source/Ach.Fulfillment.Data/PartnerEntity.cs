namespace Ach.Fulfillment.Data
{
    using System.Collections.Generic;

    public class PartnerEntity : BaseEntity
    {
        public virtual string Name { get; set; }

        public virtual ICollection<UserEntity> Users { get; set; } 
    }
}