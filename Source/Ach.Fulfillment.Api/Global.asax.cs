namespace Ach.Fulfillment.Api
{
    using System;
    using System.Web;

    using Ach.Fulfillment.Api.Configuration;
    using Ach.Fulfillment.Common;

    using global::Common.Logging;

    public class Global : HttpApplication
    {
        #region Fields

        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        #endregion

        #region Methods

        protected void Application_Start(object sender, EventArgs e)
        {
            Shell.Start<ApiContainerExtension>();
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var exception = Server.GetLastError();
            Log.Error("Unexpected error.", exception);
        }

        protected void Application_End(object sender, EventArgs e)
        {
            Shell.Shutdown();
        }

        #endregion
    }
}