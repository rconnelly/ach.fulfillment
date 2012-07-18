namespace Ach.Fulfillment.Web.Common.Controllers
{
    using System.Web.Mvc;

    using Microsoft.Practices.ServiceLocation;

    public class Controller<TManager> : Controller
        where TManager : class, new ()
    {
        private TManager manager;

        public TManager Manager
        {
            get
            {
//                return this.manager ?? (this.manager = new TManager());
                return this.manager ?? (this.manager = ServiceLocator.Current.GetInstance<TManager>());
            }
        }
    }
}