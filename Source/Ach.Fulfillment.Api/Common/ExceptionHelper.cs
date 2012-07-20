namespace Ach.Fulfillment.Api.Common
{
    using System;

    using Ach.Fulfillment.Api.Configuration;

    using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;

    internal static class ExceptionHelper
    {
        public static Exception TransformException(this Exception ex)
        {
            var exceptionToProcess = ex;
            if (ex != null)
            {
                Exception exceptionToThrow;
                var rethrow = ExceptionPolicy.HandleException(ex, ApiContainerExtension.DefaultPolicy, out exceptionToThrow);
                if (rethrow && exceptionToThrow != null)
                {
                    exceptionToProcess = exceptionToThrow;
                }
            }

            return exceptionToProcess;
        }
    }
}