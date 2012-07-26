namespace Ach.Fulfillment.Web.Areas.Manage.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    using Foolproof;

    public class UserModel
    {
        [HiddenInput]
        public long? UserId { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; }

        [Required]
        [StringLength(255)]
        public string Login { get; set; }

        [RequiredIf("UserId", Operator.EqualTo, null, ErrorMessage = "Password is required")]
        public string Password { get; set; }

        [DataType(DataType.EmailAddress)]
        [StringLength(255)]
        public string Email { get; set; }

        public long RoleId { get; set; }
        
        public long? PartnerId { get; set; }

        public Dictionary<long, string> AvailablePartners { get; set; }

        public Dictionary<long, string> AvailableRoles { get; set; }
    }
}