namespace Ach.Fulfillment.Web.Configuration
{
    using Ach.Fulfillment.Initialization.Configuration;
    using Ach.Fulfillment.Web.Common.Cache;

    using Microsoft.Practices.Unity;

    public class WebContainerExtension : InitializationContainerExtension
    {
        #region Methods

        protected override void Initialize()
        {
            base.Initialize();

            this.Container.RegisterInstance<ICacheClient>(new MemoryCacheClient());
        }

        #endregion
    }
}
