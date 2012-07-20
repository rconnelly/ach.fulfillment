namespace Ach.Fulfillment.Web.Configuration
{
    using System.Runtime.Caching;

    using Ach.Fulfillment.Initialization.Configuration;

    using Microsoft.Practices.Unity;

    public class WebContainerExtension : InitializationContainerExtension
    {
        #region Methods

        protected override void Initialize()
        {
            base.Initialize();

            this.Container.RegisterInstance<ObjectCache>(MemoryCache.Default);
        }

        #endregion
    }
}
