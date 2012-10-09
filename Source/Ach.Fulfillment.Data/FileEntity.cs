using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ach.Fulfillment.Data
{
    public class FileEntity : BaseEntity
    {
        public virtual string Name { get; set; }
        public virtual string FileIdModifier { get; set; }
        public virtual string FileStatus { get; set; }
        public virtual bool Locked { get; set; }
        public virtual PartnerEntity Partner { get; set; }
        public virtual ICollection<AchTransactionEntity> Transactions { get; set; } 
    }
}
