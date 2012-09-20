using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Ach.Fulfillment.Data.Common;

namespace Ach.Fulfillment.Data.Specifications
{
    public class AchTransactionInQueue: SpecificationBase<AchTransactionEntity>
    {
        public override Expression<Func<AchTransactionEntity, bool>> IsSatisfiedBy()
        {
            return m => m.IsQueued;
        }
    }
}
