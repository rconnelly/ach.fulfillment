namespace Ach.Fulfillment.Data.Specifications
{
    using System;
    using System.Linq.Expressions;

    using Ach.Fulfillment.Data.Common;

    [Obsolete]
    public class UniverseByActiveLogin : SpecificationBase<UniverseEntity>
    {
        private readonly string login;

        public UniverseByActiveLogin(string login)
        {
            this.login = login;
        }

        public override Expression<Func<UniverseEntity, bool>> IsSatisfiedBy()
        {
            return m => !m.Deleted && m.Login == this.login;
        }
    }
}