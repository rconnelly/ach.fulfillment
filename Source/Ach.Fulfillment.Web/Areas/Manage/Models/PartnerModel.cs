namespace Ach.Fulfillment.Web.Areas.Manage.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    using Ach.Fulfillment.Data;

    public class PartnerModel
    {
        [HiddenInput]
        public long? PartnerId { get; set; }

        [Required]
        [StringLength(MetadataInfo.StringNormal)]
        public string Name { get; set; }

        public bool Disabled { get; set; }
    }
}