namespace Ach.Fulfillment.Persistence.Impl.Commands
{
    using Ach.Fulfillment.Data.Common;

    internal abstract class ActionCommandBase<TActionData> : IActionCommand<TActionData>
        where TActionData : IActionData
    {
        public abstract void Execute(TActionData queryData);

        void IActionCommand.Execute(IActionData queryData)
        {
            this.Execute((TActionData)queryData);
        }
    }
}