namespace Ach.Fulfillment.Data
{
    using System.Collections.Generic;

    public class UserEntity : BaseEntity
    {
        public virtual string Name { get; set; }

        public virtual RoleEntity Role { get; set; }

        public virtual ICollection<UserPasswordCredentialEntity> UserPasswordCredentials { get; set; }

        public virtual ICollection<PartnerEntity> Partners { get; set; }
    }
}