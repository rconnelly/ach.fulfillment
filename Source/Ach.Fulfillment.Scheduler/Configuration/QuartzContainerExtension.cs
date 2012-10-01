using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Ach.Fulfillment.Common.Unity;
using Microsoft.Practices.Unity;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;

namespace Ach.Fulfillment.Scheduler.Configuration
{
    public class QuartzContainerExtension : UnityContainerExtension, IDisposable
    {
        private IScheduler m_scheduler;
        private ISchedulerFactory m_schedulerfactory;

        protected override void Initialize()
        {
            //NameValueCollection properties = new NameValueCollection();
            //properties["quartz.scheduler.instanceName"] = "ServerScheduler";

            //// set thread pool info
            //properties["quartz.threadPool.type"] = "Quartz.Simpl.SimpleThreadPool, Quartz";
            //properties["quartz.threadPool.threadCount"] = "5";
            //properties["quartz.threadPool.threadPriority"] = "Normal";

            //// job initialization plugin handles our xml reading, without it defaults are used
            //properties["quartz.plugin.xml.type"] = "Quartz.Plugin.Xml.XMLSchedulingDataProcessorPlugin, Quartz";
            //properties["quartz.plugin.xml.fileNames"] = "~/jobs.xml";
           // properties["quartz.scheduler.jobFactory.type"] = "Ach.Fulfillment.Scheduler.UnityJobFactory,Ach.Fulfillment.Scheduler";
            
            
           // Container.RegisterType<IJobFactory, UnityJobFactory>();
           // Context.Container.RegisterInstance<ISchedulerFactory>(new StdSchedulerFactory(),new UnitOfWorkLifetimeManager());
            //Context.Container
            //    .RegisterType<IScheduler>(new ContainerControlledLifetimeManager(),
            //               new InjectionFactory(f => f.Resolve<ISchedulerFactory>().GetScheduler()));            
            //m_schedulerfactory = Container.Resolve<ISchedulerFactory>();            
            //m_scheduler = m_schedulerfactory.GetScheduler();
            //m_scheduler.JobFactory = Container.Resolve<IJobFactory>(); 
            //m_scheduler.Start();

            //var container = ServiceLocator.Current.GetInstance<IUnityContainer>();
            Container.RegisterType<IJobFactory, UnityJobFactory>();
            Container.RegisterInstance<ISchedulerFactory>(new StdSchedulerFactory());
            ISchedulerFactory factory = Container.Resolve<ISchedulerFactory>();
            var scheduler = factory.GetScheduler();
            scheduler.JobFactory = Container.Resolve<IJobFactory>();
            scheduler.Start();
        }

        public void Dispose()
        {
            m_scheduler.Shutdown();
        }
    }
}
