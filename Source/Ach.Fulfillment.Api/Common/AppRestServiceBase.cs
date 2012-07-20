namespace Ach.Fulfillment.Api.Common
{
    using System;

    using ServiceStack.ServiceInterface;

    internal abstract class AppRestServiceBase<T> : RestServiceBase<T>
    {
        #region Methods

        protected override object HandleException(T request, Exception ex)
        {
            var transformed = ex.TransformException();
            return base.HandleException(request, transformed);
        }

        #endregion
    }
}