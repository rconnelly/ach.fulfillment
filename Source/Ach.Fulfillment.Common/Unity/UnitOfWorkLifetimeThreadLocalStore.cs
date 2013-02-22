namespace Ach.Fulfillment.Common.Unity
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    public class UnitOfWorkLifetimeThreadLocalStore : UnitOfWorkLifetimeStore
    {
        #region Fields

        private readonly ThreadLocal<bool> enabled = new ThreadLocal<bool>();

        private readonly ThreadLocal<Dictionary<Guid, object>> values = new ThreadLocal<Dictionary<Guid, object>>();

        #endregion

        #region Properties

        protected override bool Enabled
        {
            get
            {
                return this.enabled.Value;
            }

            set
            {
                this.enabled.Value = value;
            }
        }

        protected override Dictionary<Guid, object> Values
        {
            get
            {
                return this.values.Value;
            }

            set
            {
                this.values.Value = value;
            }
        }

        #endregion
    }
}