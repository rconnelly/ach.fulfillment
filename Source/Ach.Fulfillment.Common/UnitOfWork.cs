namespace Ach.Fulfillment.Common
{
    using System;

    using Ach.Fulfillment.Common.Unity;

    public sealed class UnitOfWork : IDisposable
    {
        #region Constructors

        public UnitOfWork()
        {
            UnitOfWorkLifetimeManager.Enable();
        }

        #endregion

        #region Methods

        public void Dispose()
        {
            UnitOfWorkLifetimeManager.Clear();
            UnitOfWorkLifetimeManager.Disable();
        }

        #endregion
    }
}