using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ach.Fulfillment.Business.Impl.Enumeration
{
    public enum AchTransactionStatus
    {
        Received, 
        Batched, 
        InProgress, 
        Complete, 
        Error
    }
}
