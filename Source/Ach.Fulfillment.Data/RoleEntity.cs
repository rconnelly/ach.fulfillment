namespace Ach.Fulfillment.Data
{
    using System.Collections.Generic;

    public class RoleEntity : BaseEntity
    {
        public virtual ICollection<PermissionEntity> Permissions { get; set; } 

        public virtual string Name { get; set; }
    }
}