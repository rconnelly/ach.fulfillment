﻿namespace Ach.Fulfillment.Data
{
    using System.Collections.Generic;

    public class FileEntity : BaseEntity
    {
        public virtual string Name { get; set; }
        public virtual string FileIdModifier { get; set; }
        public virtual int FileStatus { get; set; }
        public virtual bool Locked { get; set; }
        public virtual PartnerEntity Partner { get; set; }
        public virtual IList<AchTransactionEntity> Transactions { get; set; } 
    }
}
