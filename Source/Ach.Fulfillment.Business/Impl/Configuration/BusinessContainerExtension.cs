namespace Ach.Fulfillment.Business.Impl.Configuration
{
    using System;

    using Ach.Fulfillment.Business.Exceptions;
    using Ach.Fulfillment.Common.Security;
    using Ach.Fulfillment.Common.Unity;

    using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
    using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.ContainerModel.Unity;
    using Microsoft.Practices.Unity;

    using Unity.AutoRegistration;

    public class BusinessContainerExtension : UnityContainerExtension
    {
        #region Fields

        internal const string OperationNachaGenerationPolicy = "Business.Operation.NachaGeneration";

        internal const string OperationUploadPolicy = "Business.Operation.Upload";

        internal const string OperationStatusCheckPolicy = "Business.Operation.StatusCheck";

        internal const string OperationCallbackNotificationPolicy = "Business.Operation.CallbackNotification";

        #endregion

        #region Methods

        protected override void Initialize()
        {
            this.WellKnownITypeNameAutoRegistration<ContainerControlledLifetimeManager>(WellKnownAppParts.Manager);
            this.Container.RegisterType<IApplicationPrincipal, ThreadApplicationPrincipal>(new ContainerControlledLifetimeManager());

            this.ConfigureCryptography();
            this.ConfigureExceptionHandling();
        }

        private void ConfigureCryptography()
        {
            var configurationSource = new DictionaryConfigurationSource();
            var builder = new ConfigurationSourceBuilder();
            builder.ConfigureCryptography()
                   .EncryptUsingHashAlgorithmProviderNamed(UserManager.HashInstance)
                   .WithOptions.UsingHashAlgorithm<Zetetic.Security.Pbkdf2Hash>()
                /*.SetAsDefault()*/; // do not want Pbkdf2Hash (low speed algorithm) to be default
            builder.UpdateConfigurationWithReplace(configurationSource);

            var configurator = new UnityContainerConfigurator(this.Container);
            EnterpriseLibraryContainer.ConfigureContainer(configurator, configurationSource);
        }

        private void ConfigureExceptionHandling()
        {
            var configurationSource = new DictionaryConfigurationSource();

            var builder = new ConfigurationSourceBuilder();
            builder.ConfigureExceptionHandling()
                .GivenPolicyWithName(OperationNachaGenerationPolicy)
                    .ForExceptionType<BusinessException>()
                        .ThenNotifyRethrow()
                    .ForExceptionType<Exception>()
                        .WrapWith<BusinessException>()
                            .UsingMessage("Unable to generate nacha file.")
                        .ThenThrowNewException()
                .GivenPolicyWithName(OperationUploadPolicy)
                    .ForExceptionType<BusinessException>()
                        .ThenNotifyRethrow()
                    .ForExceptionType<Exception>()
                        .WrapWith<BusinessException>()
                            .UsingMessage("Unable to upload nacha file.")
                        .ThenThrowNewException()
                .GivenPolicyWithName(OperationStatusCheckPolicy)
                    .ForExceptionType<BusinessException>()
                        .ThenNotifyRethrow()
                    .ForExceptionType<Exception>()
                        .WrapWith<BusinessException>()
                            .UsingMessage("Unable to check uploaded file status.")
                        .ThenThrowNewException()
                .GivenPolicyWithName(OperationCallbackNotificationPolicy)
                    .ForExceptionType<BusinessException>()
                        .ThenNotifyRethrow()
                    .ForExceptionType<Exception>()
                        .WrapWith<BusinessException>()
                            .UsingMessage("Unable to notify.")
                        .ThenThrowNewException();
            builder.UpdateConfigurationWithReplace(configurationSource);

            var configurator = new UnityContainerConfigurator(this.Container);
            EnterpriseLibraryContainer.ConfigureContainer(configurator, configurationSource);
        }

        #endregion
    }
}