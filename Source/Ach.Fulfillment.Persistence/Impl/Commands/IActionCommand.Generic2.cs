namespace Ach.Fulfillment.Persistence.Impl.Commands
{
    using Ach.Fulfillment.Data.Common;

    internal interface IActionCommand<in TActionData> : IActionCommand
        where TActionData : IActionData
    {
        void Execute(TActionData queryData);
    }
}