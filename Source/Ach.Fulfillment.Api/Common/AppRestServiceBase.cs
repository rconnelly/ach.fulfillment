namespace Ach.Fulfillment.Api.Common
{
    using System;

    using Ach.Fulfillment.Api.Configuration;
    using Ach.Fulfillment.Common.Exceptions;

    using ServiceStack.ServiceInterface;

    internal abstract class AppRestServiceBase<T> : RestServiceBase<T>
    {
        #region Methods

        protected override object HandleException(T request, Exception ex)
        {
            var transformed = ex.TransformException(ApiContainerExtension.DefaultPolicy);
            return base.HandleException(request, transformed);
        }

        #endregion
    }
}