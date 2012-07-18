namespace Ach.Fulfillment.Api.Impl
{
    using Ach.Fulfillment.Api.Common;
    using Ach.Fulfillment.Api.Data;
    using Ach.Fulfillment.Business;
    using Ach.Fulfillment.Common.Security;
    using Ach.Fulfillment.Data;

    using Microsoft.Practices.Unity;

    using ServiceStack.Common.Web;
    using ServiceStack.ServiceInterface;

    [Authenticate]
    [RequiredPermission(AccessRightRegistry.Admin)]
    [UnitOfWork]
    public class HelloService : RestServiceBase<Hello>
    {
        [Dependency]
        public IUserManager UerManager { get; set; }

        [Dependency]
        public IApplicationPrincipal Principal { get; set; }

        // Get
        public override object OnGet(Hello request) 
        {
            var user = this.UerManager.Load(this.Principal.Identity.UserId);

            var name = this.Principal.Identity.DisplayName + " " + user.Email;
            if (string.IsNullOrEmpty(request.Name))
            {
                return new HelloResponse { Result = "Hello from " + name };
            }

            return new HelloResponse { Result = "Hello, " + request.Name + " from " + name };
        }

        // Add
        public override object OnPost(Hello request)
        {
            return new HttpError(System.Net.HttpStatusCode.Conflict, "SomeAddErrorCode");
        }

        // Update
        public override object OnPut(Hello request)
        {
            return new HttpError(System.Net.HttpStatusCode.Conflict, "SomeUpdateErrorCode");
        }

        // Delete
        public override object OnDelete(Hello request)
        {
            throw new HttpError(System.Net.HttpStatusCode.Conflict, "SomeDeleteErrorCode");
        }
    }
}