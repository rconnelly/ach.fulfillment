namespace Ach.Fulfillment.Data
{
    public class UserPasswordCredentialEntity : BaseEntity
    {
        public virtual UserEntity User { get; set; }

        public virtual string Login { get; set; }

        public virtual string PasswordHash { get; set; }

        public virtual string PasswordSalt { get; set; }
    }
}