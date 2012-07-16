namespace Ach.Fulfillment.Data
{
    using System.Collections.Generic;

    public class UserPasswordCredentialEntity : BaseEntity
    {
        public string Login { get; set; }

        public string PasswordHash { get; set; }

        public string PasswordSalt { get; set; }

        public UserEntity User { get; set; }
    }

    public class UserEntity : BaseEntity
    {
        public string Name { get; set; }

        public RoleEntity Role { get; set; }

        public ICollection<UserPasswordCredentialEntity> UserPasswordCredentials { get; set; }

        public ICollection<PartnerEntity> Partners { get; set; }
    }
    
    public class RoleEntity : BaseEntity
    {
        public string Name { get; set; }

        public ICollection<PermissionEntity> Permissions { get; set; } 
    }

    public class PermissionEntity : BaseEntity
    {
        public Privilege Name { get; set; }
    }

    public enum Privilege
    {
        Authenticate
    }

    public class PartnerEntity : BaseEntity
    {
        public string Name { get; set; }

        public ICollection<UserEntity> Users { get; set; } 
    }
}