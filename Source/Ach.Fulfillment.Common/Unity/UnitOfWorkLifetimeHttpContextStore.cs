namespace Ach.Fulfillment.Common.Unity
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Web;

    public class UnitOfWorkLifetimeHttpContextStore : UnitOfWorkLifetimeStore
    {
        #region Constants

        private const string EnabledKey = "uow_enabled";

        private const string ValuesKey = "uow_values";

        #endregion

        #region Properties

        protected override bool Enabled
        {
            get
            {
                Contract.Assert(HttpContext.Current != null);
                var v = HttpContext.Current.Items[EnabledKey] ?? false;
                return (bool)v;
            }

            set
            {
                Contract.Assert(HttpContext.Current != null);
                HttpContext.Current.Items[EnabledKey] = value;
            }
        }

        protected override Dictionary<Guid, object> Values
        {
            get
            {
                Contract.Assert(HttpContext.Current != null);
                var v = (Dictionary<Guid, object>)HttpContext.Current.Items[ValuesKey];
                return v;
            }

            set
            {
                Contract.Assert(HttpContext.Current != null);
                HttpContext.Current.Items[ValuesKey] = value;
            }
        }

        #endregion
    }
}