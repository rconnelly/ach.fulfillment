namespace Ach.Fulfillment.Data
{
    public class UserEntry : BaseEntity
    {
        public virtual string Email { get; set; }

        public virtual string Name { get; set; }

        public virtual UserPawsswordCredentialEntry Credential { get; set; }

        public virtual RoleEntry Role { get; set; }

        public virtual PartnerEntry Partner { get; set; }
    }
}