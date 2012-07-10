namespace Ach.Fulfillment.Initialization.Configuration
{
    using Microsoft.Practices.Unity;

    public class InitializationContainerExtension : UnityContainerExtension
    {
        #region Methods

        protected override void Initialize()
        {
            /*this.Container
                .AddNewExtension<CrosscuttingContainerExtension>()
                .AddNewExtension<PersistenceContainerExtension>()
                .AddNewExtension<BusinessContainerExtension>()
                .AddNewExtension<CommunicationContainerExtension>();*/
        }

        #endregion
    }
}
