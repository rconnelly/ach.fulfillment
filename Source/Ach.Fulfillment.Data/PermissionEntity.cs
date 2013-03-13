namespace Ach.Fulfillment.Data
{
    public class PermissionEntity : BaseEntity
    {
        public virtual RoleEntity Role { get; set; }

        public virtual AccessRight Name { get; set; }
    }
}