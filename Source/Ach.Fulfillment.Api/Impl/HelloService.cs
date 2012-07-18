namespace Ach.Fulfillment.Api.Impl
{
    using Ach.Fulfillment.Api.Common;
    using Ach.Fulfillment.Api.Data;
    using Ach.Fulfillment.Business;

    using Microsoft.Practices.Unity;

    using ServiceStack.Common.Web;
    using ServiceStack.ServiceInterface;

    [Authenticate]
    [RequiredPermission("Admin")]
    [UnitOfWork]
    public class HelloService : RestServiceBase<Hello>
    {
        [Dependency]
        public IUserManager UerManager { get; set; }

        // Get
        public override object OnGet(Hello request) 
        {
            var session = this.GetSession();
            
            var user = this.UerManager.Load(long.Parse(session.UserAuthId));

            var name = session.DisplayName + " " + user.Email;
            if (string.IsNullOrEmpty(request.Name))
            {
                return new HelloResponse { Result = "Hello from " + name};
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