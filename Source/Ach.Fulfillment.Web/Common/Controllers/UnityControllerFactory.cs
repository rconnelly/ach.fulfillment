namespace Ach.Fulfillment.Web.Common.Controllers
{
    using System;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;

    using Ach.Fulfillment.Common.Utils;
    using Ach.Fulfillment.Web.Areas.Common.Controllers;

    using Microsoft.Practices.ServiceLocation;
    using Microsoft.Practices.Unity;

    using global::Common.Logging;

    public class UnityControllerFactory : DefaultControllerFactory
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            var container = ServiceLocator.Current.GetInstance<IUnityContainer>();

            IController controller;

            try
            {
                controller = base.GetControllerInstance(requestContext, controllerType);

                container.BuildUp(controllerType, controller);
            }
            catch (HttpException exc)
            {
                if (exc.GetHttpCode() == 404)
                {
                    Log.Error(exc.Message, exc);

                    controller = container.Resolve<ErrorController>();

                    requestContext.RouteData.Values.Clear();
                    requestContext.RouteData.Values.Add("controller", "Error");
                    requestContext.RouteData.Values.Add("action", "Error404");
                }
                else
                {
                    throw;
                }
            }

            return controller;
        }
    }
}