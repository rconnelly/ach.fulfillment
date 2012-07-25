namespace Ach.Fulfillment.Business.Impl
{
    using System.Collections.Generic;
    using System.Linq;

    using Ach.Fulfillment.Business.Impl.Validation;
    using Ach.Fulfillment.Data;
    using Ach.Fulfillment.Data.Common;
    using Ach.Fulfillment.Persistence;

    using FluentValidation;

    using Microsoft.Practices.ServiceLocation;
    using Microsoft.Practices.Unity;

    internal class ManagerBase<T> : IManager<T>
        where T : class, IEntity
    {
        #region Public Properties

        [Dependency]
        public IServiceLocator Locator { get; set; }

        [Dependency]
        public IRepository Repository { get; set; }

        #endregion

        #region Methods

        public virtual T Create(T instance)
        {
            this.Repository.Create(instance);
            return instance;
        }

        public virtual void Update(T instance)
        {
            this.Repository.Update(instance);
        }

        public virtual T Load(long id)
        {
            var instance = this.Repository.Load<T>(id);
            return instance;
        }

        public virtual IEnumerable<T> FindAll(IQueryData<T> queryData)
        {
            var result = this.Repository.FindAll<T>(queryData);
            return result;
        }

        public virtual IQueryable<T> FindQuery(IQueryData<T> queryData)
        {
            var result = this.Repository.FindQuery<T>(queryData);
            return result;
        }

        public virtual T FindOne(IQueryData<T> queryData)
        {
            var instance = this.Repository.FindOne<T>(queryData);
            return instance;
        }

        public virtual void Delete(T instance)
        {
            this.Repository.Delete(instance);
        }

        protected void DemandValid<TValidator, TInstance>(TInstance instance)
            where TValidator : IValidator<TInstance>
        {
            var validator = this.Locator.GetInstance<TValidator>();
            validator.DemandValid(instance);
        }

        protected void DemandValid<TValidator>(object instance)
            where TValidator : class, IValidator
        {
            var validator = this.Locator.GetInstance<TValidator>();
            validator.DemandValid(instance);
        }

        #endregion
    }
}