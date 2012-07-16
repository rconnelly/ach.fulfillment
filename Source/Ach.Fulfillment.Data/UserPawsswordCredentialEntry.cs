namespace Ach.Fulfillment.Data
{
    public class UserPawsswordCredentialEntry : BaseEntity
    {
        public virtual string Login { get; set; }

        public virtual string PasswordHash { get; set; }

        public virtual string PasswordSalt { get; set; }
    }
}