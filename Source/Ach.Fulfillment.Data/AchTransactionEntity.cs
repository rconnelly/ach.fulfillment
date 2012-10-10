using System;
using System.ComponentModel.DataAnnotations;

namespace Ach.Fulfillment.Data
{
    public class AchTransactionEntity:BaseEntity
    {
        public virtual PartnerEntity Partner { get; set; }

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
        public virtual string DFIAccountId { get; set; }

        public virtual decimal Amount { get; set; }

        [StringLength(3)]
        public virtual string ServiceClassCode { get; set; }

        [StringLength(3)]
        public virtual string EntryClassCode { get; set; }

        [StringLength(80)]
        public virtual string PaymentRelatedInfo { get; set; }

        public virtual string CallbackUrl { get; set; }
        
        public virtual TransactionStatus TransactionStatus { get; set; }

        public virtual bool Locked { get; set; }

    }

    public enum TransactionStatus
    {
        Received = 0,
        Batched = 1,
        InProgress = 2,
        Complete = 3,
        Error = 4
    }
}
