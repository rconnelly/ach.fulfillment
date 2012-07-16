namespace Ach.Fulfillment.Data
{
    public class PermissionEntity : BaseEntity
    {
        public virtual AccessRight Name { get; set; }

        public virtual RoleEntity Role { get; set; }
    }
}