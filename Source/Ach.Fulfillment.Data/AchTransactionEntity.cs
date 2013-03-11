namespace Ach.Fulfillment.Data
{
    using System;

    public class AchTransactionEntity : BaseEntity
    {
        public virtual PartnerEntity Partner { get; set; }

        public virtual string IndividualIdNumber { get; set; }

        public virtual string ReceiverName { get; set; }

        public virtual string EntryDescription { get; set; }

        public virtual DateTime EntryDate { get; set; }

        public virtual string TransactionCode { get; set; }

        public virtual string TransitRoutingNumber { get; set; }

        public virtual string DfiAccountId { get; set; }

        public virtual decimal Amount { get; set; }

        public virtual string ServiceClassCode { get; set; }

        public virtual string EntryClassCode { get; set; }

        public virtual string PaymentRelatedInfo { get; set; }

        public virtual string CallbackUrl { get; set; }
        
        public virtual AchTransactionStatus Status { get; set; }

        public virtual AchTransactionStatus NotifiedStatus { get; set; }

        public virtual bool Locked { get; set; }
    }
}
