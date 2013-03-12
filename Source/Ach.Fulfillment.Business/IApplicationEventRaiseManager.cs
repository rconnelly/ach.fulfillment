namespace Ach.Fulfillment.Business
{
    using Ach.Fulfillment.Data;

    public interface IApplicationEventRaiseManager
    {
        void RaiseAchTransactionCreatedNotification(AchTransactionEntity instance);

        void RaiseAchFileStatusChangedNotification(AchFileEntity instance);
    }
}