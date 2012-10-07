using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Ach.Fulfillment.Business;
using Ach.Fulfillment.Data;

namespace Ach.Fulfillment.Web.Areas.Manage.Models
{
    public class AchTransactionModel //: BaseModel<AchTransactionEntity>
    {
      //  [HiddenInput]
       // public long? AchTransactionId { get; set; }

        [Required]
       // [StringLength(15)]
        public string IndividualIdNumber { get; set; }

        [Required]
       // [StringLength(22)]
        public string ReceiverName { get; set; }

        [Required]
        public string EntryDescription { get; set; }

        //[Required]
        //public DateTime EntryDate { get; set; }

        //[Required]
        //public string TransactionCode { get; set; }

        [Required]
        public string TransitRoutingNumber { get; set; }

        [Required]
        public string DFIAccountId { get; set; }

        [Required]
        public decimal Amount { get; set; }

        //[Required]
        //public string ServiceClassCode { get; set; }

        //[Required]
        //public string EntryClassCode { get; set; }

        //[Required]
        //public string PaymentRelatedInfo { get; set; }

        [Required]
        public string CallbackUrl { get; set; }


    }

}