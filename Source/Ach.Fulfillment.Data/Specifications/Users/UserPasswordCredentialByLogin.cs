namespace Ach.Fulfillment.Data.Specifications.Users
{
    using System;
    using System.Linq.Expressions;

    using Ach.Fulfillment.Data.Common;

    public class UserPasswordCredentialByLogin : SpecificationBase<UserPasswordCredentialEntity>
    {
        private readonly string login;

        public UserPasswordCredentialByLogin(string login)
        {
            this.login = login;
        }

        public override Expression<Func<UserPasswordCredentialEntity, bool>> IsSatisfiedBy()
        {
            return m => m.Login == this.login;
        }
    }
}