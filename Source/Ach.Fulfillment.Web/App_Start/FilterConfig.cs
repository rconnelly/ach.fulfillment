namespace Ach.Fulfillment.Web.App_Start
{
    using System.Web.Mvc;

    using Ach.Fulfillment.Web.Common.Filters;

    public class FilterConfig
    {
        #region Public Methods and Operators

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new WebHandleErrorAttribute());
            filters.Add(new AuthorizeAttribute());
        }

        #endregion
    }
}