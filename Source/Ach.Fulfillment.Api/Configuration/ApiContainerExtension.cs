namespace Ach.Fulfillment.Api.Configuration
{
    using Ach.Fulfillment.Initialization.Configuration;

    using Microsoft.Practices.Unity;

    using ServiceStack.CacheAccess;
    using ServiceStack.CacheAccess.Providers;

    public class ApiContainerExtension : InitializationContainerExtension
    {
        #region Methods

        protected override void Initialize()
        {
            base.Initialize();
            this.Container.RegisterInstance<ICacheClient>(new MemoryCacheClient());

            new ApiAppHost(this.Container).Init();
        }

        #endregion
    }
}
