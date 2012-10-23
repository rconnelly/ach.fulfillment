namespace Ach.Fulfillment.Scheduler.Common
{
    using System;
    using System.Threading;

    using Ach.Fulfillment.Common;
    using Ach.Fulfillment.Scheduler.Configuration;

    using global::Common.Logging;

    using Microsoft.Practices.ServiceLocation;

    using Quartz;

    /// <summary>
    /// The main server logic.
    /// </summary>
    internal class QuartzServer
    {
        #region Constants and Fields

        private readonly ILog logger;

        private IScheduler scheduler;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="QuartzServer"/> class.
        /// </summary>
        public QuartzServer()
        {
            this.logger = LogManager.GetCurrentClassLogger();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Returns the current scheduler instance (usually created in <see cref="Initialize"/> using the <see cref="GetScheduler"/> method).
        /// </summary>
        protected virtual IScheduler Scheduler
        {
            get
            {
                return this.scheduler;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
            // no-op for now
        }

        /// <summary>
        /// Initializes the instance of the <see cref="QuartzServer"/> class.
        /// </summary>
        public virtual void Initialize()
        {
            try
            {
                Shell.Start<SchedulerContainerExtension>();
                this.scheduler = this.GetScheduler();
            }
            catch (Exception e)
            {
                this.logger.Error("Server initialization failed:" + e.Message, e);
                throw;
            }
        }

        /// <summary>
        /// Pauses all activity in scheduler.
        /// </summary>
        public virtual void Pause()
        {
            this.scheduler.PauseAll();
        }

        /// <summary>
        /// Resumes all activity in server.
        /// </summary>
        public virtual void Resume()
        {
            this.scheduler.ResumeAll();
        }

        /// <summary>
        /// Starts this instance, delegates to scheduler.
        /// </summary>
        public virtual void Start()
        {
            this.scheduler.Start();

            try
            {
                Thread.Sleep(3000);
            }
            catch (ThreadInterruptedException)
            {
            }

            this.logger.Info("Scheduler started successfully");
        }

        /// <summary>
        /// Stops this instance, delegates to scheduler.
        /// </summary>
        public virtual void Stop()
        {
            try
            {
                this.scheduler.Shutdown(true);
                this.logger.Info("Scheduler shutdown complete");
            }
            finally
            {
                Shell.Shutdown();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the scheduler with which this server should operate with.
        /// </summary>
        /// <returns>
        /// </returns>
        protected virtual IScheduler GetScheduler()
        {
            var factory = ServiceLocator.Current.GetInstance<ISchedulerFactory>();
            var instance = factory.GetScheduler();
            return instance;
        }

        #endregion
    }
}