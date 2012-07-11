namespace Ach.Fulfillment.Business.Impl.Configuration
{
    using System.Security.Cryptography;

    using Ach.Fulfillment.Common.Unity;

    using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
    using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.ContainerModel.Unity;
    using Microsoft.Practices.Unity;

    using Unity.AutoRegistration;

    public class BusinessContainerExtension : UnityContainerExtension
    {
        protected override void Initialize()
        {
            this.WellKnownITypeNameAutoRegistration<ContainerControlledLifetimeManager>(WellKnownAppParts.Manager);

            var configurationSource = this.Container.Resolve<IConfigurationSource>();

            var builder = new ConfigurationSourceBuilder();
            builder.ConfigureCryptography()
                .EncryptUsingHashAlgorithmProviderNamed("PasswordHashing")
                .WithOptions
                    .UsingHashAlgorithm<SHA384Managed>()
                    .SetAsDefault();
            builder.UpdateConfigurationWithReplace(configurationSource);
            var configurator = new UnityContainerConfigurator(this.Container);
            EnterpriseLibraryContainer.ConfigureContainer(configurator, configurationSource);
        }
    }
}