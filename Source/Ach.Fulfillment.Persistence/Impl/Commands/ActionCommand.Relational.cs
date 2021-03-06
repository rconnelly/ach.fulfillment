namespace Ach.Fulfillment.Persistence.Impl.Commands
{
    using Ach.Fulfillment.Data.Common;

    using Microsoft.Practices.Unity;

    using NHibernate;

    internal abstract class RelationalActionCommand<TActionData> : ActionCommandBase<TActionData>
        where TActionData : IActionData
    {
        #region Public Properties

        [Dependency]
        public ISession Session { get; set; }

        #endregion
    }
}