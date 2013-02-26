namespace Ach.Fulfillment.Persistence.Impl
{
    using System;
    using System.Configuration;
    using System.Diagnostics.Contracts;
    using System.Linq;

    using Ach.Fulfillment.Data.Common;
    using Ach.Fulfillment.Persistence.Impl.Commands;
    using Ach.Fulfillment.Persistence.Impl.Configuration;

    using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
    using Microsoft.Practices.ServiceLocation;
    using Microsoft.Practices.Unity;

    using NHibernate;

    internal class Repository : IRepository
    {
        #region Fields

        private readonly Func<ISession> sessionProducer;

        #endregion

        #region Constructors

        public Repository(Func<ISession> sessionProducer)
        {
            Contract.Assert(sessionProducer != null);
            this.sessionProducer = sessionProducer;
        }

        #endregion

        #region Properties

        [Dependency]
        public IServiceLocator ServiceLocator { get; set; }

        private ISession Session
        {
            get
            {
                return this.sessionProducer();
            }
        }

        #endregion

        #region Methods

        public IQueryable<T> Query<T>(IQueryData queryData)
        {
            Contract.Assert(queryData != null);
            var command = this.ResolveCommand<T>(queryData);
            var result = command.Execute(queryData);
            return result;
        }

        public T Scalar<T>(IQueryData queryData)
        {
            Contract.Assert(queryData != null);
            var command = this.ResolveCommand<T>(queryData);
            var result = command.ExecuteScalar(queryData);
            return result;
        }

        public int Count<T>(IQueryData queryData)
        {
            Contract.Assert(queryData != null);
            var command = this.ResolveCommand<T>(queryData);
            var count = command.RowCount(queryData);
            return count;
        }

        public void Execute(IActionData actionData)
        {
            var command = this.ResolveCommand(actionData);
            try
            {
                command.Execute(actionData);
            }
            catch (Exception ex)
            {
                var rethrow = ExceptionPolicy.HandleException(ex, PersistenceContainerExtension.ExecutePolicy);
                if (rethrow)
                {
                    throw;
                }
            }
        }

        public void Flush(bool force = false)
        {
            if (force || this.Session.FlushMode == FlushMode.Commit)
            {
                this.Session.Flush();
            }
        }

        private IQueryCommand<T> ResolveCommand<T>(IQueryData commandData)
        {
            Contract.Assert(commandData != null);
            try
            {
                var type = typeof(IQueryCommand<,>).MakeGenericType(commandData.GetType(), typeof(T));
                var command = (IQueryCommand<T>)this.ServiceLocator.GetInstance(type);
                return command;
            }
            catch (ActivationException ex)
            {
                throw new ConfigurationErrorsException("Cannot find corresponding query command for " + commandData.GetType().FullName, ex);
            }
        }

        private IActionCommand ResolveCommand(IActionData actionData)
        {
            Contract.Assert(actionData != null);
            var actionDataType = actionData.GetType();
            try
            {
                var interfaceType = typeof(IActionCommand<>).MakeGenericType(actionDataType);
                var command = (IActionCommand)this.ServiceLocator.GetInstance(interfaceType);
                return command;
            }
            catch (ActivationException ex)
            {
                throw new ConfigurationErrorsException("Cannot find corresponding action command for " + actionDataType.FullName, ex);
            }
        }

        #endregion
    }
}