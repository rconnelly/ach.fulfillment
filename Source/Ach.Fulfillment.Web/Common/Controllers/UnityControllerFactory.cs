namespace Ach.Fulfillment.Web.Common.Controllers
{
    using System;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;

    using Ach.Fulfillment.Web.Areas.Main.Controllers;

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
                var httpCode = exc.GetHttpCode();
                if (httpCode == 404)
                {
                    Log.Error(exc.Message, exc);

                    controller = container.Resolve<ErrorController>();

                    requestContext.RouteData.Values.Clear();

                    RouteHelper.InitErrorRoute(httpCode, requestContext.RouteData);

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