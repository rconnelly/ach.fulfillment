namespace Ach.Fulfillment.Initialization.Configuration
{
    using Ach.Fulfillment.Business.Impl.Configuration;
    using Ach.Fulfillment.Persistence.Impl.Configuration;

    using Microsoft.Practices.Unity;

    public class InitializationContainerExtension : UnityContainerExtension
    {
        #region Methods

        protected override void Initialize()
        {
            this.Container
                .AddNewExtension<PersistenceContainerExtension>()
                .AddNewExtension<BusinessContainerExtension>();
        }

        #endregion
    }
}
