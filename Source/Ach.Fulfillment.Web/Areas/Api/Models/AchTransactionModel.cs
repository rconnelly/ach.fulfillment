namespace Ach.Fulfillment.Web.Areas.Api.Models
{
    using System;

    [Serializable]
    public class AchTransactionModel
    {
        public long AchTransactionId { get; set; }

        public string IndividualIdNumber { get; set; }

        public string ReceiverName { get; set; }

        public string EntryDescription { get; set; }

        public DateTime EntryDate { get; set; }

        public string TransactionCode { get; set; }

        public string TransitRoutingNumber { get; set; }

        public string DfiAccountId { get; set; }

        public decimal Amount { get; set; }
        
        public string ServiceClassCode { get; set; }

        public string EntryClassCode { get; set; }

        public string PaymentRelatedInfo { get; set; }

        public string CallbackUrl { get; set; }
    }
}