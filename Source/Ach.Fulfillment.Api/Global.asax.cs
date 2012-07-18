namespace Ach.Fulfillment.Api
{
    using System;
    using System.Web;

    using Ach.Fulfillment.Api.Configuration;
    using Ach.Fulfillment.Common;

    public class Global : HttpApplication
    {
        #region Methods

        protected void Application_Start(object sender, EventArgs e)
        {
            Shell.Start<ApiContainerExtension>();
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
        }

        protected void Session_Start(object sender, EventArgs e)
        {
        }

        protected void Session_End(object sender, EventArgs e)
        {
        }

        protected void Application_End(object sender, EventArgs e)
        {
            Shell.Shutdown();
        }

        protected void Application_Error(object sender, EventArgs e)
        {
        }

        #endregion
    }
}