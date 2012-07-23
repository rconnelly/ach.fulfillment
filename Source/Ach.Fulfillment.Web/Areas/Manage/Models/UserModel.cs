namespace Ach.Fulfillment.Web.Areas.Manage.Models
{
    using System.ComponentModel.DataAnnotations;

    public class UserModel
    {
        [Required]
        [StringLength(255)]
        public string Name { get; set; }

        [Required]
        [StringLength(255)]
        public string Login { get; set; }

        [Required]
        public string Password { get; set; }

        [DataType(DataType.EmailAddress)]
        [StringLength(255)]
        public string Email { get; set; }

        public long? PartnerId { get; set; }
    }
}