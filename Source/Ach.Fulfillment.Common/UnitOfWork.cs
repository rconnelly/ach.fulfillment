﻿namespace Ach.Fulfillment.Common
{
    using System;

    using Ach.Fulfillment.Common.Unity;

    public sealed class UnitOfWork : IDisposable
    {
        #region Constructors

        public UnitOfWork()
        {
            UnitOfWorkLifetimeManager.Store.Enable();
        }

        #endregion

        #region Methods

        public void Dispose()
        {
            UnitOfWorkLifetimeManager.Store.Clear();
            UnitOfWorkLifetimeManager.Store.Disable();
        }

        #endregion
    }
}