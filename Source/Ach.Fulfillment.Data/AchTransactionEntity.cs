namespace Ach.Fulfillment.Data
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class AchTransactionEntity : BaseEntity
    {
        public virtual PartnerEntity Partner { get; set; }

        // todo: how StringLength attribute will be used
        [StringLength(15)]
        public virtual string IndividualIdNumber { get; set; }

        [StringLength(22)]
        public virtual string ReceiverName { get; set; }

        [StringLength(10)]
        public virtual string EntryDescription { get; set; }

        public virtual DateTime EntryDate { get; set; }

        [StringLength(2)]
        public virtual string TransactionCode { get; set; }

        [StringLength(9)]
        public virtual string TransitRoutingNumber { get; set; }

        [StringLength(17)]
        public virtual string DfiAccountId { get; set; }

        public virtual decimal Amount { get; set; }

        [StringLength(3)]
        public virtual string ServiceClassCode { get; set; }

        [StringLength(3)]
        public virtual string EntryClassCode { get; set; }

        [StringLength(80)]
        public virtual string PaymentRelatedInfo { get; set; }

        public virtual string CallbackUrl { get; set; }
        
        public virtual AchTransactionStatus TransactionStatus { get; set; }

        public virtual bool Locked { get; set; }
    }
}
