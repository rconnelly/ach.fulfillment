namespace Ach.Fulfillment.Persistence.Impl.Commands
{
    using Ach.Fulfillment.Data.Common;

    internal interface IActionCommand
    {
        void Execute(IActionData queryData);
    }
}