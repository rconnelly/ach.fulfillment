using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace Ach.Fulfillment.Api
{
    using Funq;

    using ServiceStack.WebHost.Endpoints;

    public class Global : System.Web.HttpApplication
    {
        public class FulfillmentApiHost : AppHostBase
        {
            //Tell Service Stack the name of your application and where to find your web services
            public FulfillmentApiHost()
                : base("Fulfillment Api Host", typeof(FulfillmentApi).Assembly)
            {
            }

            public override void Configure(Container container)
            {
                Routes
                  .Add<Test>("/test")
                  .Add<Test>("/test/{Token}");
            }
        }

        protected void Application_Start(object sender, EventArgs e)
        {
            new FulfillmentApiHost().Init();
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}