// ReSharper disable CheckNamespace
namespace Ach.Fulfillment.Web
// ReSharper restore CheckNamespace
{
    using System.Web.Mvc;

    public class FilterConfig
    {
        #region Public Methods and Operators

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new AuthorizeAttribute());
        }

        #endregion
    }
}