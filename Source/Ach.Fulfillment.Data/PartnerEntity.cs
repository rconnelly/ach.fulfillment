namespace Ach.Fulfillment.Data
{
    using System.Collections.Generic;

    public class PartnerEntity : BaseEntity
    {
        public string Name { get; set; }

        public ICollection<UserEntity> Users { get; set; } 
    }
}