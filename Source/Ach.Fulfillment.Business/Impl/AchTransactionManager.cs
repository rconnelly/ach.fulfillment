﻿namespace Ach.Fulfillment.Business.Impl
{
    using System.Diagnostics.Contracts;

    using Ach.Fulfillment.Business.Impl.Validation;
    using Ach.Fulfillment.Common.Transactions;
    using Ach.Fulfillment.Data;

    internal class AchTransactionManager : ManagerBase<AchTransactionEntity>, IAchTransactionManager
    {
        #region Public Methods and Operators

        public override AchTransactionEntity Create(AchTransactionEntity transaction)
        {
            Contract.Assert(transaction != null);

            transaction.Status = AchTransactionStatus.Created;
            this.DemandValid<AchTransactionValidator>(transaction);

            AchTransactionEntity instance;
            using (var tx = new Transaction())
            {
                instance = base.Create(transaction);

                tx.Complete();
            }

            return instance;
        }

        #endregion
    }
}