namespace Ach.Fulfillment.Business
{
    using Ach.Fulfillment.Data;

    public interface IApplicationEventRaiseManager
    {
        void RaiseAchFileStatusChangedNotification(AchFileEntity instance);
    }
}