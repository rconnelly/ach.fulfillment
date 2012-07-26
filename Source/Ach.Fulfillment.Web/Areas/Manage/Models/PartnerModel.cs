namespace Ach.Fulfillment.Web.Areas.Manage.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    public class PartnerModel
    {
        [HiddenInput]
        public long? PartnerId { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; }
    }
}