namespace Ach.Fulfillment.Web.Services
{
    using System;
    using System.Linq;

    using Ach.Fulfillment.Business;
    using Ach.Fulfillment.Common;
    using Ach.Fulfillment.Data.Specifications;
    using Ach.Fulfillment.Web.Areas.Manage.Models;

    using Microsoft.Practices.Unity;

    using global::Common.Logging;

    public class ApiContext : IDisposable
    {
        #region Constants and Fields

        private IQueryable<UserGridModel> users;

        private readonly UnitOfWork unitOfWork;

        private static readonly ILog Log = LogManager.GetCurrentClassLogger(); 

        #endregion

        public ApiContext ()
        {
            Log.Debug("Creating ApiContext instance");

            unitOfWork = new UnitOfWork();
        }

        #region Public Properties

        [Dependency]
        public IUserManager UserManager { get; set; }

        public IQueryable<UserGridModel> Users
        {
            get
            {
                if (this.users == null)
                {
                    // var list = this.UserManager.FindAll(new UserPaged(0, 20));
                    var list = this.UserManager.FindAll(new UserAll());
                    this.users = (from u in list
                                  select
                                      new UserGridModel
                                          {
                                              Id = (int)u.Id,
                                              Name = u.Name,
                                              Email = u.Email,
                                              Login = u.UserPasswordCredential != null ? u.UserPasswordCredential.Login : string.Empty
                                          }).AsQueryable();
                }

                return this.users;
            }
        }

        #endregion

        public void Dispose()
        {
            Log.Debug("Disposing ApiContext");

            this.Dispose(true);
        }

        protected virtual void Dispose (bool disposing)
        {
            if(disposing)
            {
                if(this.unitOfWork != null)
                {
                    this.unitOfWork.Dispose();
                }
            }
        }
    }
}