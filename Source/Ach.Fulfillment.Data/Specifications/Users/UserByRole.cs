namespace Ach.Fulfillment.Data.Specifications.Users
{
    using System;
    using System.Linq.Expressions;

    using Ach.Fulfillment.Data.Common;

    public class UserByRole : SpecificationBase<UserEntity>
    {
        private readonly string role;

        public UserByRole(string role)
        {
            this.role = role;
        }

        public override Expression<Func<UserEntity, bool>> IsSatisfiedBy()
        {
            return m => m.Role.Name == this.role;
        }
    }
}
