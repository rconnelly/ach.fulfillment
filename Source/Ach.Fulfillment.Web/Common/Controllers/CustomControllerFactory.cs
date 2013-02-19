namespace Ach.Fulfillment.Web.Common.Controllers
{
    using System;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;

    using Ach.Fulfillment.Web.Areas.Main.Controllers;

    using global::Common.Logging;

    using Microsoft.Practices.ServiceLocation;

    internal class CustomControllerFactory : DefaultControllerFactory
    {
        #region Constants and Fields

        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        #endregion

        #region Constructors

        public CustomControllerFactory()
            : base(new CustomControllerActivator())
        {
        }

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
                var httpCode = exc.GetHttpCode();
                if (httpCode == 404)
                {
                    Log.Error("Cannot instantiate mvc controller.", exc);

                    controller = base.GetControllerInstance(requestContext, typeof(ErrorController));

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

        #endregion

        private class CustomControllerActivator : IControllerActivator
        {
            #region Implementation of IControllerActivator

            public IController Create(RequestContext requestContext, Type controllerType)
            {
                var service = (IController)ServiceLocator.Current.GetInstance(controllerType);
                return service;
            }

            #endregion
        }
    }
}