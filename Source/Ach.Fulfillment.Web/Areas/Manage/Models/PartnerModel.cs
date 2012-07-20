namespace Ach.Fulfillment.Web.Areas.Manage.Models
{
    using System.ComponentModel.DataAnnotations;

    public class PartnerModel
    {
        [Required]
        [StringLength(255)]
        public string Name { get; set; }
    }
}