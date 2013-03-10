namespace Ach.Fulfillment.Data
{
    using System.Collections.Generic;

    public class AchFileEntity : BaseEntity
    {
        public virtual string Name { get; set; } 

        public virtual string FileIdModifier { get; set; }

        public virtual AchFileStatus FileStatus { get; set; }

        public virtual PartnerEntity Partner { get; set; }

        public virtual IList<AchTransactionEntity> Transactions { get; set; } 
    }
}
