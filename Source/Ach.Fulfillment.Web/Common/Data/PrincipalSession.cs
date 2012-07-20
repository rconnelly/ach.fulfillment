namespace Ach.Fulfillment.Web.Common.Data
{
    public class PrincipalSession
    {
        public long UserId { get; set; }
        public string Login { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string[] Role { get; set; }
        public string[] Permissions { get; set; }
    }
}