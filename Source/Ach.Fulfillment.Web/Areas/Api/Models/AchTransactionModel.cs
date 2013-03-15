namespace Ach.Fulfillment.Web.Areas.Api.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    [Serializable]
    public class AchTransactionModel
    {
        public long AchTransactionId { get; set; }

        public string IndividualIdNumber { get; set; }

        public string ReceiverName { get; set; }

        public string EntryDescription { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime EntryDate { get; set; }

        public int TransactionCode { get; set; }

        public string TransitRoutingNumber { get; set; }

        public string DfiAccountId { get; set; }

        public decimal Amount { get; set; }

        public int ServiceClassCode { get; set; }

        public string EntryClassCode { get; set; }

        [DataType(DataType.MultilineText)]
        public string PaymentRelatedInfo { get; set; }

        [DataType(DataType.Url)]
        public string CallbackUrl { get; set; }
    }
}