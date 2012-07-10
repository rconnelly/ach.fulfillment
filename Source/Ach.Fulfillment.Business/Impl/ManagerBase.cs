namespace Ach.Fulfillment.Business.Impl
{
    using Ach.Fulfillment.Business.Impl.Validation;
    using Ach.Fulfillment.Persistence;

    using FluentValidation;

    using Microsoft.Practices.ServiceLocation;
    using Microsoft.Practices.Unity;

    internal class ManagerBase
    {
        #region Public Properties

        [Dependency]
        public IServiceLocator Locator { get; set; }

        [Dependency]
        public IRepository Repository { get; set; }

        #endregion

        #region Methods

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