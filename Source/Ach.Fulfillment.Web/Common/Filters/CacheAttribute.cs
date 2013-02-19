namespace Ach.Fulfillment.Web.Common.Filters
{
    using System.Web;
    using System.Web.Mvc;
    using System.Web.UI;

    public class CacheAttribute : OutputCacheAttribute
    {
        #region Constants and Fields

        private OutputCacheLocation? originalLocation;

        #endregion

        #region Constructors and Destructors

        public CacheAttribute()
        {
            this.Location = OutputCacheLocation.Any;
            this.Duration = 300; /*default cache time*/
            this.DisableForAuthenticatedUser = false;
        }

        #endregion

        #region Properties

        public bool DisableForAuthenticatedUser { get; set; }

        #endregion

        #region Public Methods and Operators

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            this.OnCacheEnabled(filterContext);
            base.OnResultExecuting(filterContext);
        }

        #endregion

        #region Methods

        protected void OnCacheEnabled(ResultExecutingContext filterContext)
        {
            var httpContext = filterContext.HttpContext;
            if (this.IsCacheDisabled(httpContext))
            {
                // it's crucial not to cache Authenticated content
                this.originalLocation = this.originalLocation ?? this.Location;
                this.Location = OutputCacheLocation.None;
            }
            else
            {
                this.Location = this.originalLocation ?? this.Location;
            }

            if (this.DisableForAuthenticatedUser)
            {
                // this smells a little but it works
                httpContext.Response.Cache.AddValidationCallback(this.IgnoreAuthenticated, null);
            }
        }

        private bool IsCacheDisabled(HttpContextBase httpContext)
        {
            return 
                httpContext.IsDebuggingEnabled || 
                (this.DisableForAuthenticatedUser && httpContext.User.Identity.IsAuthenticated);
        }

        // This method is called each time when cached page is going to be
        // served and ensures that cache is ignored for authenticated users.
        private void IgnoreAuthenticated(HttpContext context, object data, ref HttpValidationStatus validationStatus)
        {
            validationStatus = context.User.Identity.IsAuthenticated
                                   ? HttpValidationStatus.IgnoreThisRequest
                                   : HttpValidationStatus.Valid;
        }

        #endregion
    }
}