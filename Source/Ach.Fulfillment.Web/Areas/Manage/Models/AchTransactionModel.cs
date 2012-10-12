namespace Ach.Fulfillment.Web.Areas.Manage.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;
    
    public class AchTransactionModel
    {
        [HiddenInput]
        public long? AchTransactionId { get; set; }

        [Required]
        [StringLength(15)]
        public string IndividualIdNumber { get; set; }

        [Required]
        [StringLength(22)]
        public string ReceiverName { get; set; }

        [Required]
        public string EntryDescription { get; set; }

        [Required]
        public DateTime EntryDate { get; set; }

        [Required]
        public string TransactionCode { get; set; }

        [Required]
        public string TransitRoutingNumber { get; set; }

        [Required]
        public string DfiAccountId { get; set; }

        [Required]
        public decimal Amount { get; set; }

        public string ServiceClassCode { get; set; }

        [Required]
        public string EntryClassCode { get; set; }
        
        public string PaymentRelatedInfo { get; set; }

        [Required]
        public string CallbackUrl { get; set; }

    }

}