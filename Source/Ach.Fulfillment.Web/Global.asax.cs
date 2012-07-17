namespace Ach.Fulfillment.Web
{
    using System;
    using System.Web.Mvc;
    using System.Web.Optimization;
    using System.Web.Routing;

    using Ach.Fulfillment.Common;
    using Ach.Fulfillment.Initialization.Configuration;

    using global::Common.Logging;

    public class MvcApplication : System.Web.HttpApplication
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private UnitOfWork unitOfWork;

        protected void Application_Start()
        {
            Log.Debug("Starting application");

            AreaRegistration.RegisterAllAreas();

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            Log.Debug("Starting shell");
            Shell.Start<InitializationContainerExtension>();
        }

        protected void Application_End()
        {
            Log.Debug("Ending application");

            Log.Debug("Shutdowning shell");
            Shell.Shutdown();
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            if (this.unitOfWork == null)
            {
                this.unitOfWork = new UnitOfWork();
            }
        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            if (this.unitOfWork != null)
            {
                this.unitOfWork.Dispose();
                this.unitOfWork = null;
            }
        }
    }
}