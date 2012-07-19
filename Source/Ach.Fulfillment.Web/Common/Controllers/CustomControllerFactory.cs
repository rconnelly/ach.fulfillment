namespace Ach.Fulfillment.Web.Common.Controllers
{
    using System;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;

    using Ach.Fulfillment.Web.Areas.Common.Controllers;

    using global::Common.Logging;

    public class CustomControllerFactory : DefaultControllerFactory
    {
        #region Constants and Fields

        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        #endregion

        #region Methods

        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            IController controller;
            try
            {
                controller = base.GetControllerInstance(requestContext, controllerType);
            }
            catch (HttpException exc)
            {
                if (exc.GetHttpCode() == 404)
                {
                    Log.Error(exc.Message, exc);

                    controller = base.GetControllerInstance(requestContext, typeof(ErrorController));

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

        #endregion
    }
}