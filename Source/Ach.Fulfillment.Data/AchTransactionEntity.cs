namespace Ach.Fulfillment.Data
{
    using System;
    using System.Globalization;

    public class AchTransactionEntity : BaseEntity
    {
        public virtual PartnerEntity Partner { get; set; }

        public virtual AchFileEntity AchFile { get; set; }

        public virtual string IndividualIdNumber { get; set; }

        public virtual string ReceiverName { get; set; }

        public virtual string EntryDescription { get; set; }

        // todo: not used in ach generation??
        public virtual DateTime EntryDate { get; set; }

        public virtual int TransactionCode { get; set; }

        public virtual string TransitRoutingNumber { get; set; }

        public virtual string DfiAccountId { get; set; }

        public virtual decimal Amount { get; set; }

        // todo: not used in ach generation??
        public virtual int ServiceClassCode { get; set; }

        public virtual string EntryClassCode { get; set; }

        public virtual string PaymentRelatedInfo { get; set; }

        public virtual string CallbackUrl { get; set; }
        
        public virtual AchTransactionStatus Status { get; set; }

        public virtual AchTransactionStatus NotifiedStatus { get; set; }

        public override string ToString()
        {
            // do not use any other property than Id to not force proxy object loading
            var result = string.Format(
                CultureInfo.InvariantCulture,
                "AchTransaction#{0}",
                this.Id);
            return result;
        }
    }
}
