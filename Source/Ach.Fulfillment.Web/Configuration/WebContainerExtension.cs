namespace Ach.Fulfillment.Web.Configuration
{
    using System;
    using System.Collections.Specialized;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.Caching;
    using System.Web.Mvc;
    using System.Web.Mvc.Async;

    using Ach.Fulfillment.Business.Exceptions;
    using Ach.Fulfillment.Common.Exceptions;
    using Ach.Fulfillment.Common.Unity;
    using Ach.Fulfillment.Data;
    using Ach.Fulfillment.Initialization.Configuration;
    using Ach.Fulfillment.Web.Areas.Api.Models;
    using Ach.Fulfillment.Web.Common.Controllers;

    using AutoMapper;

    using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
    using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.ContainerModel.Unity;
    using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
    using Microsoft.Practices.ServiceLocation;
    using Microsoft.Practices.Unity;

    public class WebContainerExtension : InitializationContainerExtension
    {
        #region Fields

        public const string DefaultPolicy = "Communication.Web";

        public const string WebValidationPolicy = "Communication.Web.Validation";

        public const string ApiValidationPolicy = "Communication.Api.Validation";

        #endregion

        #region Methods

        protected override void Initialize()
        {
            base.Initialize();

            this.Container.RegisterType<UnitOfWorkLifetimeStore, UnitOfWorkLifetimeHttpContextStore>(new ContainerControlledLifetimeManager());
            this.Container.RegisterType<IControllerFactory, CustomControllerFactory>(new ContainerControlledLifetimeManager());
            this.Container.RegisterType<ITempDataProvider, SessionStateTempDataProvider>(new ContainerControlledLifetimeManager());
            this.Container.RegisterType<IAsyncActionInvoker, AsyncControllerActionInvoker>(new ContainerControlledLifetimeManager());
            this.Container.RegisterType<IViewPageActivator>(new ContainerControlledLifetimeManager(), new InjectionFactory(c => null));
            this.Container.RegisterType<ObjectCache>(new ContainerControlledLifetimeManager(), new InjectionFactory(c => MemoryCache.Default));
            this.Container.RegisterType<ModelMetadataProvider, CachedDataAnnotationsModelMetadataProvider>(new ContainerControlledLifetimeManager());

            this.ConfigureExceptionHandling();

            DependencyResolver.SetResolver(ServiceLocator.Current);

            this.ConfigureMappings();
        }

        private void ConfigureMappings()
        {
            Mapper.CreateMap<AchTransactionModel, AchTransactionEntity>();
        }

        private void ConfigureExceptionHandling()
        {
            var configurationSource = new DictionaryConfigurationSource();
            var builder = new ConfigurationSourceBuilder();
            builder
                .ConfigureExceptionHandling()
                .GivenPolicyWithName(WebValidationPolicy)
                    .ForExceptionType<DeleteConstraintException>()
                        .HandleCustom<BusinessValidationHandler>()
                        .ThenThrowNewException()
                    .ForExceptionType<BusinessValidationException>()
                        .ThenNotifyRethrow()
                    .ForExceptionType<BusinessException>()
                        .HandleCustom<BusinessValidationHandler>()
                        .ThenThrowNewException()
                .GivenPolicyWithName(ApiValidationPolicy)
                    .ForExceptionType<DeleteConstraintException>()
                        .HandleCustom<BusinessValidationHandler>()
                        .ThenThrowNewException()
                    .ForExceptionType<BusinessValidationException>()
                        .ThenNotifyRethrow()
                    .ForExceptionType<BusinessException>()
                        .HandleCustom<BusinessValidationHandler>()
                        .ThenThrowNewException()
                    .ForExceptionType<Exception>()
                        .LogToCategory("General")
                            .WithSeverity(TraceEventType.Critical)
                            .UsingExceptionFormatter<TextExceptionFormatter>()
                        .HandleCustom(
                            typeof(WrapHandler<Exception>),
                            new NameValueCollection
                                {
                                    { BusinessValidationHandler.MessageKey, "An error has occurred while consuming this service. Please contact your administrator for more information." },
                                    { BusinessValidationHandler.AppendHandlingIdKey, bool.TrueString }
                                })
                        .ThenThrowNewException()
                .GivenPolicyWithName(DefaultPolicy)
                    .ForExceptionType<Exception>()
                        .LogToCategory("General")
                            .WithSeverity(TraceEventType.Critical)
                            .UsingExceptionFormatter<TextExceptionFormatter>()
                        .WrapWith<Exception>()
                            .UsingMessage("An error has occurred while processing request. Please contact your administrator for more information. [Error ID: {handlingInstanceID}]")
                        .ThenThrowNewException();
            builder.UpdateConfigurationWithReplace(configurationSource);

            var configurator = new UnityContainerConfigurator(this.Container);
            EnterpriseLibraryContainer.ConfigureContainer(configurator, configurationSource);
        }

        #endregion

        #region Nested types
        // ReSharper disable UnusedParameter.Local
        internal class WrapHandler<T> : IExceptionHandler
            where T : Exception
        {
            protected internal const string MessageKey = "Message";

            protected internal const string AppendHandlingIdKey = "AppendHandlingId";

            private readonly string message;

            private readonly bool appendHandlingId;

            public WrapHandler(NameValueCollection collection)
            {
                Contract.Assert(collection != null);

                if (collection.AllKeys.Contains(MessageKey))
                {
                    this.message = collection[MessageKey];
                }

                if (collection.AllKeys.Contains(AppendHandlingIdKey))
                {
                    this.appendHandlingId = bool.Parse(collection[AppendHandlingIdKey]);
                }
            }

            public Exception HandleException(Exception exception, Guid handlingInstanceId)
            {
                var error = this.FormatErrorMessage(exception, handlingInstanceId);
                var result = (T)Activator.CreateInstance(typeof(T), error, exception);
                return result;
            }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "handlingInstanceID", Justification = "By design"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Microsoft.Practices.EnterpriseLibrary.ExceptionHandling.ExceptionUtility.FormatExceptionMessage(System.String,System.Guid)", Justification = "By design")]
            private string FormatErrorMessage(Exception exception, Guid handlingInstanceId)
            {
                var pattern = this.message ?? exception.Message;

                if (this.appendHandlingId)
                {
                    pattern = string.Format(
                        CultureInfo.InvariantCulture,
                        "{0}. [Error ID: {{handlingInstanceID}}]",
                        pattern.Trim(' ', '.'));
                }

                var error = ExceptionUtility.FormatExceptionMessage(pattern, handlingInstanceId);
                return error;
            }
        }

        internal class BusinessValidationHandler : WrapHandler<BusinessValidationException>
        {
            public BusinessValidationHandler(NameValueCollection collection) : base(collection)
            {
            }
        }

        // ReSharper restore UnusedParameter.Local
        #endregion
    }
}
