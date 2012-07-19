namespace Ach.Fulfillment.Api.Configuration
{
    using System;
    using System.Collections.Specialized;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Net;

    using Ach.Fulfillment.Business.Exceptions;
    using Ach.Fulfillment.Common.Exceptions;
    using Ach.Fulfillment.Initialization.Configuration;

    using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
    using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.ContainerModel.Unity;
    using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
    using Microsoft.Practices.Unity;

    using ServiceStack.CacheAccess;
    using ServiceStack.CacheAccess.Providers;
    using ServiceStack.Common.Web;
    using ServiceStack.FluentValidation.Results;

    public class ApiContainerExtension : InitializationContainerExtension
    {
        #region Methods

        protected override void Initialize()
        {
            base.Initialize();
            this.ConfigureExceptionHandling();

            this.Container.RegisterInstance<ICacheClient>(new MemoryCacheClient());
            new ApiAppHost(this.Container).Init();
        }

        private void ConfigureExceptionHandling()
        {
            var configurationSource = new DictionaryConfigurationSource();

            var builder = new ConfigurationSourceBuilder();
            builder.ConfigureExceptionHandling()
                   .GivenPolicyWithName("Communication.Api")
                   .ForExceptionType<Exception>()
                        .ThenNotifyRethrow()
                   .ForExceptionType<BusinessValidationException>()
                        .HandleCustom<BusinessValidationExceptionHandler>()
                        .ThenThrowNewException()
                   .ForExceptionType<DeleteConstraintException>()
                        .HandleCustom<HttpErrorExceptionHandler>()
                        .ThenThrowNewException();
            builder.UpdateConfigurationWithReplace(configurationSource);

            var configurator = new UnityContainerConfigurator(this.Container);
            EnterpriseLibraryContainer.ConfigureContainer(configurator, configurationSource);
        }

        #endregion

        #region Nested types

        // ReSharper disable ClassNeverInstantiated.Local
        // ReSharper disable UnusedParameter.Local
        private class BusinessValidationExceptionHandler : IExceptionHandler
        {
            public BusinessValidationExceptionHandler(NameValueCollection collection)
            {
            }

            public Exception HandleException(Exception exception, Guid handlingInstanceId)
            {
                var e = exception as BusinessValidationException;
                if (e != null)
                {
                    var list = from error in e.Errors ?? new ValidationFailureInfo[0]
                               select new ValidationFailure(
                                   error.PropertyName,
                                   error.ErrorMessage,
                                   error.ErrorCode,
                                   error.AttemptedValue);
                    exception = new ServiceStack.FluentValidation.ValidationException(list);
                }

                return exception;
            }
        }

        private class HttpErrorExceptionHandler : IExceptionHandler
        {
            private const string StatusCodeKey = "StatusCode";

            private readonly HttpStatusCode statusCode = HttpStatusCode.Conflict;
            
            public HttpErrorExceptionHandler(NameValueCollection collection)
            {
                Contract.Assert(collection != null);
                if (collection.AllKeys.Contains(StatusCodeKey))
                {
                    this.statusCode = (HttpStatusCode)Enum.Parse(typeof(HttpStatusCode), collection[StatusCodeKey]);
                }
            }

            public Exception HandleException(Exception exception, Guid handlingInstanceId)
            {
                var result = new HttpError(this.statusCode, exception);
                return result;
            }
        }

        // ReSharper restore UnusedParameter.Local
        // ReSharper restore ClassNeverInstantiated.Local
        #endregion
    }
}
