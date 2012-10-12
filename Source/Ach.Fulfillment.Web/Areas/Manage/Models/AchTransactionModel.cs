namespace Ach.Fulfillment.Web.Areas.Manage.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    public class AchTransactionModel
    {
        [HiddenInput]
        public long? AchTransactionId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Id { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string CallbackUrl { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public string AccountId { get; set; }

        [Required]
        public string RoutingNumber { get; set; }

    }
}