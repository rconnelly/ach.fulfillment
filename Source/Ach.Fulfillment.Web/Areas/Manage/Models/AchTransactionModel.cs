using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Ach.Fulfillment.Data;
using System.ComponentModel.DataAnnotations;

namespace Ach.Fulfillment.Web.Areas.Manage.Models
{
 
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
        public string Amount { get; set; }

        [Required]
        public string AccountId { get; set; }

        [Required]
        public string RoutingNumber { get; set; }

    }
}