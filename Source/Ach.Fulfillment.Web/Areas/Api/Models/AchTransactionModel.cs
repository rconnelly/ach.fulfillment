namespace Ach.Fulfillment.Web.Areas.Api.Models
{
    using System;

    public class AchTransactionModel
    {
        /*[HiddenInput]
        public long? AchTransactionId { get; set; }*/

        /*[Required(ErrorMessage = "Individual Id Number is required.")]
        [StringLength(15, MinimumLength = 15, ErrorMessage = "Individual Id Number must be 15 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessage = "Individual Id Number must have alphanumeric data")]*/
        public string IndividualIdNumber { get; set; }

        /*[Required(ErrorMessage = "Receiver Name is required.")]
        [StringLength(22, ErrorMessage = "Receiver Name lenght must be under 22 characters")]
        [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessage = "Receiver Name must have alphanumeric data")]*/
        public string ReceiverName { get; set; }

        /*[Required(ErrorMessage = "Entry Description is required.")]
        [StringLength(10, ErrorMessage = "Entry Description lenght must be under 10 characters")]
        [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessage = "Entry Description must have alphanumeric data")]*/
        public string EntryDescription { get; set; }

        /*[Required(ErrorMessage = "Entry Date is required.")]             
        [DataType(DataType.DateTime)]*/
        public DateTime EntryDate { get; set; }

        /*[Required(ErrorMessage = "Transaction Code is required.")]
        [RegularExpression(@"^(22|23|24|27|28|29|32|33|34|37|38|39)$", ErrorMessage = "Transaction Code: valid codes are 22, 23, 24, 27, 28, 29, 32, 33, 34, 37, 38, 39")]*/
        public string TransactionCode { get; set; }

        /*[Required(ErrorMessage = "Transit Routing Number is required.")]
        [StringLength(9, MinimumLength = 9, ErrorMessage = "Transit Routing Number lenght must be 9 characters")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Transit Routing Number must have numeric data")]*/
        public string TransitRoutingNumber { get; set; }

        /*[Required(ErrorMessage = "Dfi Account Id is required.")]
        [StringLength(17, ErrorMessage = "Dfi Account Id lenght must be under 17 characters")]
        [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessage = "Dfi Account Id must have alphanumeric data")]*/
        public string DfiAccountId { get; set; }

        /*[Required(ErrorMessage = "Amount is required.")]
        [RegularExpression(@"^[0-9]{1,8}(\,[0-9]{1,2})$", ErrorMessage = "Amount must be amount id dollars with two decimal places after ',' ")]*/
        public decimal Amount { get; set; }
        
        /*[RegularExpression(@"^(200|220|225)$", ErrorMessage = "Service Class Code: valid codes are 200, 220, 225")]*/
        public string ServiceClassCode { get; set; }

        /*[Required(ErrorMessage = "Standard Entry Class Code is required.")]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "Standard Entry Class Code lenght must be 3 characters")]
        [RegularExpression(@"^[A-Z]*$", ErrorMessage = "Standard Entry Class Code must have 3 uppercase characters")]*/
        public string EntryClassCode { get; set; }

        /*[StringLength(94, ErrorMessage = "Payment Related Info lenght must be under 94 characters")]*/
        public string PaymentRelatedInfo { get; set; }

        /*[Required(ErrorMessage = "CallbackUrl is required.")]
        [StringLength(255)]
        [DataType(DataType.Url, ErrorMessage = "CallbackUrl format is wrong")]*/
        public string CallbackUrl { get; set; }
    }
}