namespace Ach.Fulfillment.Scheduler.Configuration
{
    using System;
    using System.Diagnostics;

    using Ach.Fulfillment.Initialization.Configuration;
    using Ach.Fulfillment.Scheduler.Common;

    using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
    using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.ContainerModel.Unity;
    using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
    using Microsoft.Practices.Unity;

    using Quartz;
    using Quartz.Spi;

    internal class SchedulerContainerExtension : InitializationContainerExtension
    {
        #region Fields

        public const string JobPolicy = "Presentation.Scheduler";

        #endregion

        #region Methods

        protected override void Initialize()
        {
            this.Container.AddNewExtension<InitializationContainerExtension>();
            Container.RegisterType<ISchedulerFactory, SchedulerFactory>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IJobFactory, JobFactory>(new ContainerControlledLifetimeManager());  
           
            this.ConfigureExceptionHandling();
        }

        private void ConfigureExceptionHandling()
        {
            var configurationSource = new DictionaryConfigurationSource();

            var builder = new ConfigurationSourceBuilder();
            builder.ConfigureExceptionHandling()
                   .GivenPolicyWithName(JobPolicy)
                   .ForExceptionType<JobExecutionException>()
                        .LogToCategory("General")
                            .WithSeverity(TraceEventType.Error)
                            .UsingExceptionFormatter<TextExceptionFormatter>()
                        .ThenNotifyRethrow()
                   .ForExceptionType<Exception>()
                        .LogToCategory("General")
                            .WithSeverity(TraceEventType.Critical)
                            .UsingExceptionFormatter<TextExceptionFormatter>()
                        .WrapWith<JobExecutionException>()
                            .UsingMessage("Unexpected error occured.")
                        .ThenThrowNewException();
            builder.UpdateConfigurationWithReplace(configurationSource);

            var configurator = new UnityContainerConfigurator(this.Container);
            EnterpriseLibraryContainer.ConfigureContainer(configurator, configurationSource);
        }
        #endregion
    }
}