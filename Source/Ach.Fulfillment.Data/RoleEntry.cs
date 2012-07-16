namespace Ach.Fulfillment.Data
{
    using System.Collections.Generic;

    public class RoleEntry : BaseEntity
    {
        public virtual string Name { get; set; }

        public virtual IList<PermissionEntry> Permissions { get; set; }
    }
}