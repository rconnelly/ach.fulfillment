namespace Ach.Fulfillment.Business.Impl
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    using Ach.Fulfillment.Business.Impl.Validation;
    using Ach.Fulfillment.Common.Exceptions;
    using Ach.Fulfillment.Common.Utils;
    using Ach.Fulfillment.Data;
    using Ach.Fulfillment.Data.Specifications;

    internal class RoleManager : ManagerBase<RoleEntity>, IRoleManager
    {
        public override RoleEntity Create(RoleEntity instance)
        {
            Contract.Assert(instance != null);
            this.DemandValid<RoleValidator>(instance);
            return base.Create(instance);
        }

        public IEnumerable<RoleEntity> FindAll()
        {
            var result = this.Repository.FindAll(new RoleAll());
            return result;
        }

        public RoleEntity Load(string name)
        {
            Contract.Assert(name != null);
            var instance = this.Repository.FindOne(new RoleByName(name));
            if (instance == null)
            {
                throw new ObjectNotFoundException("Unable to load role by name {0}".Fmt(name));
            }

            return instance;
        }

        /*public void Delete(RoleEntity instance)
        {
            Contract.Assert(instance != null);

            // TODO do we want to delete record?
            this.Repository.Delete(instance);
        }*/

        public void Update(RoleEntity currency)
        {
            Contract.Assert(currency != null);
            this.DemandValid<RoleValidator>(currency);
            this.Repository.Update(currency);
        }
    }
}