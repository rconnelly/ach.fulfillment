using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Ach.Fulfillment.Data;
using Ach.Fulfillment.Data.Common;

namespace Ach.Fulfillment.Business.Impl
{
    internal class AchTransactionManager : ManagerBase<AchTransactionEntity>, IAchTransactionManager
    {
        public override AchTransactionEntity Create(AchTransactionEntity transaction)
        {
            Contract.Assert(transaction != null);

            return base.Create(transaction);
        }
       
    }
}
